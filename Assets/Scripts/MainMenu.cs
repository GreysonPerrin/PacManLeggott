//////////////////////////////////////////////
//Assignment/Lab/Project: 3D PacMan
//Name: Shelby Leggott
//Section: SGD285.4171
//Instructor: Aurore Locklear
//Date: 01/21/2024
/////////////////////////////////////////////

// MainMenu Script: Controls the main menu

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && !(Input.GetKeyDown(KeyCode.Escape)))
        {
            SceneManager.LoadScene(1); // load game scene
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit(); // quit game
        }
    }
}
