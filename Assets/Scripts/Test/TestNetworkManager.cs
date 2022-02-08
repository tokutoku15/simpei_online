using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TestNetworkManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Test connect");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Test OnConnectedToMaster");

        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("Test OnJoinedRandomFailed");
        RoomOptions ro = new RoomOptions
        {
            MaxPlayers = 2
        };
        PhotonNetwork.CreateRoom(null, ro, null);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Test OnJoinedRoom");
        PhotonNetwork.Instantiate("GamePlayer", Vector3.zero, Quaternion.identity);
    }
}