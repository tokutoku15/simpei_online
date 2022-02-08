using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Inspectorで割り当てるゲームオブジェクト(コンポーネント)
    [Header("GameObject or Component")]

    [SerializeField] GameObject pointHolderManagerPrefab;
    [SerializeField] TurnManagerModel turnManagerModel;
    [SerializeField] BoardModel boardModel;
    [SerializeField] EffectController effectController;
    [SerializeField] UIController uiController;
    // コマのオブジェクト
    [SerializeField] GameObject[] blueSimpeis;
    [SerializeField] GameObject[] redSimpeis;

    SimpeiModel[] blueSimpeiModels = new SimpeiModel[4];
    SimpeiModel[] redSimpeiModels = new SimpeiModel[4];

    SimpeiMover[] blueSimpeiMovers = new SimpeiMover[4];
    SimpeiMover[] redSimpeiMovers = new SimpeiMover[4];
    
    GamePlayer[] gamePlayers = new GamePlayer[2];
    PointHolderManager pointHolderManager;

    int targetPoint;

    // ゲームが始まる時に初期化する変数
    GameObject activeSimpei;
    int myColor;
    int oldPoint;
    byte myPlayerIndex;
    bool[] isPlayerWantToPlayAgain = new bool[2];
    // ターン決めに使う(0...プレイヤー1が先攻/1...プレイヤー2が先攻)
    int dice;
    // 挟まれたコマの番号リスト
    List<int> sandwitchSimpeis = new List<int>();
    List<int> tempSandwitchSimpeis = new List<int>();

    // 定数
    const float waitInWaitingLine = 2.0f;
    const float otherHeight = -100f;
    

    public enum Action
    {
        HAVE,
        SELECT,
        HAVE_ENEMY,
        SELECT_ENEMY,
        DONE,
    }
    Action action;
    public Action NowAction { get { return action; } }

    void Awake()
    {
        for (int i = 0; i < 2; i++)
            isPlayerWantToPlayAgain[i] = false;
        dice = 0;

        for (int i = 0; i < 4; i++)
        {
            blueSimpeiModels[i] = blueSimpeis[i].GetComponent<SimpeiModel>();
            blueSimpeiMovers[i] = blueSimpeis[i].GetComponent<SimpeiMover>();
            redSimpeiModels[i] = redSimpeis[i].GetComponent<SimpeiModel>();
            redSimpeiMovers[i] = redSimpeis[i].GetComponent<SimpeiMover>();
        }
    }

    // プレイヤーの登録
    public void RegisterPlayer(GamePlayer player)
    {
        if (player.PlayerID == 1)
        {
            gamePlayers[0] = player;
            if (player.IsMe)
            {
                myPlayerIndex = 0;
            }
            else
            {
                GameDataManager.RemotePlayerName = gamePlayers[0].MyNickName();
            }
        }
        else if (player.PlayerID == 2)
        {
            gamePlayers[1] = player;
            if (player.IsMe)
            {
                myPlayerIndex = 1;
            }
            else
            {
                GameDataManager.RemotePlayerName = gamePlayers[1].MyNickName();
            }
        }
        Debug.Log($"player1 ${gamePlayers[0]}, player2 ${gamePlayers[1]}");

        if (gamePlayers[0] != null && gamePlayers[1] != null)
        {
            Debug.Log($"player1 {gamePlayers[0]}, player2 {gamePlayers[1]}");

            AssignColor(dice);
            for (int i = 0; i < 2; i++)
            {
                gamePlayers[i].NoticeMatchingDone();
                //isPlayerWantToPlayAgain[i] = false;
            }
            //StartCoroutine(DisplayPlayersCardAndGameStart());
            DisplayPlayerCard();
        }
    }

    // プレイヤーに色を割り当てる
    public void AssignColor(int dice)
    {
        this.dice = dice;
        // TODO: ランダムでプレイヤーの色を決めるアニメーションを作る
        if (this.dice == 0)
        {
            gamePlayers[0].PlayerColor = (int)TurnManagerModel.TurnState.BLUE;
            gamePlayers[1].PlayerColor = (int)TurnManagerModel.TurnState.RED;
        }
        else
        {
            gamePlayers[0].PlayerColor = (int)TurnManagerModel.TurnState.RED;
            gamePlayers[1].PlayerColor = (int)TurnManagerModel.TurnState.BLUE;
        }
        myColor = gamePlayers[myPlayerIndex].PlayerColor;

        for (int i = 0; i < 2; i++)
        {
            gamePlayers[i].NoticeMatchingDone();
        }
    }

    // 赤色のプレイヤーはカメラの位置を対面にする
    public void SetCamera(int playerColor)
    {
        if (playerColor == (int)TurnManagerModel.TurnState.RED)
        {
            Vector3 cameraPosition = Camera.main.transform.position;
            Camera.main.transform.position = new Vector3(
                cameraPosition.x,
                cameraPosition.y,
                26.6f);
            Camera.main.transform.rotation = Quaternion.Euler(55, 180, 0);
        }
        else
        {
            Vector3 cameraPosition = Camera.main.transform.position;
            Camera.main.transform.position = new Vector3(
                cameraPosition.x,
                cameraPosition.y,
                -17f);
            Camera.main.transform.rotation = Quaternion.Euler(55, 0, 0);
        }
    }

    public void SetSimpei(int playerColor)
    {
        if (playerColor == (int)TurnManagerModel.TurnState.BLUE)
        {
            for (int i = 0; i < blueSimpeis.Length; i++)
            {
                blueSimpeis[i].transform.rotation = Quaternion.Euler(0, 0, 0);
                redSimpeis[i].transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
        else
        {
            for (int i = 0; i < blueSimpeis.Length; i++)
            {
                blueSimpeis[i].transform.rotation = Quaternion.Euler(0, 180, 0);
                redSimpeis[i].transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }

    public void StartGame()
    {
        uiController.Initialize();
        //AssignColor(0);
        tempSandwitchSimpeis.Clear();
        action = Action.HAVE;
        boardModel.Initialize();
        turnManagerModel.Initialize();
        effectController.Initialize();
        StopAllCoroutines();
        for (int i = 0; i < blueSimpeis.Length; i++)
        {
            blueSimpeiModels[i].Initialize();
            blueSimpeiMovers[i].Initialize();
            redSimpeiModels[i].Initialize();
            redSimpeiMovers[i].Initialize();
        }
        if (pointHolderManager == null)
            pointHolderManager = Instantiate(pointHolderManagerPrefab).GetComponent<PointHolderManager>();
        //pointHolderManager.ActiveUpperPoint();
        NextSimpei();
        if (IsMyTurn(myColor))
            DisplayStartPoint();
        DisplayMessageWindow();
    }

    int Dice()
    {
        dice = 1 - dice;
        return dice;
    }

    void DecidePlayerTurn()
    {
        gamePlayers[0].SendPlayerTurnDice(Dice());
    }

    public bool IsMyTurn(int playerColor)
    {
        return turnManagerModel.IsMyTurn(playerColor);
    }

    public bool TakeAction(int point)
    {
        bool isDone = false;
        targetPoint = point;
        Debug.Log($"GameController targetPoint is {targetPoint}");
        switch (action)
        {
            case Action.DONE:
                return false;
            case Action.HAVE:
                Debug.Log($"GamePlayer Action.HAVE");
                isDone = PutAction();
                break;
            case Action.SELECT:
                Debug.Log($"GamePlayer Action.SELECT");
                isDone = SelectAction();
                break;
            case Action.HAVE_ENEMY:
                Debug.Log($"GamePlayer Action.HAVE_ENEMY");
                isDone = PutEnemyAction();
                break;
            case Action.SELECT_ENEMY:
                Debug.Log($"GamePlayer Action.SELECT_ENEMY");
                isDone = SelectEnemyAction();
                break;
        }
        return isDone;
    }

    int NowTurnColor()
    {
        return (turnManagerModel.IsBlueTurn()) ? (int)TurnManagerModel.TurnState.BLUE : (int)TurnManagerModel.TurnState.RED;
    }

    bool PutAction() 
    {
        Debug.Log($"GameController PutAction point:{targetPoint}, oldPoint:{oldPoint}");
        int simpeiColor = activeSimpei.GetComponent<SimpeiModel>().simpeiColor;
        Vector3 target = boardModel.PutSimpei(targetPoint, oldPoint, turnManagerModel.NowTurnNum, simpeiColor);
        if (target.x == -100) return false;
        // コマを動かす
        activeSimpei.GetComponent<SimpeiMover>().Put(targetPoint);
        activeSimpei.GetComponent<SimpeiModel>().PutPoint = targetPoint;
        
        StartCoroutine(WaitForMoveAnimationFinish(target));
        if (turnManagerModel.NowTurnNum <= 8)
        {
            StartCoroutine(MoveWaitingLineAnimation());
        }
        DestroyStartPoint();
        if (turnManagerModel.NowTurnNum > 8)
        {
            if (targetPoint == oldPoint)
            {
                return true;
            }
        }
        // 勝利判定 return true; turnManagerModel.FinishGame()　TurnState.FINISH
        CheckWin();
        // 挟み込みの判定 SELECT_ENEMYステートへ以降
        CheckSandwitch();
        return true;
    }

    bool SelectAction()
    {
        // targetPointの値が-1なら失敗, 関数実行前と同じ値ならPickUp実行
        targetPoint = boardModel.PickUpSimpei(targetPoint, NowTurnColor());
        if (targetPoint == -1) return false;
        // 移動可能かどうかチェック、失敗ならfalseを返す
        if (!boardModel.IsCanMoveToNeighborhood(targetPoint)) return false;
        // 移動する前に置いていた場所を記録
        oldPoint = targetPoint;
        // どちらかのターンでアクティブ化するコマの決定
        if (turnManagerModel.IsBlueTurn())
        {
            foreach (SimpeiModel simpei in blueSimpeiModels)
            {
                Debug.Log(targetPoint == simpei.PutPoint);
                if (targetPoint == simpei.PutPoint)
                {
                    activeSimpei = simpei.gameObject;
                }
            }
        }
        else
        {
            foreach (SimpeiModel simpei in redSimpeiModels)
            {
                Debug.Log(targetPoint == simpei.PutPoint);
                if (targetPoint == simpei.PutPoint)
                {
                    activeSimpei = simpei.gameObject;
                }
            }
        }
        // 自分のターンかそうでないかでコマの動きを決定
        if (IsMyTurn(myColor))
        {
            activeSimpei.GetComponent<SimpeiMover>().PickUp();
            uiController.MoveToNeighborhood();
        }
        else
        {
            activeSimpei.GetComponent<SimpeiMover>().WaitToMove(otherHeight);
        }
        Debug.Log($"GameController SelectAction point:{targetPoint}, oldPoint:{oldPoint}");
        action = Action.HAVE;
        pointHolderManager.ActiveUpperPoint();
        if (IsMyTurn(myColor))
            DisplayAvailablePoint();
        return true;
    }

    void CheckSandwitch()
    {
        sandwitchSimpeis = boardModel.CheckSandwitchSimpei(targetPoint);
        if (sandwitchSimpeis.Count == 0) return ;
        tempSandwitchSimpeis = new List<int>(sandwitchSimpeis);
    }

    void SandwichSimpeiWaitToMove()
    {
        foreach (int point in sandwitchSimpeis)
        {
            for (int i = 0; i < 4; i++)
            {
                if (point == blueSimpeiModels[i].PutPoint)
                    blueSimpeiMovers[i].WaitToMove(otherHeight);
            }
            for (int i = 0; i < 4; i++)
            {
                if (point == redSimpeiModels[i].PutPoint)
                    redSimpeiMovers[i].WaitToMove(otherHeight);
            }
        }
    }

    void CheckWin()
    {
        var winnerTuple = boardModel.CheckThreeChain(targetPoint);
        if (winnerTuple.Item1 == 0) return;
        turnManagerModel.FinishGame();
        StartCoroutine(WinnerSimpeisAnimation(winnerTuple.Item1, winnerTuple.Item2));
    }

    int CheckThreeChain()
    {
        var chainerTuple = boardModel.CheckThreeChain(targetPoint);
        if (chainerTuple.Item1 == 0) return 0;
        return chainerTuple.Item1;
    }

    bool SelectEnemyAction()
    {
        Debug.Log("GameController SelectEnemyAction");
        if (!sandwitchSimpeis.Contains(targetPoint)) return false;
        Debug.Log($"GameController SelectEnemyAction {targetPoint}");
        if (turnManagerModel.IsBlueTurn())
        {
            for (int i = 0; i < 4; i++)
            {
                if (targetPoint == redSimpeiModels[i].PutPoint)
                {
                    activeSimpei = redSimpeiModels[i].gameObject;
                }
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                if (targetPoint == blueSimpeiModels[i].PutPoint)
                {
                    activeSimpei = blueSimpeiModels[i].gameObject;
                }
            }
        }
        if (IsMyTurn(myColor))
        {
            activeSimpei.GetComponent<SimpeiMover>().PickUp();
            uiController.MoveEnemySimpei();
        }
        boardModel.PickUpEnemySimpei(targetPoint);
        action = Action.HAVE_ENEMY;
        oldPoint = targetPoint;
        return true;
    }

    // 挟んだ敵のコマを動かすアクション
    // TODO: 敵の駒を3つ揃えた時は勝ち負けがつかないことをUIで通知する処理の作成
    bool PutEnemyAction()
    {
        if (sandwitchSimpeis.Contains(targetPoint)) return false;
        Vector3 target = boardModel.PutEnemySimpei(targetPoint, activeSimpei.GetComponent<SimpeiModel>().simpeiColor);
        activeSimpei.GetComponent<SimpeiMover>().Put(targetPoint);
        activeSimpei.GetComponent<SimpeiModel>().PutPoint = targetPoint;
        tempSandwitchSimpeis.Remove(oldPoint);
        oldPoint = -1;
        StartCoroutine(WaitForMoveAnimationFinish(target));
        if (CheckThreeChain() != 0)
        {
            // コルーチンの内容
            // ・アクションDONEの状態が変わるまで待ち続ける(アニメーションが終わるまで)
            // ・UIで勝ち負けがつかないテキストにUIを更新する
            StartCoroutine(NoticeDontDecideWinOrLose());
        }
        return true;
    }

    void NextTurn()
    {
        turnManagerModel.NextTurn();

        if (turnManagerModel.NowTurnNum >= 8)
        {
            if (!CheckCanMoveSimpei())
            {
                uiController.CannotMoveAnyPoint();
                NextTurn();
            }
            else
            {
                if (!IsMyTurn(myColor))
                {
                    uiController.NowIsEnemyTurn();
                }
                else
                {
                    uiController.SelectMySimpei();
                }
            }
        }
        else
        {
            if (!IsMyTurn(myColor))
            {
                uiController.NowIsEnemyTurn();
            }
            else
            {
                uiController.CanPutEveryPoint();
            }
        }
    }

    // 8ターン目以降,自分の駒を動かせるかチェック
    // 置けないならターンスキップ
    bool CheckCanMoveSimpei()
    {
        SimpeiModel[] simpeiModels = (turnManagerModel.IsBlueTurn()) ? blueSimpeiModels : redSimpeiModels;
        foreach (SimpeiModel simpei in simpeiModels)
        {
            if (boardModel.IsCanMoveToNeighborhood(simpei.PutPoint))
            {
                return true;
            }
        }
        return false;
    }

    void NextSimpei()
    {
        if (turnManagerModel.NowTurnNum <= 8)
        {
            if (turnManagerModel.IsBlueTurn())
            {
                activeSimpei = blueSimpeis[turnManagerModel.NowTurnNum / 2];
            }
            else
            {
                activeSimpei = redSimpeis[turnManagerModel.NowTurnNum / 2 - 1];
            }
            SimpeiMover mover = activeSimpei.GetComponent<SimpeiMover>();
            mover.WaitToMove(waitInWaitingLine);
            if (IsMyTurn(myColor))
            {
                mover.IsTargetOfMove = true;
            }
            // 相手側には少し浮かせて待機してるように見せる
            else 
            {
                mover.IsTargetOfMove = false;
            }
            action = Action.HAVE;
        }
        else
        {
            action = Action.SELECT;
            pointHolderManager.ActiveDownerPoint();
        }
        Debug.Log($"GameController NextSimpei is {activeSimpei}");
    }

    // 1ターン目のみスタート地点を表示する関数
    void DisplayStartPoint()
    {
        var startPointTuple = boardModel.GetStartPoints();
        for (int i = 0; i < startPointTuple.Item1.Length; i++)
        {
            float x = startPointTuple.Item2[i, 0];
            float z = startPointTuple.Item2[i, 1];
            Vector3 target = new Vector3(x, 0, z);
            effectController.GenerateAvailablePointEffect(startPointTuple.Item1[i], target);
        }
    }

    // 8ターン目以降に移動できるマスを表示する関数
    void DisplayAvailablePoint()
    {
        var availablePointTuple = boardModel.GetAvailablePoints(targetPoint);
        for (int i = 0; i < availablePointTuple.Item1.Length; i++)
        {
            if (availablePointTuple.Item2[i, 0] != -1)
            {
                float x = availablePointTuple.Item2[i, 0];
                float z = availablePointTuple.Item2[i, 1];
                Vector3 target = new Vector3(x, 0, z);
                effectController.GenerateAvailablePointEffect(availablePointTuple.Item1[i], target);
            }
        }
    }

    void DestroyStartPoint()
    {
        effectController.DestroyEffect();
    }

    // UIを更新する関数
    void DisplayWinUI(int winnerColor)
    {
        uiController.PlayerWin(winnerColor);
    }

    // 待機列のコマを動かすアニメーション
    IEnumerator MoveWaitingLineAnimation()
    {
        bool isMyTurn1 = IsMyTurn(gamePlayers[0].PlayerColor);
        bool isMyTurn2 = IsMyTurn(gamePlayers[1].PlayerColor);
        bool isBlueTurn = turnManagerModel.IsBlueTurn();
        for (int i = 0; i < gamePlayers.Length; i++)
        {
            bool isMyTurn = (i == 0) ? isMyTurn1 : isMyTurn2;
            SimpeiModel[] simpei = (isBlueTurn) ? blueSimpeiModels : redSimpeiModels;
            if (isMyTurn){
                if (isBlueTurn)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (blueSimpeiModels[j].PutPoint == -1)
                        {
                            yield return new WaitForSeconds(0.4f);
                            StartCoroutine(blueSimpeiMovers[j].MoveWaitingLine());
                        }
                    }

                }
                else
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (redSimpeiModels[j].PutPoint == -1)
                        {
                            yield return new WaitForSeconds(0.4f);
                            StartCoroutine(redSimpeiMovers[j].MoveWaitingLine());
                        }
                    }
                }
            }
        }
    }

    // アニメーションを実行して終了するまで待機するコルーチン
    IEnumerator WaitForMoveAnimationFinish(Vector3 target)
    {
        action = Action.DONE;
        bool isMyTurn = IsMyTurn(myColor);
        GamePlayer myPlayer = gamePlayers[myPlayerIndex].GetComponent<GamePlayer>();
        if (isMyTurn)
        {
            yield return StartCoroutine(activeSimpei.GetComponent<SimpeiMover>().MoveToTarget(target));
        }
        else
        {
            yield return StartCoroutine(activeSimpei.GetComponent<SimpeiMover>().MoveToTargetEnemy(target));
        }
        Debug.Log($"{tempSandwitchSimpeis.Count}");
        if (turnManagerModel.IsGameFinish()) {
            action = Action.HAVE;
            yield break;
        }
        while(!myPlayer.IsAllPlayerActionDone)
        {
            yield return null;
        }
        if (turnManagerModel.NowTurnNum > 8)
        {
            if (targetPoint == oldPoint)
            {
                action = Action.SELECT;
                uiController.SelectMySimpei();
                NextSimpei();
                yield break;
            }
        }
        if (tempSandwitchSimpeis.Count > 0)
        {
            action = Action.SELECT_ENEMY;
            SandwichSimpeiWaitToMove();
            if (isMyTurn)
            {
                uiController.SelectEnemySimpei();
            }
            yield break;
        }
        NextTurn();
        NextSimpei();
    }

    IEnumerator WinnerSimpeisAnimation(int simpeiColor, List<int> points)
    {
        while(action == Action.DONE)
        {
            yield return null;
        }
        GameObject[] winnerSimpeis;
        points.Sort();
        if (simpeiColor == 1) winnerSimpeis = (GameObject[])blueSimpeis.Clone();
        else winnerSimpeis = (GameObject[])redSimpeis.Clone();
        for (int i = 0; i < points.Count; i++)
        {
            foreach(GameObject go in winnerSimpeis)
            {
                if (points[i] == go.GetComponent<SimpeiModel>().PutPoint)
                {
                    yield return new WaitForSeconds(0.5f);
                    StartCoroutine(go.GetComponent<SimpeiMover>().WinnerAnimation(Time.time));
                    effectController.GenerateWinEffect(go.transform.position);
                }
            }
        }
        yield return new WaitForSeconds(0.3f);
        DisplayWinUI(simpeiColor);
        yield return new WaitForSeconds(3.0f);
        uiController.AskNextDoing();
    }

    IEnumerator NoticeDontDecideWinOrLose()
    {
        while(action == Action.DONE)
        {
            yield return null;
        }
        uiController.DontDecideWinOrLose();
    }

    // 対戦するプレイヤー同士のカードを見せるアニメーションを実行
    void DisplayPlayerCard()
    {
        StartCoroutine(uiController.MatchingWindowAnimation(gamePlayers[0]));
        StartCoroutine(uiController.MatchingWindowAnimation(gamePlayers[1]));
    }

    void DisplayMessageWindow()
    {
        uiController.SetMessageWindowColor(myColor);
        if (IsMyTurn(myColor))
        {
            uiController.FirstTurnMessage();
        }
        else
        {
            uiController.NowIsEnemyTurn();
        }
    }

    // アニメーションが終了した時にUIController側が呼び出す
    public void FinishDisplayPlayerCardAnimation()
    {
        StartGame();
    }

    // ホバーしたオブジェクトからの通知を処理する関数
    public void OnBoardPointMouseEnter(int point, Vector3 vector)
    {
        Debug.Log("GameController OnBoardPointEnter");
        if (action == Action.HAVE || action == Action.HAVE_ENEMY)
        {
            if (boardModel.ScoreArray[point] == 0)
                effectController.GenerateSelectEffect(point, vector);
        }
        else
        {
            if (boardModel.ScoreArray[point] != 0)
                effectController.GenerateSelectEffect(point, vector);
        }
    }

    public void OnBoardPointMouseExit()
    {
        effectController.DestroySelectEffect();
    }

    // プレイヤーからの再戦申請を受け取る
    public void RecievePlayAginFromUI()
    {
        gamePlayers[myPlayerIndex].SendPlayAgainToOpponent();
    }

    public void RecievePlayAgainRequest(GamePlayer player)
    {
        if (player.PlayerID == 1) isPlayerWantToPlayAgain[0] = true;
        else if (player.PlayerID == 2) isPlayerWantToPlayAgain[1] = true;
        if (isPlayerWantToPlayAgain[0] && isPlayerWantToPlayAgain[1])
        {
            isPlayerWantToPlayAgain[0] = false;
            isPlayerWantToPlayAgain[1] = false;
            Debug.Log("Recieve Play Again Request");

            DecidePlayerTurn();
        }
    }

    // プレイヤーから退出の通知を受け取る
    public void RecieveExitRoomFromUI()
    {
        gamePlayers[myPlayerIndex].SendExitToOpponent();
    }

    public void RecieveExitRoomRequest()
    {
        uiController.OpponentExit();
    }
    // private void OnGUI() {
    //     string label = $"Turn:{turnManagerModel.NowTurnNum} action:{action}";
    //     GUI.color = Color.white;
    //     GUI.Label(new Rect(0, 45, 500, 200), label);
    // }
}
