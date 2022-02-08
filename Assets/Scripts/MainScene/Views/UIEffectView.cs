using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEffectView : MonoBehaviour
{
    [SerializeField] float width = 720.0f;
    [SerializeField] float height = 360.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, 60) * Time.deltaTime, Space.World);
    }
}
