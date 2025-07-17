using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {

    [Header("Buttons")]
    public Button playButton;
    public Button quitButton;
    public Button instructionsButton;

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject instructionsPanel;

    void Start() {
        // Automatically bind buttons
        if (playButton != null)
            playButton.onClick.AddListener(PlayGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        if (instructionsButton != null)
            instructionsButton.onClick.AddListener(Instructions);
    }

    void PlayGame() {
        Debug.Log("Play button clicked");
        SceneManager.LoadScene("Level2");
    }

    void QuitGame() {
        Debug.Log("Quit button clicked");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void Instructions() {
        instructionsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

}
