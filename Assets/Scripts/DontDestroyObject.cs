using UnityEngine;

public class DontDestroyObject : MonoBehaviour
{
    public static DontDestroyObject Instance
    {
        get; private set;
    }

    void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("Destroy GameObject");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
