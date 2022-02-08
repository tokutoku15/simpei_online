using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    [Header("Title Objects")]
    [SerializeField] GameObject simpeiBlue;
    [SerializeField] GameObject simpeiRed;
    [SerializeField] GameObject titleTextObject;
    [SerializeField] GameObject playerNamePanel;
    [SerializeField] GameObject enterRoomPanel;
    [SerializeField] GameObject waitConnectionPanel;
    [SerializeField] GameObject waitMatchingPanel;

    private void Start()
    {
        SetTitleObjects(false);
        waitConnectionPanel.SetActive(true);
        waitMatchingPanel.SetActive(false);
    }

    public void DisplayTitleObjects()
    {
        waitConnectionPanel.SetActive(false);
        StartCoroutine(DisplayTitleAnimation());
    }

    public void StartMatching()
    {
        waitMatchingPanel.SetActive(true);
    }

    void SetTitleObjects(bool isActive)
    {
        
        simpeiBlue.SetActive(isActive);
        simpeiRed.SetActive(isActive);
        titleTextObject.SetActive(isActive);
        playerNamePanel.SetActive(isActive);
        enterRoomPanel.SetActive(isActive);
    }

    IEnumerator DisplayTitleAnimation()
    {
        yield return new WaitForSeconds(1f);
        SetTitleObjects(true);
    }
}
