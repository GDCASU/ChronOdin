using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adds the ability for the player to move back to where they were.
/// Author: Alben Trang
/// 
/// Revision Author: Cristion Dominguez'
/// Modification: Commented out the cooldown coroutine as that shall be handlded in the ReverseInvocation script.
///               Added headers so the Design team can figure out which fields to edit and which fields to leave alone.
/// </summary>
public class PlayerReverse : MonoBehaviour
{
    [Header("Editable Fields")]
    public TestMoveThree playerMovement;
    [Range(1, 50)]
    public int positionsSavedPerSecond;
    public float amountOfTimeReversed;
    // public float reverseCooldown = 1.0f;

    [Header("Calculated Fields (Leave Alone)")]
    public float lerpBetweenPositionsRate;
    private float lerpBetweenPositions;
    public float timeBetweenPositions;
    private List<Vector3> previousPositions;
    private List<Quaternion> previousRotations;
    public int previousPositionsLimit;
    private bool storePositions;
    private bool isAbleToReverse;
    private bool reversing;

    private WaitForSeconds timeBetweenSaves;
    private WaitForFixedUpdate fixedUpdate;
    public float timer;

    /// <summary>
    /// Start on frame one initializing variables.
    /// </summary>
    private void Start()
    {
        previousPositions = new List<Vector3>();
        previousRotations = new List<Quaternion>();
        timeBetweenPositions = 1.0f / positionsSavedPerSecond;
        previousPositionsLimit = (int)(positionsSavedPerSecond * amountOfTimeReversed);
        lerpBetweenPositionsRate = previousPositionsLimit / 50f;
        storePositions = true;
        isAbleToReverse = true;
        timeBetweenSaves = new WaitForSeconds(timeBetweenPositions);
        fixedUpdate = new WaitForFixedUpdate();
        StartCoroutine(AddPosition());
    }

    /// <summary>
    /// Check every frame if the backspace key is pressed to reverse
    /// and start a cooldown when the key is released.
    /// </summary>
    public void CallReverse() => StartCoroutine(ReversePosition());
    private IEnumerator Timer()
    {
        while (reversing)
        {
            timer += Time.deltaTime;
            yield return null;
        }
    }
    /// <summary>
    /// Sets a loop to lerp through previous positions.
    /// </summary>
    /// <returns>Null for this coroutine.</returns>
    private IEnumerator ReversePosition()
    {
        timer = 0;
        reversing = true;
        StartCoroutine(Timer());
        playerMovement.enabled = false;
        storePositions = false;
        isAbleToReverse = false;
        //Destroy(GetComponent<Rigidbody>());
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        Quaternion beforeReverseRotation = previousRotations[previousRotations.Count - 1];

        while (previousPositions.Count > 1)
        {
            lerpBetweenPositions = 0;
            Vector3 newPosition = transform.position;
            Quaternion newRotation = transform.rotation;
            while (lerpBetweenPositions <= 1.0f)
            {
                transform.position = Vector3.Lerp(newPosition, previousPositions[previousPositions.Count - 1], lerpBetweenPositions);
                transform.rotation = Quaternion.Slerp(newRotation, previousRotations[previousRotations.Count - 1], lerpBetweenPositions);
                lerpBetweenPositions += lerpBetweenPositionsRate;
                yield return fixedUpdate;
            }
            previousPositions.RemoveAt(previousPositions.Count - 1);
            previousRotations.RemoveAt(previousRotations.Count - 1);
        }
        Quaternion afterReverseRotation = transform.rotation;

        previousPositions.Clear();
        previousRotations.Clear();

        playerMovement.enabled = true; // this script causes the player to snap back to the rotation before hitting the backspace key
        storePositions = true;
        reversing = false;
        //StartCoroutine(ReversePositionCooldown());
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
            previousPositions.Add(transform.position);
            previousRotations.Add(transform.rotation);

            if (previousPositions.Count > previousPositionsLimit)
            {
                previousPositions.RemoveAt(0);
                previousRotations.RemoveAt(0);
            }

            yield return timeBetweenSaves;
        }
    }

    /*
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
    */
}
