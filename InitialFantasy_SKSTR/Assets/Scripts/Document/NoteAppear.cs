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

    public TextMeshProUGUI  documentBodyText;
    

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            documentImage.enabled = true;
            documentTitleText.enabled = true;
            documentBodyText.enabled = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            documentImage.enabled = false;
            documentTitleText.enabled = false;
            documentBodyText.enabled = false;

        }
    }
}
