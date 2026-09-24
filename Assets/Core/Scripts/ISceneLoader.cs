namespace Rabisco.Core
{
    /// <summary>
    /// Scene loader service. Wraps SceneManager.LoadSceneAsync with fade in/out.
    /// Hides loading screens — critical for WebGL where loading can take time.
    /// </summary>
    public interface ISceneLoader : IGameService
    {
        /// <summary>
        /// Loads a scene by name with a fade out → load → fade in sequence.
        /// </summary>
        /// <param name="sceneName">Name of the scene to load.</param>
        void LoadScene(string sceneName);
    }
}
