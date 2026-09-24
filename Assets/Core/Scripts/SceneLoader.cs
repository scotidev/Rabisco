using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Rabisco.Core
{
    /// <summary>
    /// Scene loader. Encapsulates SceneManager.LoadSceneAsync with fade in/out.
    /// Creates a black overlay UI that fades out before loading and fades in after.
    /// Uses an internal MonoBehaviour to run coroutines.
    /// </summary>
    public class SceneLoader : ISceneLoader, IGameService
    {
        #region FIELDS

        private const string c_FadeCanvasName = "-- FADE --";
        private const float c_FadeDuration = 0.5f;

        private MonoBehaviour m_CoroutineRunner;
        private Canvas m_FadeCanvas;
        private Image m_FadeImage;

        #endregion


        #region UNITY CALLBACKS

        /// <summary>
        /// Called when the SceneLoader is created by Bootstraper.
        /// Sets up the internal coroutine runner.
        /// Create a hidden GameObject with a MonoBehaviour to run coroutines
        /// </summary>
        private void Initialize()
        {
            GameObject runnerObject = new GameObject("SceneLoader_Runtime");
            m_CoroutineRunner = runnerObject.AddComponent<CoroutineRunner>();
            UnityEngine.Object.DontDestroyOnLoad(runnerObject);
            Debug.Log("[SceneLoader] Initialized.");
        }

        #endregion


        #region PUBLIC API

        /// <summary>
        /// Loads a scene by name with a fade out → load → fade in sequence.
        /// </summary>
        /// <param name="sceneName">Name of the scene to load.</param>
        public void LoadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("[SceneLoader] sceneName is null or empty.");
                return;
            }

            m_CoroutineRunner.StartCoroutine(LoadSceneAsync(sceneName));
        }

        #endregion


        #region PRIVATE

        /// <summary>
        /// Coroutine that handles the fade out, async scene load, and fade in.
        /// </summary>
        private IEnumerator LoadSceneAsync(string sceneName)
        {
            EnsureFadeOverlay();

            yield return Fade(1f);

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;

            while (!asyncLoad.isDone)
            {
                // Wait until the scene is fully loaded (progress >= 0.9)
                if (asyncLoad.progress >= 0.9f)
                {
                    asyncLoad.allowSceneActivation = true;
                }
                yield return null;
            }

            // Wait one frame for the new scene to fully initialize
            yield return null;

            // Fade in (transparent)
            yield return Fade(0f);

            Debug.Log($"[SceneLoader] Loaded scene: {sceneName}");
        }

        /// <summary>
        /// Creates the fade overlay Canvas and Image if they don't exist yet.
        /// </summary>
        private void EnsureFadeOverlay()
        {
            if (m_FadeCanvas != null && m_FadeImage != null)
            {
                return;
            }

            // Create canvas
            GameObject canvasObject = new GameObject(c_FadeCanvasName);
            m_FadeCanvas = canvasObject.AddComponent<Canvas>();
            m_FadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            m_FadeCanvas.sortingOrder = 999;

            // Create black image that covers the screen
            GameObject imageObject = new GameObject("FadeImage");
            imageObject.transform.SetParent(canvasObject.transform);

            m_FadeImage = imageObject.AddComponent<Image>();
            m_FadeImage.color = Color.black;

            // Stretch to fill screen
            RectTransform rect = m_FadeImage.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            UnityEngine.Object.DontDestroyOnLoad(canvasObject);

            // Start fully transparent (no fade)
            m_FadeImage.color = new Color(0f, 0f, 0f, 0f);
        }

        /// <summary>
        /// Fades the overlay to the target alpha (0 = transparent, 1 = black).
        /// </summary>
        private IEnumerator Fade(float targetAlpha)
        {
            Color color = m_FadeImage.color;
            float startAlpha = color.a;
            float elapsed = 0f;

            while (elapsed < c_FadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / c_FadeDuration;
                color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
                m_FadeImage.color = color;
                yield return null;
            }

            color.a = targetAlpha;
            m_FadeImage.color = color;
        }

        #endregion


        #region INTERNAL

        /// <summary>
        /// Internal MonoBehaviour used to run coroutines.
        /// Required because SceneLoader is a plain C# class, not a MonoBehaviour.
        /// </summary>
        private class CoroutineRunner : MonoBehaviour
        {
        }

        #endregion
    }
}
