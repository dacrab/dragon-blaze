using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Core.Input;

namespace UI.Menus
{
    public sealed class CreditsBackgroundManager : MonoBehaviour
    {
        [SerializeField] Image[] backgrounds;
        [SerializeField] float transitionTime = 2f;
        [SerializeField] InputReader inputReader;

        int currentIndex;

        void Start()
        {
            foreach (var bg in backgrounds) bg.color = new(bg.color.r, bg.color.g, bg.color.b, 0);
            if (backgrounds.Length > 0) backgrounds[0].color = Color.white;
            else return;
            _ = TransitionLoop();
        }

        void OnEnable() { if (inputReader != null) inputReader.InteractEvent += LoadMainMenu; }
        void OnDisable() { if (inputReader != null) inputReader.InteractEvent -= LoadMainMenu; }

		void LoadMainMenu() => SceneManager.LoadScene(GameConstants.Scenes.MainMenu);

        async Awaitable TransitionLoop()
        {
            while (this != null && gameObject.activeInHierarchy)
            {
                var current = backgrounds[currentIndex];
                var next = backgrounds[(currentIndex + 1) % backgrounds.Length];

                for (float t = 0; t < transitionTime; t += Time.deltaTime)
                {
                    float a = t / transitionTime;
                    current.color = new(1, 1, 1, 1 - a);
                    next.color = new(1, 1, 1, a);
                    await Awaitable.NextFrameAsync();
                }

                currentIndex = (currentIndex + 1) % backgrounds.Length;
            }
        }
    }
}
