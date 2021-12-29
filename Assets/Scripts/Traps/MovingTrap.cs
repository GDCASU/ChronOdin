/*
 * A trap that moves a physical component up and down. If the component hits the Player, the Player dies.
 * To move the entire trap along a path, utilize the MovingPlatformPath prefab.
 * 
 * Author: Cristion Dominguez
 * Date: 16 Dec. 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTrap : SimpleTimeManipulation
{
    [SerializeField, Tooltip("The speed the physical component moves up and down.")]
    private float speed = 2f;

    [SerializeField, Tooltip("How far the physical components travels from its moving trap parent.")]
    private float verticalDistance = 1f;

    [SerializeField, Tooltip("(Optional) The path the trap follows.")]
    private MovingPlatformPath optionalPath;

    private Transform physicalComponent;  // physical component of the trap
    public float elapsedTime = 0f;  // time since the physical component began moving or reached the value of PI

    /// <summary>
    /// Acquire the physical component.
    /// </summary>
    private void Awake()
    {
        physicalComponent = transform.GetChild(0);
    }

    /// <summary>
    /// Moves the physical component up and down.
    /// </summary>
    private void Update()
    {
        Vector3 position = physicalComponent.localPosition;
        float unitOffset = Mathf.Sin(speed * elapsedTime);
        position.y = verticalDistance * unitOffset;
        physicalComponent.localPosition = position;

        // Ensure elapsed time does not grow too large.
        if (elapsedTime >= 0 && elapsedTime % Mathf.PI <= 0.0011f)
        {
            elapsedTime = 0f;
        }

        elapsedTime += (Time.deltaTime * timeScale);
    }

    /// <summary>
    /// Kills the Player if the physical component collides with the Player.
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag.Equals("Player"))
        {
            Debug.Log("You died.");
        }
    }
}
