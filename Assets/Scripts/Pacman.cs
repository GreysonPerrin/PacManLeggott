//////////////////////////////////////////////
//Assignment/Lab/Project: 3D PacMan
//Name: Shelby Leggott
//Section: SGD285.4171
//Instructor: Aurore Locklear
//Date: 01/21/2024
/////////////////////////////////////////////

// Pacman Script: Used for implementing all pacman user input code and other functions

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacman : MonoBehaviour
{
   public Movement movement { get; private set; }

    private void Awake()
    {
        this.movement = GetComponent<Movement>();
    }

    // Used for checking user inputs
    private void Update()
    {
        // Player inputs for both WASD or Arrow Keys
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            movement.SetDirection(new Vector3(0, 0, 1));
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            gameObject.transform.rotation = Quaternion.Euler(0, -90, 0);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            movement.SetDirection(new Vector3(-1, 0, 0));
            gameObject.transform.localScale = new Vector3(-1, 1, 1);
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            movement.SetDirection(new Vector3(0, 0, -1));
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            gameObject.transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            movement.SetDirection(new Vector3(1, 0, 0));
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    // resets the state of pacman on the start of a new round or wave
    public void ResetState()
    {
        gameObject.SetActive(true);
        movement.ResetGameobjectState();
        gameObject.transform.localScale = new Vector3(1, 1, 1);
        gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
