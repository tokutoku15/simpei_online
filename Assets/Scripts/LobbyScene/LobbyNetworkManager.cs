using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class LobbyNetworkManager : MonoBehaviourPunCallbacks
{
    [Header("DefaultRoomSettings")]

    // 最大人数
    [SerializeField] private int maxPlayers = 2;
    // 公開・非公開
    [SerializeField] private bool isVisible = true;
    // (ゲームの途中含む)入室の可否
    [SerializeField] private bool isOpen = true;
    // 部屋名
    [SerializeField] private string roomName = "room";

    [Header("TitleManager")]
    [SerializeField] TitleManager titleManager;

    bool IsJoinedLobby;
    bool IsStartMatching;

    private void Awake()
    {
        // シーン遷移してもこのオブジェクトを破棄しないようにする
        //DontDestroyOnLoad(this.gameObject);
        // シーンの自動同期を無効にする
        PhotonNetwork.AutomaticallySyncScene = false;
        IsJoinedLobby = false;
        IsStartMatching = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Test connect");
        //　マスターサーバに接続
        ConnectToPhoton("1.0");
    }

    private void ConnectToPhoton(string gameVersion)
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // ニックネームを設定する
    private void SetMyNickName(string nickName)
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LocalPlayer.NickName = nickName;
        }
    }

    // ロビーに参加する
    private void JoinLobby()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    // ルームを作成する
    private void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers  = (byte)maxPlayers,
            IsOpen      = isOpen,
            IsVisible   = isVisible
        };
        PhotonNetwork.CreateRoom(null, roomOptions, null);
    }

    // ボタンで呼び出す関数
    public void StartMatch()
    {
        if (!IsJoinedLobby) return;
        if (IsStartMatching) return;
        IsStartMatching = true;
        titleManager.StartMatching();
        SetMyNickName(GameDataManager.PlayerName);
        PhotonNetwork.JoinRandomRoom();
    }

    // PhotonNetworkのコールバック
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("LobbyNetworkManager OnConnectedToMaster");
        JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("LobbyNetworkManager OnJoinedLobby");
        IsJoinedLobby = true;
        titleManager.DisplayTitleObjects();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("LobbyNetworkManager OnJoinedRoom");
        PhotonNetwork.Instantiate("TempPlayer", Vector3.zero, Quaternion.identity);

        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            // ルーム入室のタイミングによらず相手と同期が取れるようにする
            PhotonNetwork.IsMessageQueueRunning = false;
            // ルームに入室したらMainシーンに遷移する
            SceneManager.LoadSceneAsync("Main", LoadSceneMode.Single);
        }
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("LobbyNetworkManager OnCreatedRoom");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("LobbyNetworkManager OnJoinRandomFailed");
        CreateRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("LobbyNetworkManager OnPlayerEnteredRoom");
        PhotonNetwork.IsMessageQueueRunning = false;
        SceneManager.LoadSceneAsync("Main", LoadSceneMode.Single);
    }
}
