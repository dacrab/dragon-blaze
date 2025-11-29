using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Core.Input;
using Core.Constants;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UI.Menus
{
    public class BackgroundManager : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;
        [SerializeField] private Image[] backgrounds;
        [SerializeField] private float transitionTime = 2.0f;

        private int currentIndex;
        private CancellationTokenSource cts;

        private void Start()
        {
            InitializeBackgrounds();
            cts = new CancellationTokenSource();
            RunBackgroundTransitionAsync(cts.Token).Forget();
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

        private void OnDestroy() => cts?.Cancel();

        private void OnInteract() => SceneManager.LoadScene(GameConstants.Scenes.MainMenu);

        private void InitializeBackgrounds()
        {
            for (int i = 0; i < backgrounds.Length; i++)
                SetAlpha(backgrounds[i], i == 0 ? 1f : 0f);
        }

        private async UniTaskVoid RunBackgroundTransitionAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var currentBg = backgrounds[currentIndex];
                var nextBg = backgrounds[(currentIndex + 1) % backgrounds.Length];

                await FadeAsync(currentBg, nextBg, token);
                currentIndex = (currentIndex + 1) % backgrounds.Length;
            }
        }

        private async UniTask FadeAsync(Image from, Image to, CancellationToken token)
        {
            float elapsed = 0f;
            while (elapsed < transitionTime)
            {
                if (token.IsCancellationRequested) return;
                
                elapsed += Time.deltaTime;
                float t = elapsed / transitionTime;
                SetAlpha(from, 1f - t);
                SetAlpha(to, t);
                await UniTask.Yield(token);
            }
            SetAlpha(from, 0f);
            SetAlpha(to, 1f);
        }

        private static void SetAlpha(Image img, float alpha) =>
            img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);
    }
}
