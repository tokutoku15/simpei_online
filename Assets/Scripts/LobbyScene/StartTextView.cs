using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTextView : MonoBehaviour
{
    RectTransform rect;
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        Debug.Log(rect.localScale.x);
        StartCoroutine(ChangeScale());
    }

    // Update is called once per frame
    void Update()
    {
        //new WaitForSeconds(0.03f);
        //rect.localScale = new Vector3(2, 2, 1);
        //new WaitForSeconds(0.03f);
        //rect.sizeDelta = new Vector2(400, 200);
    }

    IEnumerator ChangeScale()
    {
        while(true)
        {
            while(rect.localScale.x < 1.1f)
            {
                rect.localScale += new Vector3(0.02f, 0.02f, 0);
                yield return new WaitForSeconds(0.02f);
            }
            while(rect.localScale.x > 0.9f)
            {
                rect.localScale -= new Vector3(0.02f, 0.02f, 0);
                yield return new WaitForSeconds(0.02f);
            }
        }
    }
}
