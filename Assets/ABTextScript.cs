using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ABTextScript : MonoBehaviour
{
    private Text mMyText;

    void Awake()
    {
        mMyText = GetComponent<Text>();
    }

    public void updateText(int alpha, int beta)
    {
        mMyText.text = "Alpha: " + alpha + "\nBeta: " + beta;
    }
}
