using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [HideInInspector] public static PauseMenu singleton;

    public int currentPanel;
    public List<GameObject> panels;
    public Text noteMessage;
    public Text customMessage;
    public PlayerCamera playerCamera;
    public float reactivateControllerDelay;
    public bool messagePreped = false;

    public delegate void MusicVolumeUpdated();
    public event MusicVolumeUpdated musicUpdated;
    public delegate void SFXVolumeUpdated();
    public event SFXVolumeUpdated sfxUpdated;
    public delegate void AmbientVolumeUpdated();
    public event AmbientVolumeUpdated ambientUpdated;
    void Start()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        SetVolumeSliderValues();
    }
    private void Awake()
    {
        if (singleton == null)
            singleton = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (currentPanel == 0 && (InputManager.GetButtonDown(PlayerInput.PlayerButton.Pause)))
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            SwitchPanels(1);
            PlayerController.singleton.DisableMovement();
            playerCamera.enabled = false;
        }
        else if (currentPanel != 0 && InputManager.GetButtonDown(PlayerInput.PlayerButton.UI_Cancel))
        {
            switch (currentPanel)
            {
                case 1:
                    ResumeGame();
                    break;
                case 6:
                    if (messagePreped)
                    {
                        SwitchPanels(7);
                        messagePreped = false;
                    }
                    else
                    {
                        SwitchPanels(0);
                        StartCoroutine(RestartControllerDelay());
                    }
                    break;
                case 7:
                    SwitchPanels(0);
                    StartCoroutine(RestartControllerDelay());
                    break;
                default:
                    SwitchPanels(1);
                    break;
            }
        }
    }
    public void SwitchPanels(int panelToActivate)
    {
        panels[currentPanel].SetActive(false);
        panels[panelToActivate].SetActive(true);
        currentPanel = panelToActivate;
        EventSystem.current.SetSelectedGameObject(null);
        if(panelToActivate!=0 && panelToActivate != 6 && panelToActivate != 7) EventSystem.current.SetSelectedGameObject(panels[currentPanel].GetComponentInChildren<Button>().gameObject);
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
        SwitchPanels(0);
        Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine(RestartControllerDelay());
    }
    public void CancelPrompt() => SwitchPanels(1);
    public void RestartPrompt() =>SwitchPanels(2);
    public void RestartLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().name,LoadSceneMode.Single);
    public void Settings() => SwitchPanels(3);
    public void Controls() => SwitchPanels(4);
    public void MainMenuPrompt() => SwitchPanels(5);
    public void MainMenu() => SceneManager.LoadScene((int)Levels.title, LoadSceneMode.Single);

    private void SetVolumeSliderValues()
    {
        Slider[] sliders = panels[3].GetComponentsInChildren<Slider>();
        sliders[0].value = AudioVolumeValues.singleton.MusicVolume;
        sliders[1].value = AudioVolumeValues.singleton.SFXVolume;
        sliders[2].value = AudioVolumeValues.singleton.AmbientVolume;
    }

    public void DisplayNote(string message)
    {
        SwitchPanels(6);
        noteMessage.text = message;
        PlayerController.singleton.DisableMovement();
    }
    public void CustomMessage(string message)
    {
        SwitchPanels(7);
        customMessage.text = message;
    }
    public void PrepCustomMessage(string message)
    {
        customMessage.text = message;
        messagePreped = true;
    }
    private IEnumerator RestartControllerDelay()
    {
        playerCamera.enabled = true;
        yield return new WaitForSeconds(reactivateControllerDelay);
        PlayerController.singleton.EnableMovement();
    }
    public void UpdateMusicVolume(float value)
    {
        AudioVolumeValues.singleton.MusicVolume = value;
        if (musicUpdated != null) musicUpdated();
    }
    public void UpdateSFXVolume(float value)
    {
        AudioVolumeValues.singleton.SFXVolume = value;
        if (sfxUpdated != null) sfxUpdated();
    }
    public void UpdateAmbientVolume(float value)
    {
        AudioVolumeValues.singleton.AmbientVolume = value;
        if (ambientUpdated != null) ambientUpdated();
    }
}
