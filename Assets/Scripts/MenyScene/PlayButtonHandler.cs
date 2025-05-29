using UnityEngine;

public class PlayButtonHandler : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject instructionsPanel;

    public void OnPlayPressed()
    {
        mainMenuPanel.SetActive(false);
        instructionsPanel.SetActive(true);

        // Do NOT destroy the music here.
        Debug.Log("Play button pressed. Instructions panel opened.");
    }
}
