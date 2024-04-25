using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
