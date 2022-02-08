using UnityEngine;

public class TextContainer : MonoBehaviour
{
    [Header("TextMessage")]
    [SerializeField] public string areYouPlayAgain;
    [TextArea(1, 2)] public string waitForOpponent;
    [SerializeField] public string opponentExit;
    [Space]
    [TextArea(1, 5)] public string firstTurnMessage;
    [TextArea(1, 5)] public string canPutEveryPoint;
    [TextArea(1, 5)] public string cannotMoveAnyPoint;
    [TextArea(1, 5)] public string selectMySimpei;
    [TextArea(1, 5)] public string selectEnemySimpei;
    [TextArea(1, 5)] public string dontDecideWinOrLose;
    [TextArea(1, 5)] public string moveToNeighborhood;
    [TextArea(1, 5)] public string NowIsEnemyTurn;
    [TextArea(1, 5)] public string moveEnemySimpei;

    private void Awake()
    {
        Debug.Log(GameDataManager.PlayerName);
        Debug.Log(GameDataManager.RemotePlayerName);
    }
}