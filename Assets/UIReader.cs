using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public enum UICode
{
    MAX_NODES,
    MIN_NODES,
    DEPTH,
    GENERATE,
    PARSE_MM,
    PARSE_AB,
    MIN_VAL,
    MAX_VAL,
    AB_TEXT
}

public class UIReader : MonoBehaviour
{
    private Dictionary<UICode, GameObject> mUIElements;
    private Dictionary<UICode, Text> mTextElements;

    void Awake()
    {

        mTextElements = new Dictionary<UICode, Text>
        {
            {UICode.DEPTH, GameObject.Find("Depth").transform.FindChild("Text").gameObject.GetComponent<Text>()},
            {UICode.MIN_NODES, GameObject.Find("MinNodes").transform.FindChild("Text").gameObject.GetComponent<Text>()},
            {UICode.MAX_NODES, GameObject.Find("MaxNodes").transform.FindChild("Text").gameObject.GetComponent<Text>()},
            {UICode.MIN_VAL, GameObject.Find("MinVal").transform.FindChild("Text").gameObject.GetComponent<Text>()},
            {UICode.MAX_VAL, GameObject.Find("MaxVal").transform.FindChild("Text").gameObject.GetComponent<Text>()}
        };

        mUIElements = new Dictionary<UICode, GameObject>
        {
            {UICode.MAX_NODES, GameObject.Find("MaxNodes")},
            {UICode.MIN_NODES, GameObject.Find("MinNodes")},
            {UICode.DEPTH, GameObject.Find("Depth")},
            {UICode.GENERATE, GameObject.Find("Start")},
            {UICode.PARSE_MM, GameObject.Find("ParseMM")},
            {UICode.PARSE_AB, GameObject.Find("ParseAB")},
            {UICode.MAX_VAL, GameObject.Find("MaxVal")},
            {UICode.MIN_VAL, GameObject.Find("MinVal")},
            {UICode.AB_TEXT, GameObject.Find("ABText")}
        };
        Data.timeVal = 1;
    }

    void Start()
    {
        toggleUI(false, UICode.PARSE_AB, UICode.PARSE_MM, UICode.AB_TEXT);
    }

    /// <summary>
    /// Reads in the scrollbar value and updates the speed
    /// </summary>
    public void readScroll()
    {
        float f = GameObject.Find("Scrollbar").GetComponent<Scrollbar>().value;
        Data.timeVal = 1.0f - f*0.9f;
    }

    /// <summary>
    /// Generates the tree from the given parameters
    /// </summary>
    public void activate()
    {
        int min, max, depth, maxVal, minVal;
        if (!int.TryParse(mTextElements[UICode.DEPTH].text, out depth) || !int.TryParse(mTextElements[UICode.MIN_NODES].text, out min) ||
            !int.TryParse(mTextElements[UICode.MAX_NODES].text, out max) || !int.TryParse(mTextElements[UICode.MIN_VAL].text, out minVal) ||
            !int.TryParse(mTextElements[UICode.MAX_VAL].text, out maxVal)) return;
        // Checks to make sure the values are valid
        Debug.Log(Mathf.Pow((max + min) / 2.0f, depth - 1) + 1);
        if (min < 1 || depth < 1 || min > max || maxVal <= minVal || Mathf.Pow((max + min)/2.0f, depth-1) + 1 > 1050.0f) return;
        Data.maxVal = maxVal;
        Data.minVal = minVal;
        toggleUI(true, UICode.PARSE_AB, UICode.PARSE_MM);
        GetComponent<TreeGeneration>().generate(min, max, depth);
    }

    /// <summary>
    /// Toggles certain UI elements on or off
    /// </summary>
    /// <param name="toggle"> whether to turn the elements on or off </param>
    /// <param name="codes"> The UICodes of the elements to turn on or off </param>
    public void toggleUI(bool toggle, params UICode[] codes)
    {
        for (int i = 0; i < codes.Length; i++)
        {
            GameObject theObject;
            if (!mUIElements.TryGetValue(codes[i], out theObject)) throw new ArgumentOutOfRangeException("codes");
            theObject.SetActive(toggle);
        }
    }
}
