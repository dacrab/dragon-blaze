using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class BackgroundManager : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private Image[] backgrounds;
    [SerializeField] private float transitionTime = 2.0f;
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

    private void Update()
    {
        CheckForMenuInput();
    }
    #endregion

    #region Private Methods
    private void InitializeBackgrounds()
    {
        foreach (var bg in backgrounds)
        {
            bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, 0);
        }
        backgrounds[0].color = new Color(backgrounds[0].color.r, backgrounds[0].color.g, backgrounds[0].color.b, 1);
    }

    private void CheckForMenuInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            LoadMenuLevel();
        }
    }

    private void LoadMenuLevel()
    {
        SceneManager.LoadScene(0);
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
        float elapsed = 0.0f;
        while (elapsed < transitionTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsed / transitionTime);
            currentBg.color = new Color(currentBg.color.r, currentBg.color.g, currentBg.color.b, 1 - alpha);
            nextBg.color = new Color(nextBg.color.r, nextBg.color.g, nextBg.color.b, alpha);
            yield return null;
        }

        currentBg.color = new Color(currentBg.color.r, currentBg.color.g, currentBg.color.b, 0);
        nextBg.color = new Color(nextBg.color.r, nextBg.color.g, nextBg.color.b, 1);
    }
    #endregion
}
