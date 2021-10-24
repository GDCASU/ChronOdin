using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// An enemy that will take damage from the player's gun shots.
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("Enemy UI")]
    public Canvas enemyCanvas;
    public Image enemyHealthBar;

    [Header("Enemy Stats")]
    public float startHealth = 4.0f;

    private float health;

    /// <summary>
    /// Start at frame one the beginning stats of the enemy.
    /// </summary>
    private void Start()
    {
        health = startHealth;
    }

    /// <summary>
    /// Set the enemy health bar to look at the player.
    /// </summary>
    private void Update()
    {
        enemyCanvas.gameObject.transform.LookAt(GameObject.FindGameObjectWithTag("Player").transform);
    }

    /// <summary>
    /// Reduces the number of damage for the enemy.
    /// </summary>
    /// <param name="damage">The number of hit points to lower the enemy's health.</param>
    public void TakeDamage(int damage)
    {
        health -= damage;
        enemyHealthBar.fillAmount = health / startHealth;
        if (health <= 0)
            Destroy(gameObject);
    }
}
