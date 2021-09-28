
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {

    public GameObject settingsPanel;

    public Slider sensitivitySlider, FOVSlider;
    public TextMeshProUGUI sensitivityText, FOVText;

    public void SetSensitivity() {
        Settings.Sensitivity = sensitivitySlider.value;
        sensitivityText.text = Settings.Sensitivity.ToString();
    }

    public void SetFOV() {
        Settings.FOV = FOVSlider.value;
        FOVText.text = Settings.FOV.ToString();
    }

    public void ApplySettingsToPlayer() {
        //FindObjectOfType
    }

    private void Start() {
        sensitivitySlider.value = Settings.Sensitivity;
        sensitivityText.text = Settings.Sensitivity.ToString();
        FOVSlider.value = Settings.FOV;
        FOVText.text = Settings.FOV.ToString();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(Time.timeScale > 0) {
                Time.timeScale = 0;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else {
                Time.timeScale = 1;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                ApplySettingsToPlayer();
            }
            settingsPanel.SetActive(!settingsPanel.activeSelf);
            Settings.Paused = !Settings.Paused;
        }
    }

}
