using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Text UI")]
    public TMP_Text waveText;
    public TMP_Text enemiesText;

    [Header("Panels")]
    public GameObject waveClearedPanel;
    public GameObject gameOverPanel;

    [Header("Buttons")]
    public Button continueButton;
    public Button restartButton;

    [Header("Victory Panel")]
    public GameObject victoryPanel;
    public Button quitButton;

    [Header("Score UI")]
    public TMP_Text scoreText;
    public TMP_Text highScoreText;

    private void Start()
    {
        waveClearedPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        victoryPanel.SetActive(false);
    }

    public void UpdateWave(int waveNumber)
    {
        waveText.text = "Wave: " + waveNumber;
    }

    public void UpdateEnemies(int alive, int total)
    {
        enemiesText.text = $"Enemies Left: {alive} / {total}";
    }

    public void ShowWaveCleared()
    {
        waveClearedPanel.SetActive(true);
    }

    public void HideWaveCleared()
    {
        waveClearedPanel.SetActive(false);
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);

        // Reset score here as well
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
        }
    }

    public void ShowVictory()
    {
        victoryPanel.SetActive(true);
    }

    public void UpdateScore(int current, int high)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + current;

        if (highScoreText != null)
            highScoreText.text = "High Score: " + high;
    }
    public void OnQuitButtonPressed()
    {
#if UNITY_WEBGL

    Debug.Log("WebGL detected: Reloading game instead of quitting.");
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
#else
        Application.Quit(); // Works in standalone builds
#endif
    }
}
