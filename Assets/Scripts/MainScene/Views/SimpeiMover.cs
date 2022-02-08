using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SimpeiMover : MonoBehaviour, IPointerEnterHandler
{
    [Header("Initial Position")]
    [SerializeField] private float initialX;
    [SerializeField] private float initialY;
    [SerializeField] private float initialZ;
    [SerializeField] private float waitToMoveHeight = 4f;
    Vector3 screenPoint;
    new CapsuleCollider collider;
    GameController gameController;
    AudioSource putSound;
    float angle;

    public bool IsHover { get; set; }
    public bool IsTargetOfMove { get; set; }
    public bool IsAnimationFinish { get; set; }
    // Start is called before the first frame update
    void Awake()
    {
        putSound = GetComponent<AudioSource>();
    }

    void Start()
    {
        IsAnimationFinish = true;
        IsTargetOfMove = false;
        angle = 0;
        gameController = FindObjectOfType<GameController>();
        collider = GetComponent<CapsuleCollider>();
        EnableCollider();
    }

    public void Initialize()
    {
        IsAnimationFinish = true;
        IsTargetOfMove = false;
        IsHover = false;
        angle = 0;
        if (angle == 180f)
            transform.rotation *= Quaternion.Euler(0, 0, 180);
        transform.position = new Vector3(initialX, initialY, initialZ);
        EnableCollider();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsTargetOfMove) return;
        if (!IsHover) return;
        screenPoint = Input.mousePosition; screenPoint.z = 10.0f;
        Ray ray = Camera.main.ScreenPointToRay(screenPoint);
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit))
        {
            transform.position = hit.point;
        }
    }

    public void EnableCollider()
    {
        collider.enabled = true;
    }

    public void DisableCollider()
    {
        collider.enabled = false;
    }

    public void HoverExit()
    {
        IsHover = false;
    }

    public void HoverEnter()
    {
        IsHover = true;
    }

    public void OnPointerEnter(PointerEventData data)
    {
        Debug.Log($"SimpeiMover IsHover {IsHover}, IsTargetOfMove {IsTargetOfMove}, PutPoint {GetComponent<SimpeiModel>().PutPoint}");
        HoverEnter();
        if (IsTargetOfMove)
            DisableCollider();
    }

    public void Put(int point)
    {
        IsTargetOfMove = false;
        HoverExit();
        DisableCollider();
        Debug.Log(transform.localEulerAngles.z);
        if (point < 16)
        {
            //DefaultAngle();
            if (angle == 180f)
            {
                StartCoroutine(UpsideDownAnimation());
                angle = 0f;
            }
        }
        else
        {
            if (angle == 0f)
            {
                StartCoroutine(UpsideDownAnimation());
                angle = 180f;
            }
        }
        // StartCoroutine(MoveToTarget(target));
    }

    public void PickUp()
    {
        IsTargetOfMove = true;
        IsHover = true;
    }

    IEnumerator UpsideDownAnimation()
    {
        int n = 10;
        Quaternion q = Quaternion.Euler(0, 0, 180 / n);
        for(int i = 0; i < n; i++)
        {
            yield return new WaitForSeconds(0.03f);
            transform.rotation = q * transform.rotation;
        }
    }

    public void WaitToMove(float height)
    {
        if (height == -100) height = waitToMoveHeight;
        transform.position = new Vector3(transform.position.x, height, transform.position.z);
    }

    public IEnumerator MoveToTarget(Vector3 target)
    {
        float waitSeconds = 0.03f;
        Debug.Log("MoveToTarget");
        IsAnimationFinish = false;
        int n = 20; int sum = n * (n + 1) / 2;
        Vector3 direciton = target - transform.position;
        for (int i = n; i > 0; i--) 
        {
            float dx = i * direciton.x / sum;
            float dy = i * direciton.y / sum;
            float dz = i * direciton.z / sum;
            Vector3 nowPos = transform.position;
            transform.position = new Vector3(nowPos.x + dx, nowPos.y + dy, nowPos.z + dz);
            yield return new WaitForSeconds(waitSeconds);
        }
        putSound.Play();
        IsAnimationFinish = true;
    }

    public IEnumerator MoveToTargetEnemy(Vector3 target)
    {
        float waitSeconds = 0.02f;
        Debug.Log("MoveToTargetEnemy");
        IsAnimationFinish = false;
        int n = 20; int sum = n * (n + 1) / 2;
        float diffHeight = waitToMoveHeight - transform.position.y;
        if (diffHeight != 0)
        {
            for (int i = n / 2; i > 0; i--)
            {
                float dy = i * diffHeight / (sum / 2);
                Vector3 nowPos = transform.position;
                transform.position = new Vector3(nowPos.x, nowPos.y + dy, nowPos.z);
                yield return new WaitForSeconds(waitSeconds);
            }
        }
        Vector3 direciton = target - transform.position;
        for (int i = n; i > 0; i--) 
        {
            float dx = i * direciton.x / sum;
            float dz = i * direciton.z / sum;
            Vector3 nowPos = transform.position;
            transform.position = new Vector3(nowPos.x + dx, nowPos.y, nowPos.z + dz);
            yield return new WaitForSeconds(waitSeconds);
        }
        for (int i = n; i > 0; i--) 
        {
            float dy = i * direciton.y / sum;
            Vector3 nowPos = transform.position;
            transform.position = new Vector3(nowPos.x, nowPos.y + dy, nowPos.z);
            yield return new WaitForSeconds(waitSeconds);
        }
        putSound.Play();
        IsAnimationFinish = true;
    }

    public IEnumerator MoveWaitingLine()
    {
        float waitSeconds = 0.04f;
        float x;
        if (GetComponent<SimpeiModel>().simpeiColor == 1) x = 3.2f;
        else x = -3.2f;
        int n = 16; int sum = n * (n + 1) / 2;
        for (int i = n; i > 0; i--) 
        {
            float dx = i * x / sum;
            Vector3 nowPos = transform.position;
            transform.position = new Vector3(nowPos.x + dx, nowPos.y, nowPos.z);
            yield return new WaitForSeconds(waitSeconds);
        }
    }

    public IEnumerator WinnerAnimation(float t)
    {
        float widthY = 1.25f;
        float waitSeconds = 0.01f;
        Vector3 nowPos = transform.position;
        while (true)
        {
            float dy = widthY * Mathf.Sin((Time.time) * 3.0f + t);
            if (dy < 0) dy *= -1;
            transform.position = new Vector3(nowPos.x, nowPos.y + dy, nowPos.z);
            yield return new WaitForSeconds(waitSeconds);
        }
    }

    //private void OnGUI()
    //{
    //    string label = $"{transform.rotation}";
    //    GUI.color = Color.white;
    //    GUI.Label(new Rect(0, 100, 100, 100), label);
    //}
}
