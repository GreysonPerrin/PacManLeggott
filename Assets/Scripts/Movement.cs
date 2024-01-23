//////////////////////////////////////////////
//Assignment/Lab/Project: 3D PacMan
//Name: Shelby Leggott
//Section: SGD285.4171
//Instructor: Aurore Locklear
//Date: 01/21/2024
/////////////////////////////////////////////

// Movement Script: Generalize script that is used for both pacman and the ghosts

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public new Rigidbody rigidbody { get; private set; }

    public float speed = 8.0f;
    public float speedMultiplier = 1.0f;
    public Vector3 initialDirection;
    public LayerMask obstacleLayer; // only check this layer on raycasts
    public LayerMask ghostDoorLayer;

    public Vector3 direction { get; private set; }
    public Vector3 nextDirection { get; private set; } // allows the player to queue up their next movement (would require very precise reflex speed otherwise)
    public Vector3 startingPosition { get; private set; } // reset pacman to this position on game state reset

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        startingPosition = transform.position;
    }

    private void Start()
    {
        ResetGameobjectState();
    }

    // Function for resetting the state of that gameobject
    public void ResetGameobjectState()
    {
        speedMultiplier = 1.0f;
        direction = initialDirection;
        nextDirection = Vector3.zero;
        transform.position = startingPosition;
        rigidbody.isKinematic = false; // used for ghosts so they can exit the ghost home
        this.enabled = true;
    }

    private void Update()
    {
        if (this.nextDirection != Vector3.zero)
        {
            SetDirection(this.nextDirection);
        }
    }

    // This is where the movement code will be implemented so the game is not frame dependent
    private void FixedUpdate()
    {
        Vector3 position = rigidbody.position;
        Vector3 translatePos = direction * speed * speedMultiplier * Time.fixedDeltaTime;

        rigidbody.MovePosition(position + translatePos);
    }

    // Function for changing the direction to move the gameobject
    public void SetDirection(Vector3 direction, bool forcedDir = false)
    {
        // change directions when the space is not occupied and queue up next direction if the space is occupied
        if (forcedDir || !IsSpaceOccupied(direction))
        {
            this.direction = direction;
            nextDirection = Vector3.zero;
        }
        else
        {
            nextDirection = direction;
        }
    }

    // Function that returns a bool to check if the space in the direction the gameobject is trying to go is occupied already (or blocked by a wall)
    public bool IsSpaceOccupied(Vector3 direction)
    {
        Quaternion orientation = Quaternion.identity;
        RaycastHit hit;
        if (Physics.BoxCast(this.transform.position, Vector3.one * 0.25f, direction, out hit, orientation, 1.5f, obstacleLayer | ghostDoorLayer))
        {
            return hit.collider != null;
        }
        return false;
    }
}
