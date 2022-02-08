using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GamePlayer : MonoBehaviourPunCallbacks
{
    // 自分が操作するプレイヤー
    bool _isMe;
    // プレイヤーID
    int _playerID;
    // プレイヤーの色 青:1/赤:-1
    int _playerColor;
    // 選んだマス番号
    int selectPoint;
    bool isAllPlayerActionDone;
    public bool IsAllPlayerActionDone { get { return isAllPlayerActionDone; } }
    new PhotonView photonView;
    GameController gameController;

    // 外部公開用変数
    public int PlayerID { get { return _playerID; } }
    public bool IsMe { get { return _isMe; } }
    public int PlayerColor { set { _playerColor = value; } get { return _playerColor; } }

    void Awake()
    {
        // ルーム入室のタイミングによらず相手と同期が取れるようにする
        PhotonNetwork.IsMessageQueueRunning = true;
        // PhotonViewコンポーネントの取得
        photonView = this.GetComponent<PhotonView>();
        _playerID = photonView.OwnerActorNr;
        _isMe = photonView.IsMine;
        // GameControllerコンポーネントを取得
        gameController = FindObjectOfType<GameController>();
        // gameControllerにこのコンポーネントを登録
        gameController.RegisterPlayer(this);
    }

    void Start()
    {
        if (!photonView.IsMine) return;
        Debug.Log("MyNickName is " + MyNickName());
        isAllPlayerActionDone = true;
        Debug.Log($"GamePlayer You are {_playerColor}");
        Debug.Log($"PlayerCount:{PhotonNetwork.CurrentRoom.PlayerCount}, MaxPlayers{PhotonNetwork.CurrentRoom.MaxPlayers}");
    }

    public string MyNickName()
    {
        if (photonView.IsMine)
            return PhotonNetwork.LocalPlayer.NickName;
        else
            return PhotonNetwork.PlayerListOthers[0].NickName;
    }

    public bool SelectPoint(int point)
    {
        bool isDone = false;
        Debug.Log($"GamePlayer SelectPoint {point}");
        selectPoint = point;
        if (!isAllPlayerActionDone) return isDone;
        // アクション実行(成功した場合RPCで同期する)
        isDone = gameController.TakeAction(selectPoint);
        if (isDone == true)
        {
            isAllPlayerActionDone = false;
            photonView.RPC(nameof(RPCSendPoint), RpcTarget.Others, selectPoint, gameController.NowAction);
        }
        return isDone;
    }

    // 再戦することをプレイヤーに通知する
    public void SendPlayAgainToOpponent()
    {
        photonView.RPC(nameof(SendPlayAginRequest), RpcTarget.AllViaServer);
    }

    // 退出することをプレイヤーに通知する
    public void SendExitToOpponent()
    {
        photonView.RPC(nameof(SendExitRequest), RpcTarget.AllViaServer);
    }

    void ExitRoom()
    {
        if(photonView.IsMine)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    // プレイヤーのターン決めのサイコロ(0 or 1)をプレイヤー1が通知する
    public void SendPlayerTurnDice(int dice)
    {
        if (photonView.IsMine && PlayerID == 1)
        {
            photonView.RPC(nameof(SendDiceAllViaServer), RpcTarget.AllViaServer, dice);
        }
    }

    public void NoticeMatchingDone()
    {
        if (PhotonNetwork.CurrentRoom.IsOpen)
        {
            // マッチングが終わった時点で部屋を非公開かつ途中から参加できないようにする
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
        SetCameraAndSimpeis();
    }


    public bool IsMyTurn()
    {
        return gameController.IsMyTurn(_playerColor);
    }

    // 赤色のプレイヤーはカメラの位置を対面にする
    void SetCameraAndSimpeis()
    {
        if (photonView.IsMine)
        {
            gameController.SetCamera(_playerColor);
            gameController.SetSimpei(_playerColor);
        }
    }

    // RPCで同期させる関数
    [PunRPC]
    void RPCSendPoint(int point, GameController.Action action)
    {
        selectPoint = point;
        gameController.TakeAction(selectPoint);
        StartCoroutine(WaitForEnemyActionDone());
    }

    // プレイヤーの行動が終わったことを通知する
    [PunRPC]
    void SendAllPlayerActionDone()
    {
        isAllPlayerActionDone = true;
    }

    [PunRPC]
    void SendPlayAginRequest()
    {
        gameController.RecievePlayAgainRequest(this);
    }

    [PunRPC]
    void SendExitRequest()
    {
        gameController.RecieveExitRoomRequest();
        if (photonView.IsMine)
        {
            ExitRoom();
        }
    }

    [PunRPC]
    void SendDiceAllViaServer(int dice)
    {
        //gameController.AssignColor(dice);
        Debug.Log(dice);
        gameController.AssignColor(dice);
        gameController.StartGame();
    }

    // private void OnGUI() 
    // {
    //     string label = $"GamePlayer({_playerID}) select {selectPoint}, done {isAllPlayerActionDone}";
    //     GUI.color = Color.white; 
    //     GUI.Label(new Rect(0, (_playerID-1)*15, 400, 100), label);
    // }

    IEnumerator WaitForEnemyActionDone()
    {
        while(gameController.NowAction == GameController.Action.DONE)
        {
            yield return null;
        }
        photonView.RPC(nameof(SendAllPlayerActionDone), RpcTarget.Others);
    }
}