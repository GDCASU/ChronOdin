using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adds the ability for the player to move back to where they were.
/// Author: Alben Trang
/// </summary>
public class PlayerReverse : MonoBehaviour
{
<<<<<<< HEAD:Assets/Imports/Scripts/PlayerReverse.cs
    public TestMoveThree playerMovement;
    //public TextMeshProUGUI canReverseText;
    [Range (1 , 50)]
    public int positionsSavedPerSecond;
    public float amountOfTimeReversed;
    public float reverseCooldown = 1.0f;
    public float reverseDuration;
    public float lerpBetweenPositionsRate;

    private float lerpBetweenPositions;
    public float timeBetweenPositions;
    //public float lerpBetweenPositionsTime;
=======
    public float timeBetweenPositionTracking = 0.1f;
    public float startReverseTime = 3.0f;

    public PlayerMove playerMovement;
>>>>>>> main:Assets/Scripts/Player/PlayerReverse.cs
    private List<Vector3> previousPositions;
    private List<Quaternion> previousRotations;
    public int previousPositionsLimit;
    private bool storePositions;
    private bool isAbleToReverse;

    private WaitForSeconds timeBetweenSaves;
    private WaitForFixedUpdate fixedUpdate;

    /// <summary>
    /// Start on frame one initializing variables.
    /// </summary>
    private void Start()
    {
        previousPositions = new List<Vector3>();
        previousRotations = new List<Quaternion>();
        timeBetweenPositions = 1.0f / positionsSavedPerSecond;
        previousPositionsLimit = (int) (positionsSavedPerSecond * amountOfTimeReversed);
        lerpBetweenPositionsRate = 1 / ( (reverseDuration / previousPositionsLimit) / .02f);
        if (lerpBetweenPositionsRate > 1) lerpBetweenPositionsRate = 1;
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
        //lerpBetweenPositionsTime = (reverseDuration / (float)previousPositions.Count) / (1 / lerpBetweenPositionsRate);
        //WaitForSeconds wfs = new WaitForSeconds(lerpBetweenPositionsTime);
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
<<<<<<< HEAD:Assets/Imports/Scripts/PlayerReverse.cs
                transform.position = Vector3.Lerp(newPosition, previousPositions[previousPositions.Count - 1], lerpBetweenPositions);
                transform.rotation = Quaternion.Slerp(newRotation, previousRotations[previousRotations.Count - 1], lerpBetweenPositions);
                lerpBetweenPositions += lerpBetweenPositionsRate;
                //print("stuck");
                //print(lerpBetweenPositionsTime);
                yield return fixedUpdate;
=======
                Vector3 newPosition = Vector3.Lerp(this.transform.position, previousPositions[previousPositions.Count - 1], rate);
                this.transform.position = newPosition;

                Quaternion newRotation = Quaternion.Slerp(this.transform.rotation, previousRotations[previousRotations.Count - 1], rate);
                this.transform.rotation = newRotation;

                this.GetComponent<Rigidbody>().Sleep();
>>>>>>> main:Assets/Scripts/Player/PlayerReverse.cs
            }
            previousPositions.RemoveAt(previousPositions.Count - 1);
            previousRotations.RemoveAt(previousRotations.Count - 1);
        }
        Quaternion afterReverseRotation = transform.rotation;
        //print("made it");

        //for (float slerpRate = 0.1f; slerpRate <= 1.0f; slerpRate += 0.1f)
        //{
        //    transform.rotation = Quaternion.Slerp(afterReverseRotation, beforeReverseRotation, slerpRate);
        //    yield return null;
        //}

        previousPositions.Clear();
        previousRotations.Clear();
        //Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        //rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        //rb.interpolation = RigidbodyInterpolation.Interpolate;
        playerMovement.enabled = true; // this script causes the player to snap back to the rotation before hitting the backspace key
<<<<<<< HEAD:Assets/Imports/Scripts/PlayerReverse.cs
        storePositions = true;
        //canReverseText.SetText($"Can't reverse yet...");
=======
        playerRigidBody.useGravity = true;
        isStorePosition = true;
        this.GetComponent<Rigidbody>().Sleep();
>>>>>>> main:Assets/Scripts/Player/PlayerReverse.cs

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
<<<<<<< HEAD:Assets/Imports/Scripts/PlayerReverse.cs
        yield return new WaitForSeconds(reverseCooldown);
        isAbleToReverse = true;        //canReverseText.SetText("Reverse is ready!");
        StartCoroutine(AddPosition());
=======
        yield return new WaitForSeconds(startReverseTime);
        isAbleToMoveBack = true;
        reverseTime = startReverseTime;
>>>>>>> main:Assets/Scripts/Player/PlayerReverse.cs
    }
}