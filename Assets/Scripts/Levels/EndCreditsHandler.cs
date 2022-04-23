using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndCreditsHandler : MonoBehaviour
{
    float timer = 90;
    void Update()
    {
        timer -= Time.deltaTime;
        if (InputManager.GetButtonDown(PlayerInput.PlayerButton.Pause) || timer<=0)
        {
            SceneManager.LoadScene(0);
        }
    }
}