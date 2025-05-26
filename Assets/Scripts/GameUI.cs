using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public void OnContinueButtonPressed()
    {
        WaveManager.Instance.ContinueToNextWave();
    }

    public void OnRestartButtonPressed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void OnQuitButtonPressed()
    {
        Debug.Log("Quitting game...");
        Application.Quit();

        // In the editor, stop play mode
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

}
