//////////////////////////////////////////////
//Assignment/Lab/Project: 3D PacMan
//Name: Greyson Perrin
//Section: SGD285.4171
//Instructor: Aurore Locklear
//Date: 01/28/2024
/////////////////////////////////////////////

// Power Pellet Script: Controls how the power pellets interact with pacman

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPellet : MonoBehaviour
{
    public float timeLimit = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {
            Eaten();
        }
    }

    private void Eaten()
    {
        FindObjectOfType<GameManager>().PowerPelletEaten(this);
    }
}
