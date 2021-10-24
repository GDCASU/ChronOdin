using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Adds the ability for the player to move back to where they were.
/// Author: Alben Trang
/// </summary>
public class PlayerReverse : MonoBehaviour
{
    [Header("Unity Specific Variables")]
    [Tooltip("The movement script that should be disabled when the player is being reversed")]
    public FirstPersonAIO playerMovement;
    [Tooltip("UI TextMeshProUGUI that displays if the player can reverse or not")]
    public TextMeshProUGUI canReverseText;
    [Tooltip("How many seconds it takes before the player rewind themselves again")]
    public float reverseCooldown = 1.0f;

    private List<Vector3> previousPositions;
    private List<Quaternion> previousRotations;
    private Rigidbody playerRigidBody;
    public float timeBetweenPositionTracking;
    private float reverseTimeLength;
    private float reverseTime;
    private int previousPositionsLimit;
    private bool isStorePosition;
    private bool isAbleToMoveBack;
    private bool isCountingDown;

    /// <summary>
    /// Start on frame one initializing variables.
    /// </summary>
    private void Start()
    {
        previousPositions = new List<Vector3>();
        previousRotations = new List<Quaternion>();
        playerRigidBody = GetComponent<Rigidbody>();
        timeBetweenPositionTracking = 0.1f;
        reverseTimeLength = 3.0f;
        reverseTime = reverseTimeLength;
        previousPositionsLimit = 90 * (int)reverseTimeLength;
        isStorePosition = true;
        isAbleToMoveBack = true;
        isCountingDown = false;

        StartCoroutine(AddPosition());
    }

    /// <summary>
    /// Check every frame if the backspace key is pressed to reverse
    /// and start a cooldown when the key is released.
    /// </summary>
    private void Update()
    {
        // Reverse the player's position if the cooldown is done.
        if (isAbleToMoveBack && Input.GetKeyDown(KeyCode.Backspace))
        {
            playerMovement.enabled = false;
            playerRigidBody.useGravity = false;
            isStorePosition = false;
            isAbleToMoveBack = false;
            isCountingDown = true;
            StartCoroutine(ReversePosition());
            StartCoroutine(ReversePositionCooldown());
        }

        if (isCountingDown && reverseTime > 0.0f)
        {
            reverseTime -= Time.deltaTime;
        }
        else
        {
            isCountingDown = false;
        }
    }

    /// <summary>
    /// Sets a loop to lerp through previous positions.
    /// </summary>
    /// <returns>Null for this coroutine.</returns>
    private IEnumerator ReversePosition()
    {
        Quaternion beforeReverseRotation = previousRotations[previousRotations.Count - 1];
        while (isCountingDown && previousPositions.Count > 1)
        {
            for (float rate = 0.1f; rate <= 1.0f; rate += 0.1f)
            {
                Vector3 newPosition = Vector3.Lerp(this.transform.position, previousPositions[previousPositions.Count - 1], rate);
                this.transform.position = newPosition;

                Quaternion newRotation = Quaternion.Slerp(this.transform.rotation, previousRotations[previousRotations.Count - 1], rate);
                this.transform.rotation = newRotation;
                
                this.GetComponent<Rigidbody>().Sleep();
            }

            previousPositions.RemoveAt(previousPositions.Count - 1);
            previousRotations.RemoveAt(previousRotations.Count - 1);
            yield return null;
        }
        Quaternion afterReverseRotation = this.transform.rotation;

        for (float slerpRate = 0.1f; slerpRate <= 1.0f; slerpRate += 0.1f)
        {
            this.transform.rotation = Quaternion.Slerp(afterReverseRotation, beforeReverseRotation, slerpRate);
            yield return null;
        }
        this.GetComponent<Rigidbody>().Sleep();

        previousPositions.Clear();
        previousRotations.Clear();
        playerMovement.enabled = true; // this script causes the player to snap back to the rotation before hitting the backspace key
        playerRigidBody.useGravity = true;
        isStorePosition = true;
        canReverseText.SetText($"Can't reverse yet...");
        this.GetComponent<Rigidbody>().Sleep();

        StartCoroutine(AddPosition());
    }

    /// <summary>
    /// Adds previous positions from the player as they move.
    /// </summary>
    /// <returns>Yields time between each position tracked.</returns>
    private IEnumerator AddPosition()
    {
        while (isStorePosition)
        {
            previousPositions.Add(this.gameObject.transform.position);
            previousRotations.Add(this.gameObject.transform.rotation);
            if (previousPositions.Count > previousPositionsLimit)
            {
                previousPositions.RemoveAt(0);
                previousRotations.RemoveAt(0);
            }

            yield return new WaitForSeconds(timeBetweenPositionTracking);
        }
    }

    /// <summary>
    /// Activates a cooldown before the player can reverse their position again.
    /// </summary>
    /// <returns>Yields time before the cooldown ends.</returns>
    private IEnumerator ReversePositionCooldown()
    {
        yield return new WaitForSeconds(reverseCooldown);
        isAbleToMoveBack = true;
        reverseTime = reverseTimeLength;
        canReverseText.SetText("Reverse is ready!");
    }
}
