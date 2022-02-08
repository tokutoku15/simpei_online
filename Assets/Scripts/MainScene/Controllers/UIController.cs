using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [Header("GameController")]
    [SerializeField] GameController gameController;

    [Header("WinUI")]
    [SerializeField] GameObject BackGround;
    [SerializeField] GameObject ResultEffect;
    [SerializeField] GameObject BlueWinUI;
    [SerializeField] GameObject RedWinUI;

    [Header("NextGamePanel")]
    [SerializeField] GameObject NextGameCanvas;
    [SerializeField] TextMeshProUGUI nextGameTextMeshPro;
    [SerializeField] GameObject PlayAgainButton;
    [SerializeField] GameObject ExitButton;

    [Header("MatchingPlayerNameWindow")]
    [SerializeField] PlayerCardUI myPanel;
    [SerializeField] PlayerCardUI enemyPanel;

    [Header("MessageWindow")]
    [SerializeField] MessageWindowUI messageWindow;
    [SerializeField] TextContainer textContainer;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        //nextGameTextMeshPro.text = areYouPlayAgain;
        nextGameTextMeshPro.text = textContainer.areYouPlayAgain;
    }

    private void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        BlueWinUI.SetActive(false);
        RedWinUI.SetActive(false);
        BackGround.SetActive(false);
        ResultEffect.SetActive(false);
        NextGameCanvas.SetActive(false);
        PlayAgainButton.SetActive(true);
        ExitButton.SetActive(true);
        nextGameTextMeshPro.text = textContainer.areYouPlayAgain;
    }

    public void PlayerWin(int playerColor)
    {
        BackGround.SetActive(true);
        ResultEffect.SetActive(true);
        if (playerColor == 1)
        {
            BlueWinUI.SetActive(true);
        }
        else
        {
            RedWinUI.SetActive(true);
        }
    }

    public void AskNextDoing()
    {
        NextGameCanvas.SetActive(true);
    }

    public void WaitForOpponent()
    {
        //nextGameTextMeshPro.text = waitForOpponent;
        nextGameTextMeshPro.text = textContainer.waitForOpponent;
    }

    public void OpponentExit()
    {
        nextGameTextMeshPro.text = textContainer.opponentExit;
        PlayAgainButton.SetActive(false);
        ExitButton.SetActive(true);
    }

    public IEnumerator MatchingWindowAnimation(GamePlayer player)
    {
        if (player.IsMe)
        {
            yield return StartCoroutine(myPanel.MatchingAnimation(player.PlayerID, player.PlayerColor, player.MyNickName()));
        }
        else
        {
            yield return StartCoroutine(enemyPanel.MatchingAnimation(player.PlayerID, player.PlayerColor, player.MyNickName()));
        }
        Debug.Log("UIController MatchingWindowAnimation");
        if (player.IsMe)
        {
            Debug.Log("UIController MatchingWindowAnimation Done");
            yield return new WaitForSeconds(1.5f);
            gameController.FinishDisplayPlayerCardAnimation();
        }
    }

    public void SetMessageWindowColor(int playerColor)
    {
        messageWindow.gameObject.SetActive(true);
        messageWindow.SetMessageWindowColor(playerColor);
    }

    public void FirstTurnMessage()
    {
        messageWindow.SetText(textContainer.firstTurnMessage);
    }

    public void CanPutEveryPoint()
    {
        messageWindow.SetText(GameDataManager.PlayerName + textContainer.canPutEveryPoint);
    }

    public void CannotMoveAnyPoint()
    {
        messageWindow.SetText(textContainer.cannotMoveAnyPoint);
    }

    public void SelectMySimpei()
    {
        messageWindow.SetText(textContainer.selectMySimpei);
    }

    public void SelectEnemySimpei()
    {
        messageWindow.SetText(textContainer.selectEnemySimpei);
    }

    public void DontDecideWinOrLose()
    {
        messageWindow.SetText(textContainer.dontDecideWinOrLose);
    }

    public void MoveToNeighborhood()
    {
        messageWindow.SetText(textContainer.moveToNeighborhood);
    }

    public void NowIsEnemyTurn()
    {
        messageWindow.SetText(GameDataManager.RemotePlayerName + textContainer.NowIsEnemyTurn);
    }

    public void MoveEnemySimpei()
    {
        messageWindow.SetText(textContainer.moveEnemySimpei);
    }
}
