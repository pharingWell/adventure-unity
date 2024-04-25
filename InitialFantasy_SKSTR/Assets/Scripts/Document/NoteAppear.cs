using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* REFERENCE:
Title: “Unity3D: Create a Note Appear System” [YouTube Video]
Author: Kyle Broad
Date: October, 19th, 2020
URL: https://youtube.com/watch?v=eVre2i6gPF0 
Publisher: YouTube
Used for: Document System (Scripts: NoteAppear) 
*/
public class NoteAppear : MonoBehaviour
{
    [SerializeField]
    private Image documentImage;
    public TextMeshProUGUI documentTitleText;
    public TextMeshProUGUI documentBodyText;
    public TextMeshProUGUI openText;
    public TextMeshProUGUI closeText;

    private bool openAllowed;
    private bool isDocumentOpen = false;

    private void Update() {
        if (openAllowed && Input.GetKeyDown(KeyCode.E)) {
            if (isDocumentOpen) {
                Close();
            } else {
                Open();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            openText.enabled = true;
            openText.text = "Press E to open document";
            openAllowed = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            openText.enabled = false;
            openAllowed = false;
            if (isDocumentOpen) {
                Close();
            }
        }
    }

    void Open() {
        documentImage.enabled = true;
        documentTitleText.enabled = true;
        documentBodyText.enabled = true;

        openText.enabled = false;  // Disable the "open" text
        closeText.enabled = true;
        closeText.text = "Press E to close document"; // Set close text

        isDocumentOpen = true;
    }

    void Close() {
        documentImage.enabled = false;
        documentTitleText.enabled = false;
        documentBodyText.enabled = false;

        closeText.enabled = false; // Disable the close text
        openText.enabled = true;
        //openText.text = "Press E to open document"; // Reset open text

        isDocumentOpen = false;
    }
}

