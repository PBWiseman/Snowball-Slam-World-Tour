using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SnowballCollision : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy")) //If the snowball hits an enemy
        {
            LevelManager.instance.UpdateScore("Player"); //Update the player's score 
        }
        else if (collision.gameObject.CompareTag("Player")) //If the snowball hits the player
        {
            LevelManager.instance.UpdateScore("Enemy"); //Update the enemy's score
        }

        Destroy(gameObject); //destroys itself no matter what it hits, snowball or border

    }
}
