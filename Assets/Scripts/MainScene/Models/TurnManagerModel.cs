using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManagerModel : MonoBehaviour
{
    public enum TurnState
    {
        BLUE = 1,
        RED = -1,
        FINISH = 0
    }
    TurnState turn;
    public int _turnNum;
    public int NowTurnNum { get { return _turnNum; } }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        _turnNum = 1;
        turn = TurnState.BLUE;
    }

    public void NextTurn()
    {
        turn = (TurnState)((int)turn * (-1));
        _turnNum++;
    }

    public bool IsMyTurn(int playerColor)
    {
        if (playerColor == (int)turn)
        {
            return true;
        }
        return false;
    }

    public bool IsBlueTurn()
    {
        return turn == TurnState.BLUE;
    }

    public bool IsRedTurn()
    {
        return turn == TurnState.RED;
    }

    public bool IsGameFinish()
    {
        return turn == TurnState.FINISH;
    }

    public void FinishGame()
    {
        turn = TurnState.FINISH;
    }
}

