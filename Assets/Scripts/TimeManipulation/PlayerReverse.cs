using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adds the ability for the player to move back to where they were.
/// Author: Alben Trang
/// 
/// Editor: Cristion Dominguez
/// Modification:
///     Added a property that returns the amount of positions saved.
/// </summary>
public class PlayerReverse : MonoBehaviour
{
    public PlayerController playerMovement;
    public PlayerCamera playerCamera;
    public FMODPlaySoundEffect sfx;
    [Range(1, 50)]
    public int positionsSavedPerSecond;
    public float amountOfTimeReversed;
    public float reverseDuration = 1.5f;

    public float PreviousPositionsCount { get => previousPositions.Count; }
    public float PreviousPositionsLimit { get => previousPositionsLimit; }

    private float lerpBetweenPositions;
    private float timeBetweenPositions;
    private List<Vector3> previousPositions;
    private List<Quaternion> previousRotations;
    private int previousPositionsLimit;
    private bool storePositions;
    private float timer;

    /// <summary>
    /// Start on frame one initializing variables.
    /// </summary>
    private void Start()
    {
        previousPositions = new List<Vector3>();
        previousRotations = new List<Quaternion>();
        timeBetweenPositions = 1.0f / positionsSavedPerSecond;
        previousPositionsLimit = (int)(positionsSavedPerSecond * amountOfTimeReversed);
        storePositions = true;
    }
    private void Update()
    {
        if (storePositions)
        {
            if (timer >= timeBetweenPositions)
            {
                timer = 0;
                previousPositions.Add(transform.position);
                previousRotations.Add(transform.rotation);
                if (previousPositions.Count > previousPositionsLimit)
                {
                    previousPositions.RemoveAt(0);
                    previousRotations.RemoveAt(0);
                }
            }
            timer += Time.deltaTime;
        }
    }
    public void CallReverse() => StartCoroutine(ReversePosition());
    /// <summary>
    /// Sets a loop to lerp through previous positions.
    /// </summary>
    /// <returns>Null for this coroutine.</returns>
    private IEnumerator ReversePosition()
    {
        timer = 0;
        playerMovement.enabled = false;
        playerCamera.ToggleRotation(false);
        storePositions = false;
        sfx.PlaySoundEffect();

        int i = previousPositions.Count - 1;
        while (i >= 0)
        {
            lerpBetweenPositions = 0;
            Vector3 currentPosition = transform.position;
            Quaternion currentRotation = transform.rotation;
            float maxTimePerLocation = (reverseDuration / previousPositionsLimit);
            float timer = 0;
            while (true)
            {
                transform.position = Vector3.Lerp(currentPosition, previousPositions[i], lerpBetweenPositions);
                transform.rotation = Quaternion.Slerp(currentRotation, previousRotations[i], lerpBetweenPositions);
                lerpBetweenPositions = timer / maxTimePerLocation;
                timer += Time.deltaTime;
                if (timer >= maxTimePerLocation) break;
                yield return null;
            }
            transform.position = previousPositions[i];
            transform.rotation = previousRotations[i];
            previousPositions.RemoveAt(i);
            previousRotations.RemoveAt(i);
            i--;
        }
        storePositions = true;
        playerMovement.enabled = true;
        playerCamera.RestartRotation();
    }

}
