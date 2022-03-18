using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager singleton;
    public TimeEffect currentAbility;

    SimpleTimeManipulation simpleObject = null;  // simple time object
    ComplexTimeHub complexObject = null;  // complex time object

    private bool globalBuffer;
    private bool currentGlobal;
    private bool localBuffer;
    private bool currentLocal;
    [HideInInspector] public bool environmentEffectActive;

    public bool canSlow = true;
    public bool canFreeze = true;
    public bool canReverse = true;

    private void Awake()
    {
        if (singleton == null)
            singleton = this;
        else
            Destroy(gameObject);
    }
    void Update()
    {
        if (canFreeze && InputManager.GetButtonDown(PlayerInput.PlayerButton.Freeze)) currentAbility = TimeEffect.Freeze;
        else if (canSlow && InputManager.GetButtonDown(PlayerInput.PlayerButton.Slow)) currentAbility = TimeEffect.Slow;
        else if (canReverse && InputManager.GetButtonDown(PlayerInput.PlayerButton.Reverse)) currentAbility = TimeEffect.Reverse;

        globalBuffer = InputManager.GetButton(PlayerInput.PlayerButton.Global);

        localBuffer = InputManager.GetButtonDown(PlayerInput.PlayerButton.Local);
        

        if (InputManager.inputMode == InputManager.InputMode.controller || InputManager.inputMode == InputManager.InputMode.both)
        {
            globalBuffer = (!globalBuffer)? (InputManager.GetAxis(PlayerInput.PlayerAxis.RightTrigger)!=0 ? true : false) : globalBuffer;
            localBuffer = (!localBuffer)? (InputManager.GetAxis(PlayerInput.PlayerAxis.LeftTrigger)!=0 ? true : false) : localBuffer;
        }
        if (localBuffer!=currentLocal && localBuffer)
        {
            // If the environment is undergoing a time effect, do not attempt to freeze a single object.
            if (MasterTime.singleton.timeScale != 1f)
                return;

            // Acquire the transform of the object observed by the Player. If a transform is not acquired, do nothing.
            Transform someObject = PlayerInteractions.singleton.RaycastTransform();
            if (someObject == null)
                return;

            simpleObject = someObject.GetComponent<SimpleTimeManipulation>() ?? someObject.GetComponentInParent<SimpleTimeManipulation>();
            complexObject = someObject.transform.GetComponent<ComplexTimeHub>() ?? someObject.GetComponentInParent<ComplexTimeHub>();

            if (simpleObject != null)
            {
                switch (currentAbility)
                {
                    case TimeEffect.Freeze:
                        FreezeInvocation.singleton.SimpleObjectFreeze(simpleObject);
                        break;
                    case TimeEffect.Slow:
                        SlowInvocation.singleton.SimpleObjectSlow(simpleObject);
                        break;
                    case TimeEffect.Reverse:
                        ReverseInvocation.singleton.SimpleObjectReverse(simpleObject);
                        break;
                }
            }
            else if (complexObject != null)
            {
                switch (currentAbility)
                {
                    case TimeEffect.Freeze:
                        FreezeInvocation.singleton.ComplexObjectFreeze(complexObject);
                        break;
                    case TimeEffect.Slow:
                        SlowInvocation.singleton.ComplexObjectSlow(complexObject);
                        break;
                    case TimeEffect.Reverse:
                        ReverseInvocation.singleton.ComplexObjectReverse(complexObject);
                        break;
                }
            }
        }
        else if (globalBuffer!=currentGlobal)
        {
            switch (currentAbility)
            {
                case TimeEffect.Freeze:
                    FreezeInvocation.singleton.EnvironmentFreeze();
                    break;
                case TimeEffect.Slow:
                    SlowInvocation.singleton.EnvironmentSlow();
                    break;
                case TimeEffect.Reverse:
                    ReverseInvocation.singleton.PlayerReverse();
                    break;
            }
        }
        currentGlobal = globalBuffer;
        currentLocal = localBuffer;
    }

    public void ToggleEnvironment(bool value)
    {
        environmentEffectActive = value;
    }
    /// <summary>
    /// Changes the status for a player ability
    /// </summary>
    /// <param name="effect"> the player ability (TimeEffect) that will be changed </param>
    /// /// <param name="enabled"> sets the player ability to be enabled or disabled</param>
    public void SetAbilityStatus(TimeEffect effect ,bool enabled)
    { 
        switch(effect)
        {
            case TimeEffect.Slow:
                canSlow = enabled;
                break;
            case TimeEffect.Freeze:
                canFreeze = enabled;
                break;
            case TimeEffect.Reverse:
                canReverse = enabled;
                break;
            default:
                break;
                
        }
    }
}
