using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController
{
    [System.Serializable]
    public class CrouchVariables
    {
        [HideInInspector] public bool crouchBuffer;
        [HideInInspector] public bool topIsClear;
        [HideInInspector] public bool isCrouching;
        public bool slideMechanic;
        public float playerYScaleWhenCrouched = .5f;
        public float cameraDisplacement = 1;
    }
    void CrouchInput() => crouchVariables.crouchBuffer = Input.GetKey(KeyCode.LeftControl);
    public void HandleCrouchInput()
    {
        crouchVariables.topIsClear = !Physics.Raycast(transform.position - newForwardandRight.normalized * capCollider.radius,
            transform.up, capCollider.height + .01f * transform.lossyScale.y, ~ignores); // Check if thee's nothing blocking the player from standing up

        if (isGrounded)
        {
            //Crouch
            if (!crouchVariables.isCrouching && crouchVariables.crouchBuffer)
            {
                capCollider.height *= crouchVariables.playerYScaleWhenCrouched;
                capCollider.center += Vector3.up * -crouchVariables.playerYScaleWhenCrouched;
                crouchVariables.isCrouching = true;
                playerCamera.AdjustCameraHeight(true, crouchVariables.cameraDisplacement);

                //Sliding Mechanic
                if (crouchVariables.slideMechanic)
                    if (playerState != PlayerState.Sliding && rb.velocity.magnitude > slideVariables.velocityToSlide) StartCoroutine(SlideCoroutine());

            }
            //Stand Up
            if (crouchVariables.isCrouching && !crouchVariables.crouchBuffer && playerState != PlayerState.Sliding)
            {
                if (crouchVariables.topIsClear) //Checks that there are no obstacles on top of the player so they can stand up
                {
                    capCollider.height *= (1f / crouchVariables.playerYScaleWhenCrouched);
                    capCollider.center += Vector3.up * crouchVariables.playerYScaleWhenCrouched;
                    crouchVariables.isCrouching = false;
                    playerCamera.AdjustCameraHeight(false, crouchVariables.cameraDisplacement);
                }
            }
        }
    }
}
