using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class ABParse : MonoBehaviour
{
    private delegate bool Comparator(int i, int j);

    private const float WAIT_TIME = 1.0f;
    private const float WAIT_TIME_SHORT = 0.5f;

    private CameraScript mCamera;
    private TextScript mInfoText;
    private ABTextScript mABText;

    void Awake()
    {
        mCamera = GameObject.Find("Main Camera").GetComponent<CameraScript>();
        mInfoText = GameObject.Find("InfoText").GetComponent<TextScript>();
        mABText = GameObject.Find("ABText").GetComponent<ABTextScript>();
    }

    public IEnumerator parseABCoroutine(MinMaxTree tree)
    {
        GetComponent<UIReader>().toggleUI(false, UICode.PARSE_AB, UICode.PARSE_MM, UICode.GENERATE, UICode.MAX_VAL, UICode.MIN_VAL);
        GetComponent<UIReader>().toggleUI(true, UICode.AB_TEXT);
        Node root = tree.root;
        yield return StartCoroutine(parseABCoroutineHelper(root, Data.minVal - 1, Data.maxVal + 1));
        mInfoText.updateText("Tree Parsed! The value of the best move is " + root.val);
        toggleBestMoveGlow(root);
        GetComponent<UIReader>().toggleUI(true, UICode.GENERATE, UICode.MAX_VAL, UICode.MIN_VAL);
        GetComponent<UIReader>().toggleUI(false, UICode.AB_TEXT);
    }

    /// <summary>
    /// Toggles the node glow on the best move
    /// </summary>
    /// <param name="root"> The root node of the tree </param>
    private static void toggleBestMoveGlow(Node root)
    {
        for (int i = 0; i < root.numChildren; i++)
        {
            if (root.children[i].val != root.val) continue;
            root.children[i].toggleGlow();
            break;
        }
    }

    /// <summary>
    /// Recursive helper to parse the tree
    /// </summary>
    /// <param name="current"> The current node </param>
    /// <param name="alpha"> the minimum value for the maximizing player </param>
    /// <param name="beta"> the maximum value for the minimizing player </param>
    /// <returns> the coroutine value </returns>
    private IEnumerator parseABCoroutineHelper(Node current, int alpha, int beta)
    {
        mCamera.setPosition(current);
        mABText.updateText(alpha, beta);
        int val = 0;
        initialNodeSetup(current, ref val);
        for (int i = 0; i < current.numChildren; i++)
        {
            if (current.children[i].isUninitialized())
            {
                yield return evaluateChild(current, alpha, beta, i);
                mABText.updateText(alpha, beta);
            }
            mInfoText.updateText("Is this move any good?");
            current.children[i].glow(WAIT_TIME_SHORT * 2 * Data.timeVal);
            yield return new WaitForSeconds(WAIT_TIME_SHORT * Data.timeVal);
            Comparator compare = evaluateNodeType(current, ref alpha, ref beta, ref val, i);
            if (compare(val, current.val))
            {
                mInfoText.updateText(i != 0 ? "Currently the best option - Discarding previous options" : "Currently the best option");
                yield return new WaitForSeconds(WAIT_TIME_SHORT * Data.timeVal);
                current.arrows[current.children[i]].changeColorPerm(ArrowScript.LineColor.GREEN);
                updateArrowColors(current, i);
            }
            else
            {
                mInfoText.updateText("Previous options are better, discarding current node");
                yield return new WaitForSeconds(WAIT_TIME_SHORT * Data.timeVal);
                current.arrows[current.children[i]].changeColorPerm(ArrowScript.LineColor.RED);
                if (!current.children[i].isLeaf()) updateArrowColors(current.children[i], current.children[i].numChildren);
            }
            current.assignNodeVal(val);
            mABText.updateText(alpha, beta);
            yield return new WaitForSeconds(WAIT_TIME_SHORT * Data.timeVal);

            // Checks the alpha and beta values to see if we can prune the tree (Also checks to make sure we don't do unnecessary pruning)
            if (beta > alpha || i + 1 == current.numChildren) continue;

            mInfoText.updateText("Beta <= Alpha, Pruning Tree!");
            pruneOtherChildren(current, i);
            yield return new WaitForSeconds(WAIT_TIME * Data.timeVal);
            break;
        }
    }

    /// <summary>
    /// Evaluates a child of the current node
    /// </summary>
    /// <param name="current"> the current node </param>
    /// <param name="alpha"> the current alpha value </param>
    /// <param name="beta"> the current beta value </param>
    /// <param name="i"> the position of the child to evaluate </param>
    /// <returns> the Coroutine value </returns>
    private IEnumerator evaluateChild(Node current, int alpha, int beta, int i)
    {
        mInfoText.updateText("Child is uninitialized, Moving to evaluate it");

        StartCoroutine(current.arrows[current.children[i]].changeColor(WAIT_TIME * Data.timeVal, ArrowScript.LineColor.GREEN));
        yield return new WaitForSeconds(WAIT_TIME * Data.timeVal);

        yield return StartCoroutine(parseABCoroutineHelper(current.children[i], alpha, beta));
        mInfoText.updateText("Returing to Parent");
        mCamera.setPosition(current);

        StartCoroutine(current.arrows[current.children[i]].changeColor(WAIT_TIME_SHORT*Data.timeVal, ArrowScript.LineColor.GREEN));
        yield return new WaitForSeconds(WAIT_TIME*Data.timeVal);
    }

    private static void initialNodeSetup(Node current, ref int val)
    {
        switch (current.type)
        {
            case NodeType.MIN:
                val = Data.maxVal + 1;
                current.val = Data.maxVal + 1;
                break;
            case NodeType.MAX:
                val = Data.minVal - 1;
                current.val = Data.minVal - 1;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static Comparator evaluateNodeType(Node current, ref int alpha, ref int beta, ref int val, int i)
    {
        Comparator compare;
        switch (current.type)
        {
            case NodeType.MIN:
                val = Mathf.Min(val, current.children[i].val);
                beta = Mathf.Min(beta, val);
                compare = (x, y) => x < y;
                break;
            case NodeType.MAX:
                val = Mathf.Max(val, current.children[i].val);
                alpha = Mathf.Max(alpha, val);
                compare = (x, y) => x > y;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return compare;
    }

    private static void updateArrowColors(Node current, int i)
    {
        if (current.isLeaf()) return;
        for (int j = i - 1; j >= 0; j--)
        {
            current.arrows[current.children[j]].changeColorPerm(ArrowScript.LineColor.RED);
            updateArrowColors(current.children[j], current.children[j].numChildren);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="current"></param>
    /// <param name="i"></param>
    private static void pruneOtherChildren(Node current, int i)
    {
        for (i++; i < current.numChildren; i++)
        {
            current.arrows[current.children[i]].changeColorPerm(ArrowScript.LineColor.YELLOW);
            if (!current.isLeaf()) pruneOtherChildren(current.children[i], -1);
        }
    }
}
