using UnityEngine;
using UnityEngine.UI;

public class TextScript : MonoBehaviour
{
    private Text mMyText;

	void Awake()
	{
	    mMyText = GetComponent<Text>();
	}

    public void updateText(string newText)
    {
        mMyText.text = newText;
    }
}
