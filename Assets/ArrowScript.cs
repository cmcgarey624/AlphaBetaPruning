using System;
using UnityEngine;
using System.Collections;

public class ArrowScript : MonoBehaviour
{
    public GameObject from { get; private set; }
    public GameObject to { get; private set; }


    private GameObject mRedLine;
    private GameObject mGreenLine;
    private GameObject mBlueLine;
    private GameObject mYellowLine;
    private GameObject mCurrActive;

    public enum LineColor
    {
        RED,
        GREEN,
        BLUE,
        YELLOW
    }

    /// <summary>
    /// Draws a connection between two GameObjects
    /// </summary>
    /// <param name="f"> One of the two GameObjects </param>
    /// <param name="t"> The other of two GameObjects </param>
    public void drawConnection(GameObject f, GameObject t)
    {
        // Find all of the LineRenderer GameObjects
        mRedLine = transform.FindChild("RedLine").gameObject;
        mGreenLine = transform.FindChild("GreenLine").gameObject;
        mBlueLine = transform.FindChild("BlueLine").gameObject;
        mYellowLine = transform.FindChild("YellowLine").gameObject;

        from = f;
        to = t;

        Vector3 toPos = t.transform.position;
        Vector3 fromPos = f.transform.position;

        // Shorten the length of the line so that it doesn't bleed into the circles
        Vector3 dir = toPos - fromPos;
        dir.Normalize();
        dir *= 0.5f;
        fromPos += dir;
        toPos -= dir;

        // Set the positions of the LineRenderes
        Vector3[] positions = {fromPos, toPos};
        mRedLine.GetComponent<LineRenderer>().SetPositions(positions);
        mGreenLine.GetComponent<LineRenderer>().SetPositions(positions);
        mBlueLine.GetComponent<LineRenderer>().SetPositions(positions);
        mYellowLine.GetComponent<LineRenderer>().SetPositions(positions);

        // Deactivate all but the initial LineRenderes
        mCurrActive = mBlueLine;
        mGreenLine.SetActive(false);
        mRedLine.SetActive(false);
        mYellowLine.SetActive(false);
    }

    /// <summary>
    /// Changes the color of the line to a different color for a period of time
    /// </summary>
    /// <param name="waitTime"> the time to change the line for </param>
    /// <param name="color"> the color to change to </param>
    /// <returns> The WaitForSeconds value </returns>
    public IEnumerator changeColor(float waitTime, LineColor color)
    {
        GameObject returnTo = mCurrActive;
        mCurrActive.SetActive(false);
        switchCurrActive(color);
        mCurrActive.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        mCurrActive.SetActive(false);
        mCurrActive = returnTo;
        mCurrActive.SetActive(true);
    }

    /// <summary>
    /// Changes the color of the line permanantly
    /// </summary>
    /// <param name="color"> The color to change to </param>
    public void changeColorPerm(LineColor color)
    {
        if (mCurrActive == mYellowLine) return;
        mCurrActive.SetActive(false);
        switchCurrActive(color);
        mCurrActive.SetActive(true);
    }

    /// <summary>
    /// Switches the mCurrActive member variable based on a LineColor
    /// </summary>
    /// <param name="color"> The color to change to </param>
    private void switchCurrActive(LineColor color)
    {
        switch (color)
        {
            case LineColor.RED:
                mCurrActive = mRedLine;
                break;
            case LineColor.GREEN:
                mCurrActive = mGreenLine;
                break;
            case LineColor.BLUE:
                mCurrActive = mBlueLine;
                break;
            case LineColor.YELLOW:
                mCurrActive = mYellowLine;
                break;
            default:
                throw new ArgumentOutOfRangeException("color", color, null);
        }
    }
}