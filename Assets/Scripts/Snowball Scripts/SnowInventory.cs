using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the player's snowball inventory. Stored on each player.
/// </summary>
public class SnowInventory : MonoBehaviour
{
    public int currentAmmo = 5;
    private string name;

    void Start()
    {
        name = gameObject.name; // for debugging purposes
    }

    /// <summary>
    /// Decreases the amount of ammo the player has.
    /// </summary>
    public void DecreaseAmmo()
    {
        currentAmmo--;
        Debug.Log($"{name} Snowballs: {currentAmmo}");
    }
}
