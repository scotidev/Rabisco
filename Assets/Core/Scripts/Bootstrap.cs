using UnityEngine;

namespace Rabisco.Core
{
    /// <summary>
    /// Composition root of the entire game. Lives in scene Bootstrap (first in Build Settings).
    /// On Awake, it ensures this GameObject persists across all scenes via DontDestroyOnLoad,
    /// then registers all global services into the ServiceLocator.
    /// After setup, it instructs the SceneLoader to load the next scene (Main Menu or saved era).
    /// </summary>
    public class Bootstrap : MonoBehaviour
    {
        #region PRIVATE

        private const string c_SystemsContainerName = "SYSTEMS";

        #endregion


        #region UNITY CALLBACKS

        /// <summary>
        /// Called once when the Bootstrap GameObject is created in the Bootstrap scene.
        /// Steps:
        /// 1. Make this object survive scene loads (DontDestroyOnLoad).
        /// 2. Create the systems container child GameObject.
        /// 3. Register all global managers into ServiceLocator.
        /// 4. Load the next scene via SceneLoader.
        /// </summary>
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            GameObject systemsContainer = new GameObject(c_SystemsContainerName);
            systemsContainer.transform.SetParent(transform);

            // --- Step 3: Register services ---
            // As each manager is created (GameManager, AudioManager, etc.),
            // its Awake() will call ServiceLocator.Register<IXxx>(this).
            // For now, we verify ServiceLocator is accessible.
            Debug.Log("[Bootstrap] Services will register themselves via their Awake() methods.");

            // --- Step 4: Load next scene ---
            // Once SceneLoader is created (Script #4), it will be called here:
            // SceneLoader.LoadScene("01_MainMenu");
            // For now, we log that this is the placeholder.
            Debug.Log("[Bootstrap] Setup complete. SceneLoader will be called here to load next scene.");
        }

        #endregion
    }
}
