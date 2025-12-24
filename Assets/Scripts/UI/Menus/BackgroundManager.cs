using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using Core.Input;
using Core.Utilities;

namespace UI.Menus
{
    public class BackgroundManager : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private Image[] backgrounds;
        [SerializeField] private float transitionTime = 2.0f;
        [SerializeField] private InputReader inputReader;
        #endregion

        #region Private Fields
        private int currentBackgroundIndex = 0;
        #endregion

        #region Unity Lifecycle Methods
        private void Start()
        {
            InitializeBackgrounds();
            StartCoroutine(BackgroundTransition());
        }

        private void OnEnable()
        {
            if (inputReader != null)
                inputReader.InteractEvent += OnInteract;
        }

        private void OnDisable()
        {
            if (inputReader != null)
                inputReader.InteractEvent -= OnInteract;
        }

        private void OnInteract()
        {
            SceneManager.LoadScene(0);
        }
        #endregion

        #region Private Methods
        private void InitializeBackgrounds()
        {
            foreach (var bg in backgrounds) bg.color = bg.color.WithAlpha(0);
            backgrounds[0].color = backgrounds[0].color.WithAlpha(1);
        }
        #endregion

        #region Coroutines
        private IEnumerator BackgroundTransition()
        {
            while (true)
            {
                Image currentBg = backgrounds[currentBackgroundIndex];
                Image nextBg = backgrounds[(currentBackgroundIndex + 1) % backgrounds.Length];

                yield return StartCoroutine(FadeBackgrounds(currentBg, nextBg));

                currentBackgroundIndex = (currentBackgroundIndex + 1) % backgrounds.Length;
            }
        }

        private IEnumerator FadeBackgrounds(Image currentBg, Image nextBg)
        {
            float elapsed = 0f;
            while (elapsed < transitionTime)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0, 1, elapsed / transitionTime);
                currentBg.color = currentBg.color.WithAlpha(1 - alpha);
                nextBg.color = nextBg.color.WithAlpha(alpha);
                yield return null;
            }
            currentBg.color = currentBg.color.WithAlpha(0);
            nextBg.color = nextBg.color.WithAlpha(1);
        }
        #endregion
    }
}
