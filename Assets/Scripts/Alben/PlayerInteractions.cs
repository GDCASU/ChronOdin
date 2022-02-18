using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public Text InteractionText;

    [Header("Properties")]
    [SerializeField]
    [Tooltip("The maximum distance the Player can pick up an object")]
    private float maxInteractDistance = 3f;

    [HideInInspector] public RaycastHit rayHit;

    public PlayerInput.PlayerButton button;

    public Transform transformBeingLookedAt;

    private PlayerInput.PlayerAction action;

    private void Awake()
    {
        if (singleton == null)
            singleton = this;
        else
            Destroy(gameObject);
    }
    private void Start() => action = InputManager.playerButtons[PlayerInput.PlayerButton.Interact];
    /// <summary>
    /// Check every frame to see if the player hits the Fire1 button (left mouse button) to interact with objects
    /// </summary>
    private void Update()
    {
        InteractionText.text = "";
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out rayHit, maxInteractDistance) && rayHit.collider)
        {
            if (rayHit.transform.tag.Equals("Liftable"))
            {
                InteractionText.text = "Press " + (InputManager.inputMode == InputManager.InputMode.controller? 
                    action.xboxKey.ToString(): action.keyboardKey.ToString()) + " to PickUp";
                if (InputManager.GetButtonDown(button))
                {
                    var objectPickup = GetComponent<ObjectPickup>();
                    if (objectPickup.heldObject) objectPickup.ReleaseObject();
                    else objectPickup.PickupObject();
                }    
            }
            else if (rayHit.transform.tag.Equals("Interactable"))
            {
                InteractionText.text = "Press " + (InputManager.inputMode == InputManager.InputMode.controller ?
                    action.xboxKey.ToString() : action.keyboardKey.ToString()) + "to  Interact";
                if (InputManager.GetButtonDown(button)) rayHit.transform.GetComponent<InteractiveObject>().Interact();
            }
            else transformBeingLookedAt = rayHit.transform;
        }
        else transformBeingLookedAt = null;
        //if (InputManager.GetButtonUp(button))
        //{
        //    if (rayHit.collider)
        //        if (rayHit.transform.tag.Equals("Liftable")) GetComponent<ObjectPickup>().ReleaseObject();
        //}
    }

    public Transform RaycastTransform() => transformBeingLookedAt;
}
