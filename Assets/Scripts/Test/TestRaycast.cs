using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRaycast : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Rayを飛ばして画面のxy座標をワールド座標に変換
        Vector3 screenPoint = Input.mousePosition; screenPoint.z = 10.0f;
        Ray screenPointToRay = Camera.main.ScreenPointToRay(screenPoint);

        // デバッグ機能を利用して、シーンビューでレイが出ているか見てみよう。
        Debug.DrawRay(screenPointToRay.origin, screenPointToRay.direction * 1000.0f);
    }
}
