using UnityEngine;

namespace Rabisco.Core
{
    /// <summary>
    /// Bootstrapper. Runs before any scene loads and initializes all services.
    /// Uses [RuntimeInitializeOnLoadMethod] to execute automatically.
    /// No GameObject needed, everything is created in code.
    /// </summary>
    public static class Bootstraper
    {
        #region INITIALIZATION

        /// <summary>
        /// Called automatically by Unity before any scene loads.
        /// Creates all global managers and registers them in ServiceLocator.
        /// ServiceLocator is a static class — no initialization needed, always available.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            // --- Create and register services ---
            // Each manager is created with 'new' and registered via ServiceLocator.Register<T>().
            // Placeholders for now — uncomment as each manager is implemented.

            var gameManager = new GameManager();
            ServiceLocator.Register<IGameStateService>(gameManager);

            var sceneLoader = new SceneLoader();
            ServiceLocator.Register<ISceneLoader>(sceneLoader);

            // --- Save System (Script #5) ---
            // var saveManager = new SaveManager();
            // ServiceLocator.Register<ISaveService>(saveManager);

            // --- Input System (Script #6) ---
            // var inputManager = new InputManager();
            // ServiceLocator.Register<IInputService>(inputManager);

            // --- Audio System (Script #7) ---
            // var audioManager = new AudioManager();
            // ServiceLocator.Register<IAudioService>(audioManager);

            // --- Camera System (Script #8) ---
            // var cameraManager = new CameraManager();
            // ServiceLocator.Register<ICameraService>(cameraManager);

            // --- Cutscene System (Script #9) ---
            // var cutsceneManager = new CutsceneManager();
            // ServiceLocator.Register<ICutsceneService>(cutsceneManager);

            // --- Dialogue System (Script #10) ---
            // var dialogueManager = new DialogueManager();
            // ServiceLocator.Register<IDialogueService>(dialogueManager);

            // --- Localization System (Script #11) ---
            // var localizationManager = new LocalizationManager();
            // ServiceLocator.Register<ILocalizationService>(localizationManager);

            // --- Object Pool (Script #12) ---
            // var objectPool = new ObjectPoolManager();
            // ServiceLocator.Register<IObjectPoolService>(objectPool);

            Debug.Log("[Bootstraper] All services registered.");

            // --- Load next scene ---
            // Once SceneLoader exists (Script #4), uncomment:
            // ServiceLocator.Get<ISceneLoader>().LoadScene("01_MainMenu");
        }

        #endregion
    }
}
