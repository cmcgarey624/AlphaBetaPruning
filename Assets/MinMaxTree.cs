using System.Collections.Generic;

public class MinMaxTree
{
    public Node root { get; private set; }
    public int depth { get; private set; }

    public MinMaxTree()
    {
        root = new Node();
        depth = 1;
    }

    public MinMaxTree(int theDepth)
    {
        root = new Node();
        depth = theDepth;
    }

    public List<Node> getAllLeafNodes()
    {
        return getAllNodesAtDepth(depth);
    }

    /// <summary>
    /// Gets every node at a particular depth
    /// </summary>
    /// <param name="theDepth"> the depth to search </param>
    /// <returns> A list of all the nodes at a particular depth </returns>
    public List<Node> getAllNodesAtDepth(int theDepth)
    {
        if (theDepth > depth || theDepth < 1) return null;
        var nodes = new List<Node>();
        getAllNodesAtDepthHelper(theDepth, 1, nodes, root);
        return nodes;
    }

    /// <summary>
    /// Recursive helper function called by getAllNodesAtDepth
    /// </summary>
    /// <param name="targetDepth"> The desired depth </param>
    /// <param name="currentDepth"> The current depth </param>
    /// <param name="currentNodes"> A list of all the currently added nodes </param>
    /// <param name="currentNode"> The current node </param>
    private static void getAllNodesAtDepthHelper(int targetDepth, int currentDepth, List<Node> currentNodes, Node currentNode)
    {
        if (currentDepth == targetDepth)
        {
            currentNodes.Add(currentNode);
            return;
        }
        foreach (Node n in currentNode.children)
        {
            getAllNodesAtDepthHelper(targetDepth, currentDepth + 1, currentNodes, n);
        }
    }

    /// <summary>
    /// Recursivly gets the number of total children (not just the children in the immediate family) of a particular node
    /// </summary>
    /// <param name="n"> The current node </param>
    /// <returns> How many children the current node has in total </returns>
    public int getTotalNumChildren(Node n)
    {
        if (n.isLeaf()) return 0;
        int total = 0;
        for (int i = 0; i < n.numChildren; i++)
        {
            total++;
            total += getTotalNumChildren(n.children[i]);
        }
        return total;
    }
}