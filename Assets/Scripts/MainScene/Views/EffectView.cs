using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectView : MonoBehaviour
{
    [SerializeField] bool isBlinking;
    new Renderer renderer;
    Color _color;
    AudioSource winSound;
    // Start is called before the first frame update
    void Awake()
    {
        if (gameObject.CompareTag("WinEffect"))
            winSound = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        Debug.Log("EffectView OnEnable");
        if (gameObject.CompareTag("WinEffect"))
            winSound.Play();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 90, 0) * Time.deltaTime, Space.World);
    }

    IEnumerator Blinking(){
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            _color.a = 1.0f - _color.a;
            renderer.sharedMaterial.color = _color;
        }
    }
}
