using System.Collections;
using BloomJam.Player;
using UnityEngine;
using UnityEngine.UI;
using YigitcanCaliskan.EventBus;

public class DamageEffect : MonoBehaviour
{
    [SerializeField] private Image vignetteImage;
    [SerializeField] private float flashAlpha   = 0.5f;
    [SerializeField] private float fadeOutTime  = 0.6f;

    private Coroutine _fadeRoutine;

    private void OnEnable()  => EventBus.Subscribe<PlayerDamagedEvent>(OnDamaged);
    private void OnDisable() => EventBus.Unsubscribe<PlayerDamagedEvent>(OnDamaged);

    private void OnDamaged(PlayerDamagedEvent e)
    {
        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
        _fadeRoutine = StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        SetAlpha(flashAlpha);

        for (float t = 0f; t < fadeOutTime; t += Time.unscaledDeltaTime)
        {
            SetAlpha(Mathf.Lerp(flashAlpha, 0f, t / fadeOutTime));
            yield return null;
        }

        SetAlpha(0f);
    }

    private void SetAlpha(float a)
    {
        var c = vignetteImage.color;
        c.a = a;
        vignetteImage.color = c;
    }
}