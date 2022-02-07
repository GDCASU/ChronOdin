using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script lets the player interact with objects in the scene such as doors or pickup objects.
/// Author: Alben Trang
/// 
/// 
/// </summary>
[RequireComponent(typeof(ObjectPickup))]
public class PlayerInteractions : MonoBehaviour
{
    [HideInInspector] public static PlayerInteractions singleton;

    [Tooltip("The camera attached to Player")]
    [SerializeField]
    public Transform playerCamera;

    [Header("Properties")]
    [SerializeField]
    [Tooltip("The maximum distance the Player can pick up an object")]
    private float maxInteractDistance = 3f;

    [HideInInspector] public RaycastHit rayHit;

    private void Awake()
    {
        if (singleton == null)
            singleton = this;
        else
            Destroy(gameObject);
    }
    /// <summary>
    /// Check every frame to see if the player hits the Fire1 button (left mouse button) to interact with objects
    /// </summary>
    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out rayHit, maxInteractDistance) && rayHit.collider)
            {
                if (rayHit.transform.tag.Equals("Liftable")) GetComponent<ObjectPickup>().PickupObject();
                else if (rayHit.transform.tag.Equals("Interactable")) rayHit.transform.GetComponent<InteractiveObject>().Interact();
            }
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out rayHit, maxInteractDistance) && rayHit.collider)
                if (rayHit.transform.tag.Equals("Liftable")) GetComponent<ObjectPickup>().PickupObject();
        }
    }

    public Transform RaycastTransform()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out rayHit, maxInteractDistance) && rayHit.collider)
        {
            return rayHit.transform;
        }
        else
        {
            return null;
        }
    }
}
