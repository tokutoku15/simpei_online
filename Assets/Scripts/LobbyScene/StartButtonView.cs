using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButtonView : MonoBehaviour
{
    RectTransform rect;
    bool isAnimated;
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        isAnimated = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseEnter()
    {
        //Debug.Log("StartButtonView OnMouseEnter");
        StartCoroutine(ExpandButton());
    }

    IEnumerator ExpandButton()
    {
        if (isAnimated) yield break;
        isAnimated = true;
        while (rect.localScale.x < 1.1f)
        {
            rect.localScale += new Vector3(0.02f, 0.02f, 0);
            yield return new WaitForSeconds(0.02f);
        }
        while (rect.localScale.x > 1f)
        {
            rect.localScale -= new Vector3(0.02f, 0.02f, 0);
            yield return new WaitForSeconds(0.02f);
        }
        isAnimated = false;
    }
}
