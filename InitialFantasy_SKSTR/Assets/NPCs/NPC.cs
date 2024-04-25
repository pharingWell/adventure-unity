using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPC : MonoBehaviour
{
    
    public GameObject button; 
    public Button buttonx;

    void Start() {
        button.SetActive(false);  // Deactivate the button at the start
        buttonx.onClick.AddListener(HideButton);
    }
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            button.SetActive(true);
        }
     }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            button.SetActive(false);

        }
    }
void HideButton()
    {
        buttonx.gameObject.SetActive(false); 
    }


}
