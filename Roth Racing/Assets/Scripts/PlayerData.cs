using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public static PlayerData instance;

    private AudioManager audioManager;

    #region MOVEMENT
    public bool canMove = true;
    public float moveSpeed = 140f;
    #endregion

    #region LIFE
    public int maxLife = 3;
    public int currLife;
    #endregion

    #region OFFENSE
    public int currAmmo = 3;
    public int projectileDamage = 50;
    public float attackCooldown = 1f;
    #endregion

    

    public void PickupAmmo()
    {
        currAmmo++;
        GameManager.instance.UpdatePlayerAmmoHUD();

        if (audioManager == null)
            audioManager = GameManager.instance.gameObject.GetComponent<AudioManager>();

        audioManager.PlayPickupAmmoSFX();
    }
}
