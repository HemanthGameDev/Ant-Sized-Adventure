using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenuController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject settingsPanel;
    public Slider sensitivitySlider;
    public Slider volumeSlider;

    [Header("Camera & Audio")]
    public CameraFollowCollision cameraFollowScript;
    public AudioSource musicAudioSource;

    private const string SensitivityKey = "CameraSensitivity";
    private const string VolumeKey = "MusicVolume";

    void Start()
    {
        // Load saved values or use defaults
        float savedSensitivity = PlayerPrefs.GetFloat(SensitivityKey, 2f);
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 0.6f);

        sensitivitySlider.value = savedSensitivity;
        volumeSlider.value = savedVolume;

        ApplySensitivity(savedSensitivity);
        ApplyVolume(savedVolume);

        // Add listeners
        sensitivitySlider.onValueChanged.AddListener(ApplySensitivity);
        volumeSlider.onValueChanged.AddListener(ApplyVolume);

        settingsPanel.SetActive(false); // Start hidden
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void ApplySensitivity(float value)
    {
        if (cameraFollowScript != null)
        {
            cameraFollowScript.mouseSensitivity = value;
        }
        PlayerPrefs.SetFloat(SensitivityKey, value);
    }

    void ApplyVolume(float value)
    {
        if (musicAudioSource != null)
        {
            musicAudioSource.volume = value;
        }
        PlayerPrefs.SetFloat(VolumeKey, value);
    }
}
