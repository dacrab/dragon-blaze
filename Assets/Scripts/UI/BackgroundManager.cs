using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement; // Include this for scene management

public class BackgroundManager : MonoBehaviour
{
    public Image[] backgrounds; // Assign your 4 backgrounds in the inspector
    public float transitionTime = 2.0f; // Time it takes to fade between backgrounds
    private int currentBackgroundIndex = 0;

    void Start()
    {
        // Initialize by setting the first background to full opacity and others to transparent
        foreach (var bg in backgrounds)
        {
            bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, 0);
        }
        backgrounds[0].color = new Color(backgrounds[0].color.r, backgrounds[0].color.g, backgrounds[0].color.b, 1);

        // Start the background transition coroutine
        StartCoroutine(BackgroundTransition());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            LoadMenuLevel();
        }
    }

    IEnumerator BackgroundTransition()
    {
        while (true)
        {
            Image currentBg = backgrounds[currentBackgroundIndex];
            Image nextBg = backgrounds[(currentBackgroundIndex + 1) % backgrounds.Length];

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
            currentBackgroundIndex = (currentBackgroundIndex + 1) % backgrounds.Length;
        }
    }

    void LoadMenuLevel()
    {
        SceneManager.LoadScene(0); // Load the scene at index 0, which is the menu level
    }
}
