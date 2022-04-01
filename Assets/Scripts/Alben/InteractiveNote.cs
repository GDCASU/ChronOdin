using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The player can pick up this note to see its contents.
/// Author: Alben Trang
/// </summary>
public class InteractiveNote : MonoBehaviour, InteractiveObject
{
    [Tooltip("This is what's written on the note.")] [TextArea(15, 20)] public string noteContent = "";

    /// <summary>
    /// Get the contents of the note object.
    /// </summary>
    /// <returns>The contents of the note.</returns>
    public void Interact()
    {
        PauseMenu.singleton.DisplayNote(noteContent);
    }
}
