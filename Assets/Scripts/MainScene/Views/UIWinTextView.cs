using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWinTextView : MonoBehaviour
{
    float diff = 0.05f;
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(0.1f, 1, 1);
    }

    private void OnEnable()
    {
        transform.localScale = new Vector3(0.1f, 1, 1);
        enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localScale.x < 1f)
        {
            transform.localScale = new Vector3(transform.localScale.x + diff, 1, 1);
            new WaitForSeconds(0.02f);
        }
        else
        {
            enabled = false;
        }
    }
}
