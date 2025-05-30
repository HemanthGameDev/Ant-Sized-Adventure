using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    [Header("Assign the Transition Canvas prefab (used in first scene only)")]
    [SerializeField] private GameObject transitionCanvasPrefab;

    private GameObject currentCanvasInstance;
    private Animator transitionAnim;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeInAndDestroyCanvas());
    }

    IEnumerator FadeInAndDestroyCanvas()
    {
        yield return null; // Wait one frame for scene to fully load

        // Attempt to find a canvas from scene if not kept as reference
        if (currentCanvasInstance == null)
        {
            currentCanvasInstance = GameObject.FindWithTag("TransitionCanvas");

            if (currentCanvasInstance == null && transitionCanvasPrefab != null)
            {
                currentCanvasInstance = Instantiate(transitionCanvasPrefab);
                Debug.Log("Instantiated transition canvas from prefab.");
            }
        }

        if (currentCanvasInstance != null)
        {
            currentCanvasInstance.SetActive(true);

            transitionAnim = currentCanvasInstance.GetComponent<Animator>();
            if (transitionAnim != null)
            {
                transitionAnim.ResetTrigger("Start");
                transitionAnim.SetTrigger("Start");

                // Wait until current animation completes using its length
                float animLength = transitionAnim.GetCurrentAnimatorStateInfo(0).length;
                yield return new WaitForSeconds(animLength > 0 ? animLength : 1f);

                Debug.Log("Fade-in animation completed. Destroying canvas.");
            }

            Destroy(currentCanvasInstance);
            currentCanvasInstance = null;
        }
        else
        {
            Debug.LogWarning("Transition canvas not found in scene or prefab not assigned.");
        }
    }

    public void NextLevel()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
