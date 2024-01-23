//////////////////////////////////////////////
//Assignment/Lab/Project: 3D PacMan
//Name: Shelby Leggott
//Section: SGD285.4171
//Instructor: Aurore Locklear
//Date: 01/21/2024
/////////////////////////////////////////////

// Ghost Script: Manages the script references for ghost behavior, state, and movement

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public Movement movement { get; private set; }
    public GhostBehavior behaviorScript;

    public Vector3 target; // target will vary between pacman and a scatter target

    private void Awake()
    {
        movement = GetComponent<Movement>();
        behaviorScript = GetComponent<GhostBehavior>();
    }

    private void Start()
    {
        ResetState(); // called at the start of a game
    }

    // resets the state of this ghost on the start of a new round or wave
    public void ResetState()
    {
        gameObject.SetActive(true);
        movement.ResetGameobjectState();
        behaviorScript.currentBehavior = behaviorScript.startingBehavior;
    }

    // ghosts will destroy pacman on collision
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {
            // You can create an if check for if the ghost is in frightened state from the GhostBehavior script here!!! :)
            FindObjectOfType<GameManager>().PacmanEaten();
        }
    }
}
