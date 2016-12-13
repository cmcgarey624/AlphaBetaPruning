using UnityEngine;
using DG.Tweening;
using System.Collections;

public class CameraScript : MonoBehaviour
{
    // Used to convert an X distance to camera size
    private const float SIZE_TO_X_DIST = 1.6f;

    // Camera Size constraints
    private const float MIN_SIZE = 3.0f;
    //private const float MAX_SIZE = 10.0f;

    // The time it takes the camera to move
    private const float CAMERA_TIME = 0.3f;

    /// <summary>
    /// Tweens the position of the camera to the position of the node
    /// </summary>
    /// <param name="n"> The Node to tween to </param>
    public void setPosition(Node n)
    {
        Vector3 pos = n.go.transform.position;
        pos.z = -10;
        //if (!n.isLeaf()) scaleCamera(n);
        gameObject.transform.DOMove(pos, CAMERA_TIME);
    }

    void Update()
    {
        GetComponent<Camera>().orthographicSize -= 5 * Input.GetAxis("Mouse ScrollWheel");
        if (GetComponent<Camera>().orthographicSize <= 3.0f) GetComponent<Camera>().orthographicSize = 3.0f;
    }

    /// <summary>
    /// Scales the camera so it can see all the children of the current node
    /// </summary>
    /// <param name="n"> The node in question </param>
    public void scaleCamera(Node n)
    {
        float leftPos = n.children[0].go.transform.position.x - 0.5f;
        float rightPos = n.children[n.children.Count - 1].go.transform.position.x + 0.5f;
        float pos = n.go.transform.position.x;

        float distLeft = Mathf.Abs(leftPos - pos);
        float distRight = Mathf.Abs(rightPos - pos);
        float dist = Mathf.Max(distRight, distLeft);
        float desiredSize = dist/SIZE_TO_X_DIST;
        if (desiredSize < MIN_SIZE) desiredSize = MIN_SIZE;
        
        StartCoroutine(scale(desiredSize));
    }

    /// <summary>
    /// Uses a coroutine to scale the camera so the scaling won't snap
    /// </summary>
    /// <param name="desiredSize"> The desired size to scale to </param>
    /// <returns> returns each frame so it only scales a bit each frame </returns>
    private IEnumerator scale(float desiredSize)
    {
        float startSize = GetComponent<Camera>().orthographicSize;
        float time = 0;
        do
        {
            GetComponent<Camera>().orthographicSize = Mathf.Lerp(startSize, desiredSize, time/(CAMERA_TIME * Data.timeVal));
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        } while (time < CAMERA_TIME * Data.timeVal);
    }
}
