using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This singleton controls the Note Panel's text on the UI.
/// Author: Alben Trang
/// </summary>
public class NoteSingleton : MonoBehaviour
{
    [HideInInspector] public static NoteSingleton noteSingleton;

    // Grab the elements from the Notes Panel
    private Image noteImage;
    private Text noteText;
    private Text closeNoteText;
    private string currentContents;

    /// <summary>
    /// Initialize the NoteSingleton singleton.
    /// </summary>
    private void Awake()
    {
        if (noteSingleton == null)
            noteSingleton = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Initialize the variables and their attributes to start the Notes Panel to be invisible.
    /// </summary>
    private void Start()
    {
        noteImage = this.GetComponent<Image>();
        noteText = this.transform.GetChild(0).GetComponent<Text>();
        closeNoteText = this.transform.GetChild(1).GetComponent<Text>();

        noteImage.color = new Color(noteImage.color.r, noteImage.color.g, noteImage.color.b, 0.0f);
        currentContents = "";
        noteText.text = currentContents;
        noteImage.raycastTarget = false;
        noteText.raycastTarget = false;
    }

    /// <summary>
    /// Set the text of the Note panel to show what's on the interactable notes.
    /// </summary>
    /// <param name="contents">What's written on the notes.</param>
    public void setNoteContents(string contents) 
    {
        currentContents = contents;
        noteText.text = currentContents;
    }

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
            noteText.text = currentContents;
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
