using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Put this script in an enpty game object. This takes in a platform that disappears if
/// the player touches it. The script creates a grid of the disappearing object. 
/// The grid will be placed from top left to bottom right
/// Some of the platforms won't disappear, and that is set by the game designer.
/// This one uses a pressure plate to trigger the platforms to appear.
/// They will all disappear after a set amount of time until the pressure plate
/// is pressed again.
/// 
/// Author: Alben Trang
public class TimedDisappearingGrid : MonoBehaviour, LinkedToPressurePlate
{
    [Tooltip("The platform prefab that disappears upon the player touching it when set to true")]
    public GameObject disapperingPlatformPrefab;
    [Tooltip("Creates the boolean 2D array to set if each platform disappears or not")]
    public BooleanArray2D boolArray2D = new BooleanArray2D();

    [Header("Activation Delays")]
    [Tooltip("Set how many seconds before the object is activated")]
    public float activationDelay = .5f;
    [Tooltip("Set how many seconds before the object is deactivated (Tip: if the pressure plate is pressed briefly, set this higher than activationDelay for better effect)")]
    public float deactivationDelay = 1f;

    private GameObject[,] platforms;

    /// <summary>
    /// Start on frame one by creating the grid of disappearing platforms, setting
    /// their positions & whether they can disappear by player touch or not,
    /// and have them default to be hidden until a pressure plate activates them
    /// </summary>
    void Start()
    {
        platforms = new GameObject[boolArray2D.info.rows, boolArray2D.info.columns];
        Vector3 newPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        for (int i = 0; i < boolArray2D.info.rows; i++)
        {
            newPos = new Vector3(newPos.x, newPos.y, transform.position.z - (disapperingPlatformPrefab.transform.localScale.z * i));
            for (int j = 0; j < boolArray2D.booleanArrays[i].boolArray.Length; j++)
            {
                newPos = new Vector3(transform.position.x + (disapperingPlatformPrefab.transform.localScale.x * j), newPos.y, newPos.z);
                platforms[i, j] = Instantiate(disapperingPlatformPrefab, newPos, Quaternion.identity, this.gameObject.transform);
                platforms[i, j].GetComponent<DisappearWhenPlayerTouch>().SetDisappearTrigger(boolArray2D.booleanArrays[i].boolArray[j]);
                platforms[i, j].GetComponent<MeshRenderer>().enabled = false;
                platforms[i, j].GetComponent<Collider>().enabled = false;
            }
        }
    }

    /// <summary>
    /// Activates the grid of timed, disappearing platforms
    /// </summary>
    public void Activate()
    {
        StopCoroutine(DeactivateGrid());
        StartCoroutine(ActivateGrid());
    }

    /// <summary>
    /// Deactivates the grid of timed, disappearing platforms
    /// </summary>
    public void Deactivate()
    {
        StopCoroutine(ActivateGrid());
        StartCoroutine(DeactivateGrid());
    }

    /// <summary>
    /// Enables all platforms after an activation delay
    /// </summary>
    /// <returns>null</returns>
    private IEnumerator ActivateGrid()
    {
        yield return new WaitForSeconds(activationDelay);

        for (int i = 0; i < boolArray2D.info.rows; i++)
        {
            for (int j = 0; j < boolArray2D.booleanArrays[i].boolArray.Length; j++)
            {
                platforms[i, j].GetComponent<MeshRenderer>().enabled = true;
                platforms[i, j].GetComponent<Collider>().enabled = true;
            }
        }
    }

    /// <summary>
    /// Disables all platforms after a deactivation delay
    /// </summary>
    /// <returns>null</returns>
    private IEnumerator DeactivateGrid()
    {
        yield return new WaitForSeconds(deactivationDelay);

        for (int i = 0; i < boolArray2D.info.rows; i++)
        {
            for (int j = 0; j < boolArray2D.booleanArrays[i].boolArray.Length; j++)
            {
                platforms[i, j].GetComponent<MeshRenderer>().enabled = false;
                platforms[i, j].GetComponent<Collider>().enabled = false;
            }
        }
    }

    /// <summary>
    /// Draws a wire cube of all the platforms when the object holding this scirpt is
    /// clicked on the hierarchy and can be seen in the Unity Scene view
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 newPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 platformSize = disapperingPlatformPrefab.transform.localScale;

        for (int i = 0; i < boolArray2D.info.rows; i++)
        {
            newPos = new Vector3(newPos.x, newPos.y, transform.position.z - (disapperingPlatformPrefab.transform.localScale.z * i));
            for (int j = 0; j < boolArray2D.booleanArrays[i].boolArray.Length; j++)
            {
                newPos = new Vector3(transform.position.x + (disapperingPlatformPrefab.transform.localScale.x * j), newPos.y, newPos.z);
                Gizmos.DrawWireCube(newPos, platformSize);
            }
        }
    }
}
