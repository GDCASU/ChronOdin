/*
 * Freezes objects for a specified amount of time.
 * The Player has the option to freeze a single object or all freezeable objects.
 * To freeze a single object, the Player must look at the object and press the freeze single object button whilst the corresponding cooldown is inactive.
 * To freeze the environment, the Player must press the freeze environment button whilst the corresponding cooldown is inactive.
 * 
 * Author: Cristion Dominguez
 * Date: 10 September 2021
 */

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FreezeInvocation : MonoBehaviour
{
    [Header("Button")]
    [Tooltip("The button to freeze a single object")]
    [SerializeField]
    private KeyCode freezeSingleObjectButton = KeyCode.F;

    [Tooltip("The button to freeze the entire environment")]
    [SerializeField]
    private KeyCode freezeEnvironmentButton = KeyCode.G;

    [Header("Time Values")]
    [Tooltip("The time the object shall be frozen for")]
    [SerializeField]
    private float freezeSingleTime = 5f;

    [Tooltip("The duration the object can't be frozen after being frozen")]
    [SerializeField]
    private float freezeSingleCooldown = 5f;

    [Tooltip("The duration the environment shall be frozen for")]
    [SerializeField]
    private float freezeEnvironmentTime = 10f;

    [Tooltip("The duration the environment can't be frozen after being frozen")]
    [SerializeField]
    private float freezeEnvironmentCooldown = 10f;

    [Header("Transforms")]
    [Tooltip("The camera providing the Player vision")]
    [SerializeField]
    private Transform playerCamera;
    [SerializeField]
    private Text globalActiveTimerText;
    [SerializeField]
    private Text globalCooldownTimerText;
    [SerializeField]
    private Text localActiveTimerText;
    [SerializeField]
    private Text localCooldownTimerText;


    // Values for casting a ray to detect collisions.
    RaycastHit rayHit;
    private Vector3 startRayPosition, rayDirection;
    private int maxRayCasts = 2;
    private float rayPositionOffset = 0.000006f;

    private bool canInitiateSingleFreeze = true;  // Is the single freeze cooldown inactive?
    private bool canInitiateEnvironmentFreeze = true;  // Is the environment freeze cooldown inactive?
    SimpleTimeManipulation simpleObject = null;  // object with a simple freeze mechanism
    ComplexTimeHub complexObject = null;  // objecct with a complex freeze mechanism
    public static Action<TimeEffect, float, float> freezeAllComplexObjects;  // event container for freezing every freezeable object


    /// <summary>
    /// Freezes a single object or the environment depending on Player input and the ability to freeze a specific object(s).
    /// </summary>
    private void Update()
    {
        // If the Player presses the freeze single object button and the corresponding cooldown is inactive, attempt to freeze a single object.
        if (Input.GetKeyDown(freezeSingleObjectButton) && canInitiateSingleFreeze)
        {
            // Set the ray's starting position and direction.
            startRayPosition = playerCamera.position;
            rayDirection = playerCamera.TransformDirection(Vector3.forward);

            // Cast the ray until the ray does not hit the Player or maxRayCasts has been reached.
            for (int i = 0; i < maxRayCasts; i++)
            {
                if (Physics.Raycast(startRayPosition, rayDirection, out rayHit))
                {
                    // If the ray hits the Player, re-assign the starting position to be a bit away from the hit position
                    // in the previous ray's direction and continue to the next loop iteration.
                    if (rayHit.transform.gameObject.CompareTag("Player"))
                    {
                        startRayPosition = rayHit.point + (rayDirection.normalized * rayPositionOffset);
                        continue;
                    }

                    // If the ray does not hit the Player, attempt to detect an object that can be frozen.
                    simpleObject = rayHit.transform.GetComponent<SimpleTimeManipulation>();
                    complexObject = rayHit.transform.GetComponent<ComplexTimeHub>();

                    // If the ray hits an object that can be frozen, then freeze the object, activate the freeze single object cooldown, and stop casting rays.
                    if (simpleObject != null)
                    {
                        simpleObject.ActivateSingleObjectEffect(freezeSingleTime, TimeEffect.Freeze);
                        StartCoroutine(ActivateSingleCooldown());
                        StartCoroutine(CountdownSimpleFreeze(simpleObject));
                        return;
                    }
                    if (complexObject != null)
                    {
                        // If the object does not possess a freeze script, then do not activate cooldown.
                        if (complexObject.transform.GetComponent<ComplexFreeze>() == null)
                        {
                            return;
                        }
                        complexObject.AffectObject(TimeEffect.Freeze, freezeSingleTime, 0f);
                        StartCoroutine(ActivateSingleCooldown());
                        StartCoroutine(CountdownSimpleFreeze(simpleObject));
                        return;
                    }
                    // If the ray hits nothing, stop casting rays.
                    // FOR TESTING PURPOSES, comment this "else" block out.
                    else
                    {
                        return;
                    }
                }
            }

            // FOR TESTING PURPOSES, remove the comments for the block below.
            /*
             if (rayHit.transform != null)
                Debug.Log(rayHit.transform.name);
             else
                Debug.Log("N/A");
            */
        }

        // If the Player presses the freeze environment button and the corresponding cooldown is inactive, attempt to freeze all freezeable objects.
        if (Input.GetKeyDown(freezeEnvironmentButton) && canInitiateEnvironmentFreeze)
        {
            // If there are freezeable objects existing in the scene, then freeze all of them and activate the freeze environment cooldown.
            MasterTime.singleton.UpdateTime((int)TimeEffect.Freeze);
            if (freezeAllComplexObjects != null) freezeAllComplexObjects(TimeEffect.Freeze, freezeEnvironmentTime, 0);
            StartCoroutine(ActivateEnvironmentCooldown());
            StartCoroutine(CountdownEnvironmentFreeze());
        }
    }

    /// <summary>
    /// Updates the simple object's timescale to the default value after the single active time passes.
    /// </summary>
    /// <param name="simpleObject"> object with a simple freeze mechanism </param>
    private IEnumerator CountdownSimpleFreeze(SimpleTimeManipulation simpleObject)
    {
        float timer = freezeSingleTime;
        while (timer > 0)
        {
            localActiveTimerText.text = timer.ToString("0.00") + "";
            timer -= Time.deltaTime;
            yield return null;
        }
        localActiveTimerText.text = "";
        if (simpleObject != null) simpleObject.UpdateWithGlobalTimescale(1f);
    }

    /// <summary>
    /// Updates every simple object's timescale to the default value after the environment active time passes.
    /// </summary>
    private IEnumerator CountdownEnvironmentFreeze()
    {
        float timer = freezeEnvironmentTime;
        while (timer > 0)
        {
            globalActiveTimerText.text = timer.ToString("0.00") + "";
            timer -= Time.deltaTime;
            yield return null;
        }
        globalActiveTimerText.text = "";
        MasterTime.singleton.UpdateTime((int)TimeEffect.None);
    }

    /// <summary>
    /// Denies the Player from freezing an object throughout the freeze single cooldown.
    /// </summary>
    private IEnumerator ActivateSingleCooldown()
    {
        canInitiateSingleFreeze = false;
        float timer = freezeSingleCooldown;
        while (timer > 0)
        {
            localCooldownTimerText.text = timer.ToString("0.00") + "";
            timer -= Time.deltaTime;
            yield return null;
        }
        localCooldownTimerText.text = "";
        canInitiateSingleFreeze = true;
    }

    /// <summary>
    /// Denies the Player from freezing the environment throughout the freeze environment cooldown.
    /// </summary>
    private IEnumerator ActivateEnvironmentCooldown()
    {
        canInitiateEnvironmentFreeze = false;
        float timer = freezeEnvironmentCooldown;
        while (timer > 0)
        {
            globalCooldownTimerText.text = timer.ToString("0.00") + "";
            timer -= Time.deltaTime;
            yield return null;
        }
        globalCooldownTimerText.text = "";
        canInitiateEnvironmentFreeze = true;
    }
}
