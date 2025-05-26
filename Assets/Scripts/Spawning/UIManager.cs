using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    }
    public void ShowVictory()
    {
        victoryPanel.SetActive(true);
    }

}
