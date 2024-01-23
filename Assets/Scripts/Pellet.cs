//////////////////////////////////////////////
//Assignment/Lab/Project: 3D PacMan
//Name: Shelby Leggott
//Section: SGD285.4171
//Instructor: Aurore Locklear
//Date: 01/21/2024
/////////////////////////////////////////////

// Pellet Script: Controls how the pellets interact with pacman

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pellet : MonoBehaviour
{
    public int points = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {
            Eaten();
        }
    }

    private void Eaten()
    {
        FindObjectOfType<GameManager>().PelletEaten(this);
    }
}
