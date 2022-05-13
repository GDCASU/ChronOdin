using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public static MainMenu singleton;
    public int currentPanel;
    public List<GameObject> panels;

    public delegate void MusicVolumeUpdated();
    public event MusicVolumeUpdated musicUpdated;
    public delegate void SFXVolumeUpdated();
    public event SFXVolumeUpdated sfxUpdated;
    public delegate void AmbientVolumeUpdated();
    public event AmbientVolumeUpdated ambientUpdated;
    private void Awake()
    {
        if (singleton == null)
            singleton = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(panels[currentPanel].GetComponentInChildren<Button>().gameObject);
        //Check for saves, if there are some then set text of button to continue isntead of start
    }

    void Update()
    {
        if (InputManager.GetButtonDown(PlayerInput.PlayerButton.UI_Cancel))
        {
            switch (currentPanel)
            {
                default:
                    SwitchPanels(0);
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
        EventSystem.current.SetSelectedGameObject(panels[currentPanel].GetComponentInChildren<Button>().gameObject);
    }

    //public void LoadSaves() => SwitchPanels(1);
    public void LoadSaves() => SceneManager.LoadScene(1);
    public void CancelPrompt() => SwitchPanels(0);
    public void Settings() => SwitchPanels(2);
    public void Controls() => SwitchPanels(3);
    public void ExitPrompt() => SwitchPanels(4);
    public void ExitGame() => Application.Quit();

    public void Credits() => SceneManager.LoadScene((int)Levels.credits);

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
