using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionController : MonoBehaviour
{
    public Animator transitionAnimator; // Assign in Inspector
    public string sceneToLoad = "SampleScene"; // Change this from button or Inspector
    public float fadeInDuration = 1f;  // Match clip length
    public float fadeOutDuration = 1f; // Match clip length

    public void StartSceneTransition()
    {
        StartCoroutine(HandleFullSceneTransition());
    }

    private IEnumerator HandleFullSceneTransition()
    {
        // Step 1: Play fade-in animation
        transitionAnimator.Play("SceneTransitions");
        yield return new WaitForSeconds(fadeInDuration);

        // Step 2: Load the next scene
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(sceneToLoad);
    }

    // Step 3: After scene loads, play fade-out
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe to avoid duplicate calls

        StartCoroutine(FadeOutAfterSceneLoad());
    }

    private IEnumerator FadeOutAfterSceneLoad()
    {
        yield return new WaitForSeconds(0.1f); // Small delay to let scene initialize

        Animator fadeOutAnimator = FindFirstObjectByType<Animator>();
        if (fadeOutAnimator != null)
        {
            fadeOutAnimator.Play("SceneTransitionFadeOut");
            yield return new WaitForSeconds(fadeOutDuration);
        }
    }
}
