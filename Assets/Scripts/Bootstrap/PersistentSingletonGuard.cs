using UnityEngine;
using UnityEngine.EventSystems;

namespace BloomJam
{
    /// <summary>
    /// Attach to EventSystem in every scene.
    /// Destroys itself if another EventSystem already exists (from Bootstrap DontDestroyOnLoad).
    /// </summary>
    public class PersistentSingletonGuard : MonoBehaviour
    {
        private void Awake()
        {
#if UNITY_2022_2_OR_NEWER
            var all = FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
            var all = FindObjectsOfType<EventSystem>(true);
#endif
            if (all.Length > 1)
                Destroy(gameObject);
        }
    }
}