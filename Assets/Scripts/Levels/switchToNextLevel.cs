using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class switchToNextLevel : MonoBehaviour
{
    public Levels nextLevel;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))SceneManager.LoadScene((int)nextLevel);
    }
}
