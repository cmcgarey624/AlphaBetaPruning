using System;
using UnityEngine;
using System.Collections;

public class MinMaxParse : MonoBehaviour {

    private delegate bool Comparator(int i, int j);

    private const float WAIT_TIME = 1.0f;
    private const float WAIT_TIME_SHORT = 0.5f;

    private CameraScript mCamera;
    private TextScript mInfoText;

    void Awake()
    {
        mCamera = GameObject.Find("Main Camera").GetComponent<CameraScript>();
        mInfoText = GameObject.Find("InfoText").GetComponent<TextScript>();
    }

    public IEnumerator parseMinMaxCoroutine(MinMaxTree tree)
    {
        GetComponent<UIReader>().toggleUI(false, UICode.PARSE_AB, UICode.PARSE_MM, UICode.GENERATE, UICode.MIN_VAL, UICode.MAX_VAL);
        Node root = tree.root;
        Node current = root;
        Node bestMove = null;
        while (root.isUninitialized())
        {
            // Set the posistion of the camera to the current node
            mCamera.setPosition(current);
            mInfoText.updateText("Are there unevaluated children?");
            yield return new WaitForSeconds(WAIT_TIME * Data.timeVal);
            if (!current.hasUninitializedChildren()) // If all children of the current node have been evaluated
            {
                Comparator compare;
                int val = evaluateCurrNodeType(current, out compare);
                yield return new WaitForSeconds(WAIT_TIME_SHORT * Data.timeVal);

                for (int i = 0; i < current.numChildren; i++)
                {
                    current.children[i].glow(WAIT_TIME * Data.timeVal);
                    if (compare(current.children[i].val, val))
                    {
                        // If the current child is better than all previous children, discard previous options
                        val = current.children[i].val;
                        current.arrows[current.children[i]].changeColorPerm(ArrowScript.LineColor.GREEN);
                        updateArrowColors(current, i);
                        bestMove = current.children[i];
                        current.assignNodeVal(val);
                    }
                    else
                    {
                        // Otherwise discard this option
                        current.arrows[current.children[i]].changeColorPerm(ArrowScript.LineColor.RED);
                        if (!current.children[i].isLeaf()) updateArrowColors(current.children[i], current.children[i].numChildren);
                    }
                    yield return new WaitForSeconds(WAIT_TIME * Data.timeVal);
                }

                if (current.parent == null) break;
                // If this is not the root node, return to the parent
                StartCoroutine(current.parent.arrows[current].changeColor(WAIT_TIME * Data.timeVal, ArrowScript.LineColor.GREEN));
                current = current.parent;
                mInfoText.updateText("Returning to Parent Node");
                yield return new WaitForSeconds(WAIT_TIME * Data.timeVal);
            }
            else
            {
                mInfoText.updateText("Yes - Evaluating the next unevaluated child");
                yield return new WaitForSeconds(WAIT_TIME_SHORT);
                Node next = current.getNextUninitializedChild();
                StartCoroutine(current.arrows[next].changeColor(WAIT_TIME * Data.timeVal, ArrowScript.LineColor.GREEN));
                current = next;
            }
        }
        mInfoText.updateText("Tree Parsed! The value of the best node is " + bestMove.val);
        bestMove.toggleGlow();
        root.arrows[bestMove].changeColorPerm(ArrowScript.LineColor.GREEN);
        GetComponent<UIReader>().toggleUI(true, UICode.GENERATE, UICode.MIN_VAL, UICode.MAX_VAL);
    }

    /// <summary>
    /// Changes the all the arrow colors of all previous children of the node to red
    /// </summary>
    /// <param name="current"> the current node </param>
    /// <param name="i"> A marker for which children to begin changing </param>
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
    /// Extracts necessary information based on the node's type
    /// </summary>
    /// <param name="current"> the node in question </param>
    /// <param name="c"> The comparison function to be used for each node </param>
    /// <returns></returns>
    private int evaluateCurrNodeType(Node current, out Comparator c)
    {
        int val;
        switch (current.type)
        {
            case NodeType.MIN:
                val = Data.maxVal + 1;
                c = (i, j) => i < j;
                mInfoText.updateText("No - Finding the lowest value from the child nodes");
                break;
            case NodeType.MAX:
                val = Data.minVal - 1;
                c = (i, j) => i > j;
                mInfoText.updateText("No - Finding the highest value from the child nodes");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return val;
    }
}
