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
    public PlayerController playerController;
    public float reactivateControllerDelay;
    void Start()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
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
        if (InputManager.GetButtonDown(PlayerInput.PlayerButton.Pause))
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            SwitchPanels(1);
            playerController.enabled = false;
        }
        if (InputManager.GetButtonDown(PlayerInput.PlayerButton.UI_Cancel))
        {
            switch (currentPanel)
            {
                case 1:
                    ResumeGame();
                    break;
                case 4:
                    SwitchPanels(3);
                    break;
                case 6:
                    SwitchPanels(0);
                    break;
                case 7:
                    SwitchPanels(0);
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
        if(panelToActivate!=0)EventSystem.current.SetSelectedGameObject(panels[currentPanel].GetComponentInChildren<Button>().gameObject);
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

    public void NotesUI(string message)
    {
        SwitchPanels(6);
        noteMessage.text = message;
    }
    public void CustomMessage(string message)
    {
        SwitchPanels(7);
        customMessage.text = message;
    }
    private IEnumerator RestartControllerDelay()
    {
        yield return new WaitForSeconds(reactivateControllerDelay);
        playerController.enabled = true;
    }
}
