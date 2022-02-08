using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPointHolder : MonoBehaviour
{
    [SerializeField] GameObject pointPrefab;
    [SerializeField] float pointHeight = 5.0f;

    BoardModel board;
    BoardPointView[] boardPointViews = new BoardPointView[25];

    // Start is called before the first frame update
    void Start()
    {
        // ボードポイントにそれぞれの番号を割り当てる
        board = FindObjectOfType<BoardModel>();
        float[,] wp = board.WP;
        for (int i = 0; i < wp.GetLength(0); i++)
        {
            boardPointViews[i] = Instantiate(
                pointPrefab,
                new Vector3(wp[i, 0], pointHeight, wp[i, 1]),
                Quaternion.identity,
                transform
                ).GetComponent<BoardPointView>();
            boardPointViews[i].PointNum = i;
        }
    }
}
