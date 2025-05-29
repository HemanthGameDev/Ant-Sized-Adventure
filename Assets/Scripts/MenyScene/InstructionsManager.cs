using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class InstructionsManager : MonoBehaviour
{
    [System.Serializable]
    public class InstructionSlide
    {
        public Sprite image;
        [TextArea] public string description;
        [HideInInspector] public bool isChecked;
    }

    public InstructionSlide[] slides;

    public Image instructionImage;
    public TMP_Text instructionText;
    public Toggle checkbox;

    public Button previousButton;
    public Button nextButton;
    public Button continueButton;

    private int currentIndex = 0;

    void Start()
    {
        checkbox.onValueChanged.AddListener(OnCheckboxChanged);
        previousButton.onClick.AddListener(GoToPrevious);
        nextButton.onClick.AddListener(GoToNext);
        continueButton.onClick.AddListener(ContinueToGame);

        ShowSlide(0);
    }

    void ShowSlide(int index)
    {
        currentIndex = index;

        InstructionSlide slide = slides[index];
        instructionImage.sprite = slide.image;
        instructionText.text = slide.description;

        checkbox.onValueChanged.RemoveAllListeners();
        checkbox.isOn = slide.isChecked;
        checkbox.onValueChanged.AddListener(OnCheckboxChanged);

        previousButton.interactable = (index > 0);
        nextButton.interactable = false; // Locked until checkbox is checked
        continueButton.gameObject.SetActive(false);

        if (slide.isChecked)
        {
            if (currentIndex < slides.Length - 1)
            {
                nextButton.interactable = true;
            }
            else
            {
                continueButton.gameObject.SetActive(true);
            }
        }
    }

    void OnCheckboxChanged(bool value)
    {
        slides[currentIndex].isChecked = value;

        if (value)
        {
            if (currentIndex < slides.Length - 1)
            {
                nextButton.interactable = true;
            }
            else
            {
                continueButton.gameObject.SetActive(true);
            }
        }
        else
        {
            nextButton.interactable = false;
            continueButton.gameObject.SetActive(false);
        }
    }

    void GoToPrevious()
    {
        if (currentIndex > 0)
            ShowSlide(currentIndex - 1);
    }

    void GoToNext()
    {
        if (currentIndex < slides.Length - 1)
            ShowSlide(currentIndex + 1);
    }

    void ContinueToGame()
    {
        // Stop music only now
        GameObject music = GameObject.Find("MenuMusicPlayer");
        if (music != null)
        {
            Destroy(music);
        }

        SceneManager.LoadScene("SampleScene");
    }

}
