using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Put this script in an enpty game object. This takes in a platform that disappears if
/// the player touches it. The script creates a grid of the disappearing object. 
/// The grid will be placed from top left to bottom right
/// Some of the platforms won't disappear, and that is set by the game designer.
/// 
/// Author: Alben Trang
/// </summary>
public class DisappearingGrid : MonoBehaviour
{
    [Tooltip("The platform prefab that disappears upon the player touching it when set to true")]
    public GameObject disapperingPlatformPrefab;
    [Tooltip("Creates the boolean 2D array to set if each platform disappears or not")]
    public BooleanArray2D boolArray2D = new BooleanArray2D();

    /// <summary>
    /// Start on frame one by creating the grid of disappearing platforms and setting
    /// their positions & whether they can disappear by player touch or not
    /// </summary>
    void Start()
    {
        GameObject[,] platforms = new GameObject[boolArray2D.info.rows, boolArray2D.info.columns];
        Vector3 newPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        for (int i = 0; i < boolArray2D.info.rows; i++)
        {
            newPos = new Vector3(newPos.x, newPos.y, transform.position.z - (disapperingPlatformPrefab.transform.localScale.z * i));
            for (int j = 0; j < boolArray2D.booleanArrays[i].boolArray.Length; j++)
            {
                newPos = new Vector3(transform.position.x + (disapperingPlatformPrefab.transform.localScale.x * j), newPos.y, newPos.z);
                platforms[i, j] = Instantiate(disapperingPlatformPrefab, newPos, Quaternion.identity, this.gameObject.transform);
                platforms[i, j].GetComponent<DisappearWhenPlayerTouch>().SetDisappearTrigger(boolArray2D.booleanArrays[i].boolArray[j]);
                print(boolArray2D.booleanArrays[i].boolArray[j]);
            }
        }
    }

    /// <summary>
    /// Draws a wire cube of all the platforms when the object holding this scirpt is
    /// clicked on the hierarchy and can be seen in the Unity Scene view
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
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
