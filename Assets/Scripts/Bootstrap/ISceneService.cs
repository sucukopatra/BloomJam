namespace BloomJam
{
    public interface ISceneService
    {
        void LoadScene(int buildIndex);
        void ReloadCurrent();
        void LoadMainMenu();
        void LoadGameplay();
    }
}