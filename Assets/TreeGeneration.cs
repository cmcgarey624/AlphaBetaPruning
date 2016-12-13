using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class TreeGeneration : MonoBehaviour
{
    // Generation Spacing Data
    private const float NODE_SIZE = 1.0f;
    private const float CHILD_SPACING = 0.25f;
    private float mDepthSpacing;

    private MinMaxTree mTree;
    
    public GameObject NodePrefab;
    public GameObject ConnectionPrefab;


    /// <summary>
    /// Generates a tree
    /// </summary>
    /// <param name="minNodes"> the minimum number of children each node can have </param>
    /// <param name="maxNodes"> the maximum number of children each node can have </param>
    /// <param name="depth"> the desired depth of the tree </param>
    public void generate(int minNodes, int maxNodes, int depth)
    {
        mDepthSpacing = 1.5f * Mathf.CeilToInt(maxNodes/7.0f);
        clearGameObjects();
        mTree = new MinMaxTree(depth);
        int numNodes = 1;
        for (int i = 1; i < depth; i++)
        {
            List<Node> currentNodes = mTree.getAllNodesAtDepth(i);
            foreach (Node n in currentNodes)
            {
                generateChildren(n, minNodes, maxNodes, i, ref numNodes);
            }
        }
        generateGameObjects(mTree);
        assignValsToLeafNodes();
    }

    /// <summary>
    /// Deletes all the GameObjects
    /// </summary>
    private void clearGameObjects()
    {
        GameObject[] toDelete = GameObject.FindGameObjectsWithTag("Delete");
        foreach (GameObject g in toDelete)
        {
            Destroy(g);
        }
    }

    /// <summary>
    /// Generates GameObjects for each node in the tree
    /// </summary>
    /// <param name="tree"> the tree to generate GameObjects for </param>
    private void generateGameObjects(MinMaxTree tree)
    {
        Vector3 currentPos = Vector3.zero;
        generateGameObjectsHelper(tree.root, null, currentPos);
    }

    /// <summary>
    /// Calculates the total amount of space needed underneath the node
    /// </summary>
    /// <param name="n"> the node </param>
    /// <returns> the total amount of space </returns>
    private float calcTotalSpaceUnder(Node n)
    {
        if (n.isLeaf()) return NODE_SIZE;
        float size = 0;
        int i = 0;
        foreach (Node child in n.children)
        {
            i++;
            size += calcTotalSpaceUnder(child);
            if (i != n.children.Count) size += CHILD_SPACING;
        }
        return size;
    }

    /// <summary>
    /// Recursively generates all GameObjects
    /// </summary>
    /// <param name="current"> the current node </param>
    /// <param name="parent"> the parent of the current node </param>
    /// <param name="currentPos"> the position of the current node </param>
    private void generateGameObjectsHelper(Node current, Node parent, Vector3 currentPos)
    {
        // Instantiate the node
        instantiateNode(current, parent, currentPos);
        if (current.isLeaf()) return;
        // Find the space needed underneath
        var spaceUnder = getSpaceUnderDict(current);
        float totalSpaceUnder = spaceUnder.Values.Sum() + (spaceUnder.Count - 1) * CHILD_SPACING;
        // Get the position of the leftmost child (the left edge)
        Vector3 edge = currentPos - new Vector3(totalSpaceUnder / 2.0f, mDepthSpacing, 0);
        int i = 0;
        foreach (var data in spaceUnder)
        {
            // Get the position of the new node (x pos is half the space required of children
            Vector3 newPos = edge;
            newPos.x += data.Value/2;
            // Get the position of the new left edge
            edge.x += data.Value;
            i++;
            // Unless it is the last child, add the child spacing
            if (i != spaceUnder.Count) edge.x += CHILD_SPACING;
            generateGameObjectsHelper(data.Key, current, newPos);
        }
    }

    /// <summary>
    /// Instantiates a GameObject to represent the node, as well as the connection to the parent
    /// </summary>
    /// <param name="current"> the node to instantiate </param>
    /// <param name="parent"> the parent of the current node </param>
    /// <param name="currentPos"> the position the node should be instatiated at </param>
    private void instantiateNode(Node current, Node parent, Vector3 currentPos)
    {
        GameObject g = Instantiate(NodePrefab, currentPos, Quaternion.identity) as GameObject;
        g.name = current.id.ToString();
        if (current.isLeaf()) g.GetComponent<SpriteRenderer>().color = Color.black;
        else g.GetComponent<SpriteRenderer>().color = current.type == NodeType.MAX ? Color.green : Color.red;
        current.go = g;
        if (parent == null) return;
        GameObject parentGO = parent.go;
        GameObject newConn = Instantiate(ConnectionPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        newConn.name = "" + parent.id + " To " + current.id;
        newConn.GetComponent<ArrowScript>().drawConnection(parentGO, g);
        parent.arrows.Add(current, newConn.GetComponent<ArrowScript>());
    }

    /// <summary>
    /// Gets the amount of space needed underneath each node
    /// </summary>
    /// <param name="current"> the node </param>
    /// <returns> Each child node and how much space they need underneath them </returns>
    private Dictionary<Node, float> getSpaceUnderDict(Node current)
    {
        Dictionary<Node, float> spaceUnder = new Dictionary<Node, float>();
        foreach (Node n in current.children)
        {
            float space = calcTotalSpaceUnder(n);
            spaceUnder.Add(n, space);
        }
        return spaceUnder;
    }

    /// <summary>
    /// Generates all children of the given node
    /// </summary>
    /// <param name="node"> the current node </param>
    /// <param name="minNodes"> the minimum number of child nodes </param>
    /// <param name="maxNodes"> the maximum number of child nodes </param>
    /// <param name="depth"> the desired depth of the tree </param>
    /// <param name="numNodes"> the total number of nodes in the tree </param>
    private static void generateChildren(Node node, int minNodes, int maxNodes, int depth, ref int numNodes)
    {
        int numChildren = Random.Range(minNodes, maxNodes + 1);
        NodeType type = (NodeType) ((depth - 1) % 2);
        for (int i = 0; i < numChildren; i++)
        {
            node.children.Add(new Node(node, type, numNodes));
            numNodes++;
        }
    }

    private void assignValsToLeafNodes()
    {
        List<Node> leafs = mTree.getAllLeafNodes();
        foreach (Node leaf in leafs)
        {
            leaf.assignNodeVal(Random.Range(Data.minVal, Data.maxVal + 1));
        }
    }

    public void parseMinMax()
    {
        if (mTree == null) return;
        StartCoroutine(GetComponent<MinMaxParse>().parseMinMaxCoroutine(mTree));
    }

    public void parseAlphaBeta()
    {
        if (mTree == null) return;
        StartCoroutine(GetComponent<ABParse>().parseABCoroutine(mTree));
    }
}