using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// The game manager in charge of UI and other game elements.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Text Variables")]
    public TextMeshProUGUI gunStatsText;
    public TextMeshProUGUI winText;

    private (string, string, int) gunStats;

    #region Singleton for GameManager

    public static GameManager instance;

    /// <summary>
    /// Gets the singleton of the game manager to call its public functions.
    /// </summary>
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                Debug.LogError("The GameManager instance is NULL.");

            return instance;
        }
    }

    /// <summary>
    /// Sets the singleton for the GameManager to this.
    /// </summary>
    private void Awake()
    {
        instance = this;
    }
    #endregion

    /// <summary>
    /// Check at the end of every frame if all enemies are not in the scene.
    /// If so, display the win text.
    /// </summary>
    private void LateUpdate()
    {
        int enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (enemyCount == 0)
            winText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Updates the UI for the gun's stats and ammo.
    /// </summary>
    public void UpdateGunStats()
    {
        gunStats = PlayerGuns.Instance.GetCurrentGunStats();
        gunStatsText.SetText($"{gunStats.Item1}\nFire Rate: {gunStats.Item2}\nAmmo: {gunStats.Item3}");
        if (gunStats.Item3 == 0)
            gunStatsText.SetText(gunStatsText.text + "\nOUT OF AMMO!");
    }
}
