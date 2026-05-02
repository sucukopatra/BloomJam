using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using YigitcanCaliskan;
using YigitcanCaliskan.ServiceLocator;

namespace BloomJam
{
    public class SceneLoader : MonoBehaviour, IBootstrapService, ISceneService
    {
        public static SceneLoader Instance { get; private set; }

        [SerializeField] private CanvasGroup fadePanel;
        [SerializeField] private float fadeDuration = 0.35f;
        [SerializeField] private int gamePlayIndex  = 2;
        [SerializeField] private int mainMenuIndex  = 1;
        [SerializeField] private int bootstrapIndex  = 0;

        private bool _isLoading;

        private void Awake() => Instance = this;

        public void Register() => ServiceLocator.Register<ISceneService>(this);

        public void LoadScene(int buildIndex)
        {
            if (_isLoading) return;
            StartCoroutine(LoadRoutine(buildIndex));
        }

        public void ReloadCurrent() =>
            LoadScene(SceneManager.GetActiveScene().buildIndex);

        public void LoadMainMenu() => LoadScene(mainMenuIndex);
        public void LoadGameplay()
        {
            LoadScene(gamePlayIndex);
            
        }

        public void LoadBootstrapMenu() => LoadScene(mainMenuIndex);

        private IEnumerator LoadRoutine(int index)
        {
            _isLoading = true;

            yield return StartCoroutine(Fade(1f));

            var op = SceneManager.LoadSceneAsync(index);
            op.allowSceneActivation = false;

            while (op.progress < 0.9f)
                yield return null;

            op.allowSceneActivation = true;

            yield return StartCoroutine(Fade(0f));

            _isLoading = false;
        }

        private IEnumerator Fade(float target)
        {
            if (fadePanel == null)
            {
                Debug.LogError("[SceneLoader] fadePanel atanmamış! Inspector'dan CanvasGroup sürükle.");
                yield break;
            }

            float start = fadePanel.alpha;
            for (float t = 0f; t < fadeDuration; t += Time.unscaledDeltaTime)
            {
                fadePanel.alpha = Mathf.Lerp(start, target, t / fadeDuration);
                yield return null;
            }
            fadePanel.alpha = target;
            fadePanel.blocksRaycasts = target > 0f;
        }
    }
}