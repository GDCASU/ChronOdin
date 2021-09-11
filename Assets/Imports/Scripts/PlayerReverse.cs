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
    public TestMoveThree playerMovement;
    //public TextMeshProUGUI canReverseText;
    public int positionsSavedPerSecond;
    public float amountOfTimeReversed;
    public float reverseCooldown = 1.0f;
    public float lerpBetweenPositionsRate;

    private float lerpBetweenPositions;
    private float timeBetweenPositions;
    private float lerpBetweenPositionsTime;
    private List<Vector3> previousPositions;
    private List<Quaternion> previousRotations;
    private int previousPositionsLimit;
    private bool storePositions;
    private bool isAbleToReverse;

    private WaitForSeconds timeBetweenSaves;
    private WaitForSeconds timeBetweenLerps;

    /// <summary>
    /// Start on frame one initializing variables.
    /// </summary>
    private void Start()
    {
        previousPositions = new List<Vector3>();
        previousRotations = new List<Quaternion>();
        timeBetweenPositions = 1.0f / positionsSavedPerSecond;
        lerpBetweenPositionsTime = 1.0f / lerpBetweenPositionsRate;
        previousPositionsLimit = (int) (positionsSavedPerSecond * amountOfTimeReversed);
        storePositions = true;
        isAbleToReverse = true;
        timeBetweenSaves = new WaitForSeconds(timeBetweenPositions);
        timeBetweenLerps = new WaitForSeconds(lerpBetweenPositionsTime);
        StartCoroutine(AddPosition());
    }

    /// <summary>
    /// Check every frame if the backspace key is pressed to reverse
    /// and start a cooldown when the key is released.
    /// </summary>
    private void Update()
    {
        // Reverse the player's position if the cooldown is done.
        if (isAbleToReverse && Input.GetKeyDown(KeyCode.Backspace)) StartCoroutine(ReversePosition());
    }

    /// <summary>
    /// Sets a loop to lerp through previous positions.
    /// </summary>
    /// <returns>Null for this coroutine.</returns>
    private IEnumerator ReversePosition()
    {
        playerMovement.enabled = false;
        storePositions = false;
        isAbleToReverse = false;
        Destroy(GetComponent<Rigidbody>());

        Quaternion beforeReverseRotation = previousRotations[previousRotations.Count - 1];

        while (previousPositions.Count > 1)
        {
            lerpBetweenPositions = 0;
            Vector3 newPosition = transform.position;
            Quaternion newRotation = transform.rotation;
            while (lerpBetweenPositions <= 1.0f)
            {
                transform.position = Vector3.Lerp(newPosition, previousPositions[previousPositions.Count - 1], lerpBetweenPositions); ;

                transform.rotation = Quaternion.Slerp(newRotation, previousRotations[previousRotations.Count - 1], lerpBetweenPositions);
                transform.rotation = newRotation;
                lerpBetweenPositions += lerpBetweenPositionsRate;
                yield return timeBetweenLerps;
            }
            previousPositions.RemoveAt(previousPositions.Count - 1);
            previousRotations.RemoveAt(previousRotations.Count - 1);
        }
        Quaternion afterReverseRotation = this.transform.rotation;

        //for (float slerpRate = 0.1f; slerpRate <= 1.0f; slerpRate += 0.1f)
        //{
        //    transform.rotation = Quaternion.Slerp(afterReverseRotation, beforeReverseRotation, slerpRate);
        //    yield return null;
        //}

        previousPositions.Clear();
        previousRotations.Clear();
        playerMovement.enabled = true; // this script causes the player to snap back to the rotation before hitting the backspace key
        storePositions = true;
        //canReverseText.SetText($"Can't reverse yet...");
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        StartCoroutine(ReversePositionCooldown());
        StartCoroutine(AddPosition());
    }

    /// <summary>
    /// Adds previous positions from the player as they move.
    /// </summary>
    /// <returns>Yields time between each position tracked.</returns>
    private IEnumerator AddPosition()
    {
        while (storePositions)
        {
            previousPositions.Add(this.gameObject.transform.position);
            previousRotations.Add(this.gameObject.transform.rotation);
            if (previousPositions.Count > previousPositionsLimit)
            {
                previousPositions.RemoveAt(0);
                previousRotations.RemoveAt(0);
            }

            yield return timeBetweenSaves;
        }
    }

    /// <summary>
    /// Activates a cooldown before the player can reverse their position again.
    /// </summary>
    /// <returns>Yields time before the cooldown ends.</returns>
    private IEnumerator ReversePositionCooldown()
    {
        yield return new WaitForSeconds(reverseCooldown);
        isAbleToReverse = true;        //canReverseText.SetText("Reverse is ready!");
        StartCoroutine(AddPosition());
    }
}
