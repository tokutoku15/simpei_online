using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSimpeiView : MonoBehaviour
{
    [SerializeField] float x;
    [SerializeField] float y;
    [SerializeField] float z;
    [SerializeField] float rotX;
    [SerializeField] float rotY;
    [SerializeField] float rotZ;
    // Start is called before the first frame update

    private void Start()
    {
        transform.position = new Vector3(x, y, z);
        transform.rotation = Quaternion.Euler(rotX, rotY, rotZ);
    }

    void OnEnable()
    {
        transform.position = new Vector3(x, y, z);
        transform.rotation = Quaternion.Euler(rotX, rotY, rotZ);
        StopAllCoroutines();
        StartCoroutine(Popping());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Popping()
    {
        float nowPosY = transform.position.y;
        float diffY = 0.2f;
        float dy = 0;
        int n = 6; int sum = n * (n + 1) / 2;
        while (true)
        {
            for (int i = n; i > 0; i--)
            {
                dy = i * diffY / sum;
                Vector3 nowPos = transform.position;
                transform.position = new Vector3(nowPos.x, nowPos.y + dy, nowPos.z);
                yield return new WaitForSeconds(0.01f);
            }
            for (int i = 1; i <= n; i++)
            {
                dy = - i * diffY / sum;
                Vector3 nowPos = transform.position;
                transform.position = new Vector3(nowPos.x, nowPos.y + dy, nowPos.z);
                yield return new WaitForSeconds(0.01f);
            }
            yield return new WaitForSeconds(1.5f);
        }
    }
}
