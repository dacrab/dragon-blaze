using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Core.Constants;
using Core.Input;
using Core.Services;

namespace UI.Menus
{
    public sealed class CreditsBackgroundManager : MonoBehaviour
    {
        [SerializeField] Image[] backgrounds;
        [SerializeField] float transitionTime = 2f;

        int currentIndex;
        Color[] baseColors;
        InputReader inputReader;
        CancellationTokenSource loopCts;

        void Start()
        {
            if (backgrounds is not { Length: > 0 }) return;
            baseColors = new Color[backgrounds.Length];
            for (int i = 0; i < backgrounds.Length; i++)
            {
                baseColors[i] = backgrounds[i].color;
                backgrounds[i].color = new(baseColors[i].r, baseColors[i].g, baseColors[i].b, 0);
            }
            backgrounds[0].color = baseColors[0];
            loopCts = new CancellationTokenSource();
            _ = TransitionLoop(loopCts.Token);
        }

        void OnEnable()
        {
            inputReader = InputReader.Instance;
            if (inputReader != null) inputReader.InteractEvent += LoadMainMenu;
        }

        void OnDisable()
        {
            if (inputReader != null) inputReader.InteractEvent -= LoadMainMenu;
        }

        void OnDestroy() => loopCts?.Cancel();

        void LoadMainMenu() => ServiceLocator.Get<ISceneLoader>()?.LoadScene(GameConfig.Default.MainMenuSceneName);

        async Awaitable TransitionLoop(CancellationToken ct)
        {
            int count = backgrounds.Length;
            while (!ct.IsCancellationRequested && gameObject.activeInHierarchy)
            {
                var current = backgrounds[currentIndex];
                var next = backgrounds[(currentIndex + 1) % count];
                await CrossFade(current, next, ct);
                currentIndex = (currentIndex + 1) % count;
            }
        }

        async Awaitable CrossFade(Image from, Image to, CancellationToken ct)
        {
            for (float t = 0; t < transitionTime && !ct.IsCancellationRequested; t += Time.deltaTime)
            {
                float a = Mathf.Clamp01(t / transitionTime);
                SetAlpha(from, 1 - a);
                SetAlpha(to, a);
                await Awaitable.NextFrameAsync();
            }
        }

        void SetAlpha(Image image, float alpha) =>
            image.color = new(image.color.r, image.color.g, image.color.b, alpha);
    }
}
