using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This singleton controls the Note Panel's text on the UI.
/// Author: Alben Trang
/// </summary>
public class NotesManager : MonoBehaviour
{
    [HideInInspector] public static NotesManager singleton;

    // Grab the elements from the Notes Panel
    private Image noteImage;
    private Text noteText;
    private Text closeNoteText;

    /// <summary>
    /// Initialize the NoteSingleton singleton.
    /// </summary>
    private void Awake()
    {
        if (singleton == null)
            singleton = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Set the text of the Note panel to show what's on the interactable notes.
    /// </summary>
    /// <param name="message">What's written on the notes.</param>
    public void SetNoteUIText(string message) => noteText.text = message;
    

    /// <summary>
    /// Pressing the 'Interaction' button when the player isn't looking at anything
    /// closes/opens the notes they currently have.
    /// </summary>
    /// <param name="toggle">Set whether or not the notes should show on the UI.</param>
    public void setNoteVisibility(bool toggle)
    {
        if (toggle)
        {
            noteImage.color = new Color(noteImage.color.r, noteImage.color.g, noteImage.color.b, 1.0f);
            //noteText.text = currentContents;
            closeNoteText.text = "(Press 'Interaction' button without pointing at anything to close/open notes)";
        }
        else
        {
            noteImage.color = new Color(noteImage.color.r, noteImage.color.g, noteImage.color.b, 0.0f);
            noteText.text = "";
            closeNoteText.text = "";
        }
    }
}
