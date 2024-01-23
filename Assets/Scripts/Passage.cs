//////////////////////////////////////////////
//Assignment/Lab/Project: 3D PacMan
//Name: Shelby Leggott
//Section: SGD285.4171
//Instructor: Aurore Locklear
//Date: 01/21/2024
/////////////////////////////////////////////

// Passage Script: Controls the event that occurs when pacman or ghosts enter a passage way

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passage : MonoBehaviour
{
    [SerializeField] private Transform teleportPoint;

    private void OnTriggerEnter(Collider other)
    {
        Vector3 pos = other.transform.position;
        pos.x = teleportPoint.position.x;
        pos.y = teleportPoint.position.y;
        other.transform.position = pos;
    }
}
