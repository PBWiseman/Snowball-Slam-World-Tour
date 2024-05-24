using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class SnowballCollision : MonoBehaviour
{
    [HideInInspector]
    public string owner; //The owner of the snowball

    private bool isOtherSnowball;

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(owner))
        {
            if (collision.gameObject.GetComponent<ThrowSnowballs>() != null) //If the snowball hits another snowball
            {
                LevelManager.instance.UpdateScore("Player"); //Update the player's score
                ThrowSnowballs ts = collision.gameObject.GetComponent<ThrowSnowballs>();
                if (!ts.Invulnerable)
                {
                    if (collision.gameObject.CompareTag("Enemy")) //If the snowball hits an enemy
                    {
                        LevelManager.instance.UpdateScore("Player"); //Update the player's score
                    }
                    else if (collision.gameObject.CompareTag("Player")) //If the snowball hits the player
                    {
                        LevelManager.instance.UpdateScore("Enemy"); //Update the enemy's score
                    }
                    else if (collision.gameObject.CompareTag("Snowball")) //If the snowball hits another snowball
                    {
                        return; //snowballs ignore each other
                    }
                    ts.Invulnerable = true; //Makes the snowball invulnerable
                }
            }
            Destroy(gameObject); //destroys itself no matter what it hits, snowball or border
        }
    }
}