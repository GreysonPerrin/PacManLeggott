//////////////////////////////////////////////
//Assignment/Lab/Project: 3D PacMan
//Name: Shelby Leggott, Greyson Perrin
//Section: SGD285.4171
//Instructor: Aurore Locklear
//Date: 01/28/2024
/////////////////////////////////////////////

// GhostBehavior Script: Manages the behaviors for the ghosts (currently has HOME, SCATTER, FRIGHTENED and CHASE) - MISSING EATEN STATE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GhostBehavior : MonoBehaviour
{
    public Ghost ghost { get; private set; }
    public GameManager gameManager;

    private SphereCollider ghostCollider;

    public LayerMask obstacleLayer; // assigned in editor
    public LayerMask ghostDoorLayer;
    public LayerMask ghostLayer;

    private Vector3 position;

    [SerializeField] private Transform scatterTransform; // assigned in editor
    [SerializeField] private Transform homeTransform; // assigned in editor
    [SerializeField] private Transform homeExitTransform; // assigned in editor

    private int exitPriority = 0;
    private int level1PelletLimit;
    private int level2PelletLimit;
    private int level3PlusPelletLimit = 0;
    private int postDeathPelletLimit;

    // Enum to dictate initial game behavior, assigned to each ghost in editor
    public enum CurrentBehavior
    {
        Home,
        Scatter,
        Chase,
        Frightened,
        LeavingHome
    }

    public CurrentBehavior startingBehavior;
    public CurrentBehavior currentBehavior;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        ghost = GetComponent<Ghost>();
        ghostCollider = GetComponent<SphereCollider>();
        
    }

    private void Start()
    {
        position = ghost.transform.position;
        
        // perform these checks for each ghost and run at start of game
        if (this.gameObject.tag == "Blinky")
        {
            startingBehavior = CurrentBehavior.LeavingHome;
            level1PelletLimit = 0;
            level2PelletLimit = 0;
            postDeathPelletLimit = 0;
            this.exitPriority = 0;
            currentBehavior = startingBehavior;
        }
        if (this.gameObject.tag == "Pinky")
        {
            startingBehavior = CurrentBehavior.Home;
            level1PelletLimit = 0;
            level2PelletLimit = 0;
            postDeathPelletLimit = 7;
            this.exitPriority = 1;
            currentBehavior = startingBehavior;
        }
        if (this.gameObject.tag == "Inky")
        {
            startingBehavior = CurrentBehavior.Home;
            level1PelletLimit = 30;
            level2PelletLimit = 0;
            postDeathPelletLimit = 17;
            this.exitPriority = 2;
            currentBehavior = startingBehavior;
        }
        if (this.gameObject.tag == "Clyde")
        {
            startingBehavior = CurrentBehavior.Home;
            level1PelletLimit = 60;
            level2PelletLimit = 50;
            postDeathPelletLimit = 32;
            this.exitPriority = 3;
            currentBehavior = startingBehavior;
        }
    }

    private void Update()
    {
        // Set the ghost behavior to either Home, Scatter, Flee, or Chase (ghosts start in chase in later rounds of the game but that is not implemented)
        if (this.currentBehavior == CurrentBehavior.Home)
        {
            Home();
        }
        if (this.currentBehavior == CurrentBehavior.Scatter)
        {
            Scatter();
        }
        if (this.currentBehavior == CurrentBehavior.Chase)
        {
            Chase();
        }
        if (this.currentBehavior == CurrentBehavior.Frightened)
        {
            Frightened();
        }
        if (this.currentBehavior == CurrentBehavior.LeavingHome)
        {
            LeavingHome();
        }
    }

    private void Scatter()
    {
        ghostCollider.excludeLayers = ghostLayer;

        ghost.target = scatterTransform.position;
        ChooseDirection();
    }

    private void Chase()
    {
        ghostCollider.excludeLayers = ghostLayer;

        // each ghost has a unique approach to chasing pacman:
        // Blinky: Target is directly pacman's position
        // Pinky: Target is 4 tiles ahead of pacman's position
        // Inky: Target is created in 3 steps -
        // - Intermediate point is created two tiles ahead of pacman,
        // - Vector is created between blinky and that intermediate point,
        // - Vector is flipped 180 degrees and the end point is Inky's position
        // Clyde: Target is pacman's position if Clyde is 8 tiles or farther away from pacman, if less than 8 tiles, target becomes Clyde's scatter target

        // ensure that pacman can be found in the level, then set pacman as the ghost's target during chase mode (SIMPLIFIED VERSION OF WHAT EACH GHOST DOES, READ DETAILS ABOVE)
        if (FindObjectOfType<Pacman>() != null)
        {
            Pacman pacman = FindObjectOfType<Pacman>();
            // BLINKY
            if (gameObject.tag == "Blinky")
            {
                ghost.target = pacman.transform.position;
            }
            // PINKY
            if (gameObject.tag == "Pinky")
            {
                Vector3 pacmanDirection = pacman.movement.direction * 4f;

                ghost.target = pacman.transform.position + pacmanDirection;
            }
            // INKY
            if (gameObject.tag == "Inky")
            {
                // Store the intermediate point
                Vector3 pacmanDirection = pacman.movement.direction * 2f;

                // Create a vector between the intermediate point and Blinky
                if (GameObject.FindGameObjectWithTag("Blinky") != null)
                {
                    GameObject blinky = GameObject.FindGameObjectWithTag("Blinky");

                    Vector3 dist = blinky.transform.position - (pacman.transform.position + pacmanDirection);

                    ghost.target = dist + blinky.transform.position;
                }
            }
            // CLYDE
            if (gameObject.tag == "Clyde")
            {
                float dist = Vector3.Distance(this.transform.position, pacman.transform.position);

                // Clyde moves towards pacman if 8 or more tiles away, else clyde goes towards their scatter target
                if (dist >= 8f)
                {
                    ghost.target = pacman.transform.position;
                }
                else
                {
                    ghost.target = scatterTransform.transform.position;
                }
            }
        }
        ChooseDirection();
    }

    private void Frightened()
    {
        ghostCollider.excludeLayers = ghostLayer;

        ghost.target = FindObjectOfType<Pacman>().transform.position;
        ChooseDirection();
    }

    // Function is called to both return ghosts to their home as well as retain them within the home until it is time for them to leave the ghost home
    private void Home()
    {
        ghostCollider.excludeLayers = ghostDoorLayer;

        if (this.ghost.transform.position != homeTransform.position)
        {
            this.ghost.target = homeTransform.position;
            ChooseDirection();
            // check if it is this ghosts time to keep count of pellets in order to decide when to leave the ghost house (ghost will immediately leave the ghost house if the priority is greater than their assigned priority number)
            if (gameManager != null && exitPriority >= gameManager.priority && gameManager.livesLostOnRound <= 0)
            {
                // choose which limit is needed based on current level of pacman, limit gets lower making the ghosts exit sooner on later levels
                switch (gameManager.levelCounter)
                {
                    case 1:
                    {
                        if (gameManager.pelletCounter >= level1PelletLimit)
                        {
                            this.currentBehavior = CurrentBehavior.LeavingHome;
                        }
                        break;
                    }
                    case 2:
                    {
                        if (gameManager.pelletCounter >= level2PelletLimit)
                        {
                            this.currentBehavior = CurrentBehavior.LeavingHome;
                        }
                        break;
                    }
                    case 3:
                    {
                        if (gameManager.pelletCounter >= level3PlusPelletLimit)
                        {
                            this.currentBehavior = CurrentBehavior.LeavingHome;
                        }
                        break;
                    }
                    default:
                    {
                        this.currentBehavior = CurrentBehavior.LeavingHome;
                        break;
                    }
                }
            }
            // ghosts operate differently after pacman has a death, but this is reset at the start of a new level
            else if (gameManager.livesLostOnRound >= 1)
            {
                if (gameManager.pelletCounter >= postDeathPelletLimit)
                {
                    this.currentBehavior = CurrentBehavior.LeavingHome;
                }
            }
        }
    }

    // Function is called as a transition method from Home to Scatter (or chase depending on level)
    private void LeavingHome()
    {
        ghostCollider.excludeLayers = ghostDoorLayer;

        this.ghost.target = homeExitTransform.position;

        float dist = Vector3.Distance(ghost.transform.position, ghost.target);

        if (dist <= 1f) 
        {
            gameManager.priority++; // increment the priority level
            gameManager.pelletCounter = 0; // reset the pellet counter for the next ghost
            this.currentBehavior = CurrentBehavior.Scatter;
        }
        ChooseDirection();
    }

    // Ghosts will procedurally decide which way to go based on the distance from their target, can be used for both scatter and chase
    private void ChooseDirection()
    {
        // List resets each time the function is called to ensure it does not tell the ghost that it can move in a direction that it cant
        List<Vector3> directionsToMove = new List<Vector3>();

        // distance from the original position to the new position the ghost has moved to
        float dist = Vector3.Distance(position, ghost.transform.position);

        Quaternion orientation = Quaternion.identity;
        RaycastHit hit;

        // call a box cast to see if the ghost has an obstacle in front of it but did not move the minimum distance (leading to them being stuck),
        // if obstacle in front, force the distance check through by making the distance a massively large number
        if ((Physics.BoxCast(new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + 0.65f), Vector3.one * 0.25f, ghost.movement.direction, out hit, orientation, 0.33f, obstacleLayer)))
        {
            dist = 1000f;
        }
        // checked 2 times per tile
        if (dist >= 0.5f)
        {
            // Check all four possible directions (forward backward left right)
            CheckDirection(new Vector3(-1, 0, 0), directionsToMove);
            CheckDirection(new Vector3(1, 0, 0), directionsToMove);
            CheckDirection(new Vector3(0, 0, -1), directionsToMove);
            CheckDirection(new Vector3(0, 0, 1), directionsToMove);
            
            Vector3 direction = new Vector3(1, 0, 0);
            float minDistance = float.MaxValue;
            float maxDistance = 0f;

            // Checks the distance from the target for each possible direction
            foreach (Vector3 possibleDirection in directionsToMove) 
            {
                // if going in a certain direction leads to a greater distance from the target, the ghost will not choose that direction to minimize time to get to target
                // if the ghost is in the frightened state, they instead choose the direction that leads to the greatest distance from the target(pac-man)
                Vector3 newPosition = ghost.transform.position + new Vector3(possibleDirection.x, 0.0f, possibleDirection.z);
                float distance = (ghost.target - newPosition).sqrMagnitude;

                if (this.currentBehavior == CurrentBehavior.Frightened)
                {
                    if (distance > maxDistance)
                    {
                        direction = possibleDirection;
                        maxDistance = distance; // set the new maximum distance to be the one just identified to allow to be checked with the remaining distances
                    }
                }
                else
                {
                    if (distance < minDistance)
                    {
                        direction = possibleDirection;
                        minDistance = distance; // set the new minimum distance to be the one just identified to allow to be checked with the remaining distances
                    }
                }
            }

            if (direction == new Vector3(-1, 0, 0))
            {

                Vector3 FaceLeft = new Vector3(-1.0f, 1.0f, 0.99f);
                ghost.transform.localScale = FaceLeft;
            }
            if (direction == new Vector3(1, 0, 0))
            {
                Vector3 FaceRight = new Vector3(1.0f, 1.0f, 0.99f);
                ghost.transform.localScale = FaceRight;
            }

            ghost.movement.SetDirection(direction, true);
            position = ghost.transform.position; // reset the position of the ghost to reset the distance from their previous spot
        }
    }

    // Function will shoot a raycast out in the vector3 provided to check if the space in that direction is occupied, if it is, that space cannot be moved in to
    private void CheckDirection(Vector3 direction, List<Vector3> possibleDirections)
    {
        Vector3 currentDir = ghost.movement.direction;

        Quaternion orientation = Quaternion.identity;
        RaycastHit hit;

        // if boxcast does not hit an obstacle in that vector direction, add it to the list of possible directions the ghost can move
        if ((currentBehavior == CurrentBehavior.Home || currentBehavior == CurrentBehavior.LeavingHome) && !(Physics.BoxCast(new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + 0.65f), Vector3.one * 0.25f, direction, out hit, orientation, 0.33f, obstacleLayer)))
        {
            // do not add backward or null directions (ghosts cannot move into walls nor can they move backwards)
            if (hit.collider == null && direction != -currentDir)
            {
                possibleDirections.Add(direction);
            }
        }
        else
        {
            if (!(Physics.BoxCast(new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + 0.65f), Vector3.one * 0.25f, direction, out hit, orientation, 0.33f, obstacleLayer | ghostDoorLayer)))
            {
                // do not add backward or null directions (ghosts cannot move into walls nor can they move backwards)
                if (hit.collider == null && direction != -currentDir)
                {
                    possibleDirections.Add(direction);
                }
            }
        }
    }
}