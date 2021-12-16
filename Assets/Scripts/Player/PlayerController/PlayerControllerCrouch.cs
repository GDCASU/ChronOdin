using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController
{
    [System.Serializable]
    public class CrouchVariables
    {
        [HideInInspector]public bool crouchBuffer;
        [HideInInspector] public bool topIsClear;
        [HideInInspector] public bool isCrouching;
        public bool slideMechanic;
    }

    void CrouchInput()=> crouchVariables.crouchBuffer = Input.GetKey(KeyCode.LeftControl);
    public void HandleCrouchInput()
    {
        crouchVariables.topIsClear = !Physics.Raycast(transform.position - newForwardandRight.normalized * capCollider.radius, Vector3.up, capCollider.height + .01f); // Check if thee's nothing blocking the player from standing up

        if (isGrounded)
        {
            //Crouch
            if (!crouchVariables.isCrouching && crouchVariables.crouchBuffer)
            {
                capCollider.height *= .5f;
                capCollider.center += Vector3.up * -.5f;
                crouchVariables.isCrouching = true;
                playerCamera.AdjustCameraHeight(true);

                //Sliding Mechanic
                if (crouchVariables.slideMechanic)
                    if (playerState != PlayerState.Sliding && rb.velocity.magnitude > slideVariables.velocityToSlide) StartCoroutine(SlideCoroutine());

            }
            //Stand Up
            if (crouchVariables.isCrouching && !crouchVariables.crouchBuffer && playerState != PlayerState.Sliding)
            {
                if (crouchVariables.topIsClear) //Checks that there are no obstacles on top of the player so they can stand up
                {
                    capCollider.height *= 2f;
                    capCollider.center += Vector3.up * .5f;
                    crouchVariables.isCrouching = false;
                    playerCamera.AdjustCameraHeight(false);
                }
            }
        }
    }
}
