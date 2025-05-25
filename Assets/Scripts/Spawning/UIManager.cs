using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TMP_Text waveText;
    public TMP_Text timerText;
    public TMP_Text enemiesText;

    public void UpdateWave(int waveNumber)
    {
        waveText.text = "Wave: " + waveNumber;
    }

    public void UpdateTimer(float timeLeft)
    {
        timerText.text = "Time: " + Mathf.Ceil(timeLeft) + "s";
    }

    public void UpdateEnemies(int count)
    {
        enemiesText.text = "Enemies Left: " + count;
    }
}
