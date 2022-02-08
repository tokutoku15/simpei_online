using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    // ローカルプレイヤーのニックネーム
    public static string PlayerName { get; set; }
    // リモートプレイヤーのニックネーム
    public static string RemotePlayerName { get; set; }
    // すでにアプリケーションを起動しているか
    public static bool IsStartedApp { get; set; }

    private void Start()
    {
        PlayerName = "名無し";
        IsStartedApp = false;
    }
}
