using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressButton : MonoBehaviour, InteractiveObject
{
    public GameObject buttonDoor;
    public FMODPlay3DSoundEffect effect;
    public bool pressed;
    PressButtonDoor linkedDoor;
    private void Start()
    {
        linkedDoor = buttonDoor.GetComponent<PressButtonDoor>();
    }

    public void Interact()
    {
        if (linkedDoor != null && !linkedDoor.isMoving)
        {
            effect.PlaySoundEffect();
            if (!pressed)
            {
                linkedDoor.Increment();
                pressed = true;
            } 
        }
    }
}
