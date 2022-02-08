using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerNamePanel : MonoBehaviour
{
    [Header("GameObjects")]
    // プレイヤーネームのInputField
    [SerializeField] private TMP_InputField inputField;

    private TextMeshProUGUI playerNameText;
    private string defaultPlayerName = "名無し";
    // Start is called before the first frame update
    void Start()
    {
        playerNameText = GameObject.FindGameObjectWithTag("PlayerName")
                                   .GetComponent<TextMeshProUGUI>();
        if (GameDataManager.PlayerName == "")
        {
            playerNameText.text = defaultPlayerName;
        } else
        {
            playerNameText.text = GameDataManager.PlayerName;
        }
    }

    public void SetPlayerNameText()
    {
        if (inputField.text == "")
        {
            playerNameText.text = defaultPlayerName;
            GameDataManager.PlayerName = defaultPlayerName;
        } else
        {
            playerNameText.text = inputField.text;
            GameDataManager.PlayerName = inputField.text;
        }
    }

    public void OnValueChanged()
    {
        string value = inputField.text;
        if (value.IndexOf("\n") != -1)
        {
            inputField.text = value;
        }
    }
}
