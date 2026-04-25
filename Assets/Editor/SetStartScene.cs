#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

public static class SetStartScene
{
    [MenuItem("Tools/Set Start Scene to First Build Scene")]
    private static void Set()
    {
        string path = EditorBuildSettings.scenes[0].path;
        var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
        EditorSceneManager.playModeStartScene = scene;
    }

    [MenuItem("Tools/Clear Start Scene")]
    private static void Clear()
    {
        EditorSceneManager.playModeStartScene = null;
    }
}
#endif