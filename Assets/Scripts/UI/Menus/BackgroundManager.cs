using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using Core.Input;

namespace UI.Menus
{

public sealed class BackgroundManager : MonoBehaviour
{
    [SerializeField] Image[] backgrounds;
    [SerializeField] float transitionTime = 2f;
    [SerializeField] InputReader inputReader;

    int currentIndex;

    void Start()
    {
        foreach (var bg in backgrounds) bg.color = new(bg.color.r, bg.color.g, bg.color.b, 0);
        if (backgrounds.Length > 0) backgrounds[0].color = Color.white;
        StartCoroutine(Transition());
    }

    void OnEnable() { if (inputReader != null) inputReader.InteractEvent += () => SceneManager.LoadScene(0); }
    void OnDisable() { if (inputReader != null) inputReader.InteractEvent -= () => SceneManager.LoadScene(0); }

    IEnumerator Transition()
    {
        while (true)
        {
            var current = backgrounds[currentIndex];
            var next = backgrounds[(currentIndex + 1) % backgrounds.Length];

            for (float t = 0; t < transitionTime; t += Time.deltaTime)
            {
                float a = t / transitionTime;
                current.color = new(1, 1, 1, 1 - a);
                next.color = new(1, 1, 1, a);
                yield return null;
            }

            currentIndex = (currentIndex + 1) % backgrounds.Length;
        }
    }
}
}