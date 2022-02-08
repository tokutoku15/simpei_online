using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageWindowUI : MonoBehaviour
{
    [Header("Simepi")]
    [SerializeField] GameObject messageWindowPanel;
    [SerializeField] GameObject simpeiBlue;
    [SerializeField] GameObject simpeiRed;
    [SerializeField] TextMeshProUGUI messageText;

    Image image;

    void Awake()
    {
        image = messageWindowPanel.GetComponent<Image>();
        Debug.Log(image);
    }

    public void SetMessageWindowColor(int playerColor)
    {
        if (playerColor == 1)
        {
            simpeiBlue.SetActive(true);
            simpeiRed.SetActive(false);
            image.color = new Color(180f / 255f, 180f / 255f, 255f / 255f);
        }
        else if (playerColor == -1)
        {
            simpeiBlue.SetActive(false);
            simpeiRed.SetActive(true);
            image.color = new Color(255f / 255f, 180f / 255f, 180f / 255f);
        }
    }

    public void SetText(string textString)
    {
        messageText.text = textString;
    }
}
