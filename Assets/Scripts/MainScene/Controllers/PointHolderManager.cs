using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointHolderManager : MonoBehaviour
{
    [Header("PointHolderObjects")]
    [SerializeField] GameObject upperPointHolderPrefab;
    [SerializeField] GameObject downerPointHolderPrefab;

    GameObject upperPointHolder;
    GameObject downerPointHolder;

    private void Start()
    {
        upperPointHolder = Instantiate(upperPointHolderPrefab, Vector3.zero, Quaternion.identity, transform);
        downerPointHolder = Instantiate(downerPointHolderPrefab, Vector3.zero, Quaternion.identity, transform);
        ActiveUpperPoint();
    }

    public void ActiveUpperPoint()
    {
        upperPointHolder.SetActive(true);
        downerPointHolder.SetActive(false);
    }

    public void ActiveDownerPoint()
    {
        upperPointHolder.SetActive(false);
        downerPointHolder.SetActive(true);
    }

    public void InactiveBothPoint()
    {
        upperPointHolder.SetActive(false);
        downerPointHolder.SetActive(false);
    }
}
