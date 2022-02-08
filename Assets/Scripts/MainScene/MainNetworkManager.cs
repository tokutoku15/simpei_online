using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class MainNetworkManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Instantiate("GamePlayer", Vector3.zero, Quaternion.identity);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Debug.Log("Test OnLeftRoom");
        SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Single);
    }
}
