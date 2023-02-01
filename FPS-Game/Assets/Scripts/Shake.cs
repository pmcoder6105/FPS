using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    public bool start = false;
    public float duration = 1f;
    public AnimationCurve curve;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            start = false;
            StopCoroutine("Shaking");
            StartCoroutine("Shaking");
        }
    }

    IEnumerator Shaking()
    {
        Vector2 startPosition = new Vector2(transform.position.x, transform.position.y);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strenght = curve.Evaluate(elapsedTime / duration);
            transform.position = startPosition + Random.insideUnitCircle * strenght;
            //transform.position = startPosition + Random.insideUnitCircle * strenght;
            yield return null;
        }
        transform.position = startPosition;
    }
}
