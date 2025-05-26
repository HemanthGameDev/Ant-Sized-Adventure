using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TMP_Text waveText;
    public TMP_Text enemiesText;

    public void UpdateWave(int waveNumber)
    {
        waveText.text = "Wave: " + waveNumber;
    }

    public void UpdateEnemies(int count)
    {
        enemiesText.text = "Enemies Left: " + count;
    }
}
