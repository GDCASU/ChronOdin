using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressButton : MonoBehaviour, InteractiveObject
{
    public GameObject[] gameObjects;
    public FMODPlay3DSoundEffect effect;
    public bool pressed;

    // InteractiveObject carries a generic type because NotePickup.cs needed it.
    public void Interact()
    {
        effect.PlaySoundEffect();
        foreach (GameObject obj in gameObjects)
        {
            if (!pressed) obj.GetComponent<LinkedToPressButton>().Increment();
            else obj.GetComponent<LinkedToPressButton>().Decrement();
        }
        pressed = !pressed;
    }
}
