using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FluctuatingOpacity : MonoBehaviour
{
    [SerializeField] private float secondsInCycle;
    [SerializeField] private float minOpacity;
    [SerializeField] private float maxOpacity;

    private Tilemap renderer;

    private float opacityFactor;
    
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Tilemap>();
        StartCoroutine(FadeIn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator FadeIn()
    {
        opacityFactor = 0;
        Color c = renderer.color;
        while (opacityFactor < 1)
        {
            opacityFactor += Time.deltaTime * 2f / secondsInCycle;
            c.a = minOpacity + (maxOpacity - minOpacity) * Mathf.Sin(opacityFactor * Mathf.PI);
            renderer.color = c;
            yield return null;
        }
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        opacityFactor = 1;
        Color c = renderer.color;
        while (opacityFactor > 0)
        {
            opacityFactor -= Time.deltaTime * 2f / secondsInCycle;
            c.a = minOpacity + (maxOpacity - minOpacity) * Mathf.Sin(opacityFactor * Mathf.PI);
            renderer.color = c;
            yield return null;
        }
        StartCoroutine(FadeIn());
    }
}
