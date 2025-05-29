using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject instructionsPanel;

    public void OnPlayButtonClicked()
    {
        mainMenuPanel.SetActive(false);
        instructionsPanel.SetActive(true);
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
