using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moves the door up smoothly.
/// </summary>
public class MoveDoor : MonoBehaviour
{
    public bool open;
    private float timer;
    public float up;
    public float og;

    /// <summary>
    /// Start at frame one by setting the up and og.
    /// </summary>
    private void Start()
    {
        up=transform.position.y+transform.localScale.y;
        og=transform.position.y;
    }

    /// <summary>
    /// Set the bool open to its opposite.
    /// </summary>
    public void Move()=>open = !open;

    /// <summary>
    /// Opens or closes the door depending on the bool open.
    /// </summary>
    public void Update()
    {
        if((open)?transform.position.y<up: transform.position.y > og) transform.position += new Vector3(0, open?.05f:-.05f, 0);
    }
}
