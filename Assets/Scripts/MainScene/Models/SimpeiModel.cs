using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpeiModel : MonoBehaviour
{
    public int simpeiColor { get; set; }
    public int PutPoint { get; set; }

    void Start()
    {
        Initialize();
        simpeiColor = (transform.gameObject.CompareTag("SimpeiBlue")) ? 1 : -1;
    }

    public void Initialize()
    {
        PutPoint = -1;
    }
}
