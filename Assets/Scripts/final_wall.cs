using System.Collections;
using UnityEngine;

public class final_wall : MonoBehaviour
{
    public GameObject enemi;

    [SerializeField] private float slideDistance = 10f;
    [SerializeField] private float slideDuration = 1.2f;
    [SerializeField] private Material slideMaterial;

    private bool _sliding;

    void Update()
    {
        if (!_sliding && enemi.transform.childCount == 0)
        {
            _sliding = true;
            if (slideMaterial != null)
                GetComponent<Renderer>().material = slideMaterial;
            StartCoroutine(SlideOut());
        }
    }

    private IEnumerator SlideOut()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos   = startPos + Vector3.forward * slideDistance;
        float prevEase   = 0f;

        for (float t = 0f; t < slideDuration; t += Time.deltaTime)
        {
            float ease  = Mathf.SmoothStep(0f, 1f, t / slideDuration);
            float delta = ease - prevEase;
            transform.position = Vector3.Lerp(startPos, endPos, ease);
            transform.Rotate(0f, 0f, 360f * delta, Space.Self);
            prevEase = ease;
            yield return null;
        }

        Destroy(gameObject);
    }
}