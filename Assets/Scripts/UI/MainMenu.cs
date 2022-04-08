using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public int currentPanel;
    public List<GameObject> panels;

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

    public void LoadSaves() => SwitchPanels(1);
    public void CancelPrompt() => SwitchPanels(0);
    public void Settings() => SwitchPanels(2);
    public void Controls() => SwitchPanels(3);
    public void ExitPrompt() => SwitchPanels(4);
    public void ExitGame() => Application.Quit();
}
