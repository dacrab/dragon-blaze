using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using Core.Input;

namespace UI.Menus
{
    public class BackgroundManager : MonoBehaviour
    {
        [SerializeField] private Image[] backgrounds;
        [SerializeField] private float transitionTime = 2.0f;
        [SerializeField] private InputReader inputReader;

        private int currentIndex;

        private void Start()
        {
            foreach (var bg in backgrounds)
                bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, 0);
            backgrounds[0].color = new Color(backgrounds[0].color.r, backgrounds[0].color.g, backgrounds[0].color.b, 1);
            StartCoroutine(BackgroundTransition());
        }

        private void OnEnable()
        {
            if (inputReader != null) inputReader.InteractEvent += OnInteract;
        }

        private void OnDisable()
        {
            if (inputReader != null) inputReader.InteractEvent -= OnInteract;
        }

        private void OnInteract() => SceneManager.LoadScene(0);

        private IEnumerator BackgroundTransition()
        {
            while (true)
            {
                var current = backgrounds[currentIndex];
                var next = backgrounds[(currentIndex + 1) % backgrounds.Length];

                float elapsed = 0f;
                while (elapsed < transitionTime)
                {
                    elapsed += Time.deltaTime;
                    float alpha = elapsed / transitionTime;
                    current.color = new Color(current.color.r, current.color.g, current.color.b, 1 - alpha);
                    next.color = new Color(next.color.r, next.color.g, next.color.b, alpha);
                    yield return null;
                }

                currentIndex = (currentIndex + 1) % backgrounds.Length;
            }
        }
    }
}
