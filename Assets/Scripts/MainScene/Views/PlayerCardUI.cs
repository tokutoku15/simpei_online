using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCardUI : MonoBehaviour
{
    [Header("TMPro")]
    [SerializeField] TextMeshProUGUI playerNumText;
    [SerializeField] TextMeshProUGUI playerNameText;

    [Header("Properties")]
    [SerializeField] bool IsMe;
    [SerializeField] int playerColor;
    [SerializeField] int playerId;
    [SerializeField] string playerName;

    RectTransform rect;
    Image image;

    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        //StartCoroutine(MatchingAnimation(IsMe));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetPlayerIDAndName(int playerId, string playerName)
    {
        playerNumText.text = $"プレイヤー{playerId}";
        playerNameText.text = playerName;
    }

    public IEnumerator MatchingAnimation(int playerId, int playerColor, string name)
    {
        // debug
        SetPlayerIDAndName (playerId, name);
        // debug
        Debug.Log("TextWindowUI MatchingAnimation");
        Vector2 targetPos;
        if (playerColor == 1) image.color = new Color(180f / 255f, 180f / 255f, 255f / 255f);
        else if (playerColor == -1) image.color = new Color(255f / 255f, 180f / 255f, 180f / 255f);
        if (IsMe)
        {
            targetPos = new Vector2(0, -90f);
            rect.anchoredPosition = new Vector2(-430f, -90f);
        }
        else
        {
            targetPos = new Vector2(0, 90f);
            rect.anchoredPosition = new Vector2(430f, 90f);
        }
        Vector2 dir = targetPos - rect.anchoredPosition;
        int n = 45, sum = n * (n + 1) / 2;
        for (int i = n; i > 0; i--)
        {
            float dx = i * dir.x / sum;
            Vector3 nowPos = rect.anchoredPosition;
            rect.anchoredPosition = new Vector2(nowPos.x + dx, nowPos.y);
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

        if (IsMe)
        {
            targetPos = new Vector2(430f, -90f);
        }
        else
        {
            targetPos = new Vector2(-430f, 90f);
        }
        dir = targetPos - rect.anchoredPosition;
        for (int i = 0; i < n; i++)
        {
            float dx = i * dir.x / sum;
            Vector3 nowPos = rect.anchoredPosition;
            rect.anchoredPosition = new Vector2(nowPos.x + dx, nowPos.y);
            yield return null;
        }

        this.gameObject.SetActive(false);
    }
}
