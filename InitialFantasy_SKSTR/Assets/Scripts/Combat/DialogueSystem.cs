using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;
    public Button continueButton;

    private Queue<string> sentences; // Stores all sentences in the current dialogue

    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(string[] dialogueLines)
    {
        continueButton.gameObject.SetActive(true);
        dialoguePanel.SetActive(true); // Show the dialogue panel
        sentences.Clear();

        foreach (string sentence in dialogueLines)
        {
            sentences.Enqueue(sentence); // Add all lines to the queue
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue(); // Get the next line
        dialogueText.text = sentence; // Display it
    }

    void EndDialogue()
    {
        continueButton.gameObject.SetActive(false);
        dialoguePanel.SetActive(false); // Hide the dialogue panel
        // Here, we can trigger other events, like resuming gameplay? 
    }

    // This method is called by clicking the continue button
    public void OnContinueButton()
    {
        DisplayNextSentence();
    }
}
