using UnityEngine;
using System.Collections;

public class NodeScript : MonoBehaviour
{
    private GameObject mNodeGlow;

    void Awake()
    {
        mNodeGlow = transform.Find("Glow").gameObject;
        mNodeGlow.SetActive(false);
    }

    public void glow(float time)
    {
        StartCoroutine(glowCoroutine(time));
    }

    public void toggleGlow()
    {
        mNodeGlow.SetActive(!mNodeGlow.activeInHierarchy);
    }

    private IEnumerator glowCoroutine(float time)
    {
        mNodeGlow.SetActive(true);
        yield return new WaitForSeconds(time);
        mNodeGlow.SetActive(false);
    }
}
