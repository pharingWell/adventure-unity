using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScript : MonoBehaviour
//Will need to duplicate and rename this script for every NPC
// We can use an interface - Seb
{
    public string[] dialogueLines; // The lines this NPC will say
    public DialogueSystem dialogueSystem; // Reference to the DialogueSystem

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            TriggerDialogue();
        }
    }

    void TriggerDialogue()
    {
        dialogueSystem.StartDialogue(dialogueLines);
    }
}