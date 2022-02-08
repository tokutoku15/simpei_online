using UnityEngine;
using UnityEngine.EventSystems;

public class BoardPointView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{   
    GameController gameController;
    // 自分自身のGamePlayer
    GamePlayer myPlayer;
    int _pointNum;
    public int PointNum { set { _pointNum = value; } }

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        RegisterMyPlayer();
    }

    public void RegisterMyPlayer()
    {
        //Debug.Log("BoardPointView RegisterMyPlayer");
        GamePlayer[] players = FindObjectsOfType<GamePlayer>();
        if (players[0].IsMe) myPlayer = players[0];
        else myPlayer = players[1];
    }

    public void OnPointerClick(PointerEventData data)
    {
        // Debug.Log($"BoardPointView{_pointNum} is Clicked");
        if (!myPlayer.IsMyTurn()) return;
        bool isDone = myPlayer.SelectPoint(_pointNum);
        if (isDone) gameController.OnBoardPointMouseExit();
    }

    public void OnPointerEnter(PointerEventData data) 
    {
        // Debug.Log($"BoardPointView{_pointNum} is Enter");
        if (!myPlayer.IsMyTurn()) return;
        gameController.OnBoardPointMouseEnter(_pointNum, transform.position);
    }

    public void OnPointerExit(PointerEventData data)
    {
        //Debug.Log($"BoardPointView{_pointNum} is Exit");
        gameController.OnBoardPointMouseExit();
    }

    // void OnGUI()
    // {
        //string label = $"My Player is {myPlayer}";
        //GUI.color = Color.black;
        //GUI.Label(new Rect(0, 0, 300, 100), label);
    // }
}
