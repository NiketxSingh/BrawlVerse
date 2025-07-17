using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InstructionsUI : MonoBehaviour {

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject instructionsPanel;

    public void MainMenu() {
        mainMenuPanel.SetActive(true);
        instructionsPanel.SetActive(false);
    }
}
