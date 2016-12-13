using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum NodeType
{
    MIN,
    MAX
}

public class Node
{

    public int id { get; private set; }
    public int val { get; set; }
    public int numChildren { get { return children.Count; } }
    public NodeType type { get; set; }
    public Node parent { get; set; }
    public List<Node> children { get; set; }

    // A Dictionary that represents all of the connections the node has to its children
    public Dictionary<Node, ArrowScript> arrows { get; set; }

    // The GameObject that represents the node
    public GameObject go { get; set; }

    public Node()
    {
        parent = null;
        val = -200;
        children = new List<Node>();
        arrows = new Dictionary<Node, ArrowScript>();
        type = NodeType.MAX;
        id = 0;
    }

    public Node(Node p, NodeType type, int id)
    {
        parent = p;
        val = -200;
        children = new List<Node>();
        arrows = new Dictionary<Node, ArrowScript>();
        this.type = type;
        this.id = id;
    }

    public bool isLeaf()
    {
        return children.Count == 0;
    }

    public bool isUninitialized()
    {
        return val == -200;
    }

    public bool hasUninitializedChildren()
    {
        for (int i = 0; i < children.Count; i++)
        {
            if (children[i].isUninitialized()) return true;
        }
        return false;
    }

    public Node getNextUninitializedChild()
    {
        for (int i = 0; i < children.Count; i++)
        {
            if (children[i].isUninitialized()) return children[i];
        }
        return null;
    }

    public void assignNodeVal(int theVal)
    {
        if (go == null) return;
        Text t = go.transform.FindChild("Canvas").FindChild("ValText").gameObject.GetComponent<Text>();
        val = theVal;
        t.text = val.ToString();
    }

    public void glow(float time)
    {
        go.GetComponent<NodeScript>().glow(time);
    }

    public void toggleGlow()
    {
        go.GetComponent<NodeScript>().toggleGlow();
    }
}