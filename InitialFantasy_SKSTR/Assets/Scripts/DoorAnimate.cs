using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimate : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    void Awake()
    {
        //Make Collider2D as trigger 
        GetComponent<Collider2D>().isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other) {
        // Debug.Log("trigger touched");
        if(other.CompareTag("Player")) {
            animator.SetBool("door_open", true);

        }
    }
    void OnTriggerExit2D(Collider2D other) {
        animator.SetBool("door_open", false);
    
    }



}