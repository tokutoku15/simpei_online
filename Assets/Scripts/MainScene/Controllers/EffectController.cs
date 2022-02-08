using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    [Header("Effect Prefab")]
    [SerializeField] GameObject selectEffectPrefab;
    [SerializeField] GameObject winEffectPrefab;
    [SerializeField] GameObject startPointEffectPrefab;

    [Header("Parameters")]
    [SerializeField] private float upperEffectHeight;
    [SerializeField] private float downerEffectHeight;

    // Update is called once per frame
    GameObject selectEffect;
    int indexAvailable;
    GameObject[] availablePointEffects = new GameObject[4];
    int indexWinEffect;
    GameObject[] winPointEffects = new GameObject[3];

    void Start()
    {
        selectEffect = Instantiate(selectEffectPrefab, transform);
        selectEffect.SetActive(false);
        indexAvailable = 0;
        indexWinEffect = 0;

        for (int i = 0; i < 4; i++)
        {
            availablePointEffects[i] = Instantiate(startPointEffectPrefab, transform);
            availablePointEffects[i].SetActive(false);
        }
        for (int i = 0; i < 3; i++)
        {
            winPointEffects[i] = Instantiate(winEffectPrefab, transform);
            winPointEffects[i].SetActive(false);
        }
    }

    public void GenerateSelectEffect(int point, Vector3 target)
    {
        float y = (point < 16) ? upperEffectHeight : downerEffectHeight;
        selectEffect.transform.position = new Vector3(target.x, y, target.z);
        selectEffect.SetActive(true);
    }

    public void DestroySelectEffect()
    {
        selectEffect.SetActive(false);
    }

    public void GenerateWinEffect(Vector3 target)
    {
        winPointEffects[indexWinEffect].transform.position = new Vector3(target.x, upperEffectHeight + 0.2f, target.z);
        winPointEffects[indexWinEffect].SetActive(true);
        indexWinEffect++;
    }

    public void DestroyEffect()
    {
        for (int i = 0; i < 3; i++)
        {
            winPointEffects[i].SetActive(false);
        }
        for (int i = 0; i < 4; i++)
        {
            availablePointEffects[i].SetActive(false);
        }
        indexAvailable = 0;
        indexWinEffect = 0;
    }

    // 置けるマスだけ表示する
    public void GenerateAvailablePointEffect(int point, Vector3 target)
    {
        float effectHeight = (point < 16) ? upperEffectHeight : downerEffectHeight;
        availablePointEffects[indexAvailable].transform.position = new Vector3(target.x, effectHeight + 0.05f, target.z);
        availablePointEffects[indexAvailable].SetActive(true);
        indexAvailable++;
    }

    public void Initialize()
    {
        DestroySelectEffect();
        DestroyEffect();
    }
}
