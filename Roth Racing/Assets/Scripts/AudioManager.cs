using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    AudioClip seaShanty;


    #region SETTINGS
    private bool allowSFXs = true;
    #endregion

    #region PLAYER SFXS
    [Header("PLAYER SFX")]
    [SerializeField]
    AudioClip cannonSFX;
    private float cannonVolume = 0.3f;
    private bool allowNextPlayerCannonSFX = true;
    private float sfxCooldown = 0.5f;

    [SerializeField]
    AudioClip enemyKilledSFX;
    private int chanceToPlayEnemyKillSFX = 100;
    private float enemyKilledSFXVolume = 1f;
    private bool allowEnemyKilledSFX = true;

    [SerializeField]
    AudioClip shipDamagedSFX;
    private float shipDamagedSFXVolume = 0.2f;
    private bool allowShipDamagedSFX = true;
    #endregion

    #region PICKUP SFXs
    [Header("PICKUP SFX")]
    [SerializeField]
    AudioClip ammoPickupSFX;
    private float ammoPickupSFXVolume = 1f;

    [SerializeField]
    AudioClip projectileDisappearSFX;
    private float projectileDisappearVolumePlayer = 0.7f;
    private float projectileDisappearVolumeEnemy = 0.3f;
    #endregion

    AudioSource audioSource;
    private void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.Stop();
    }

    public void PlaySeaShanty()
    {
        audioSource.Play();
    }
    #region BOTH ENEMY AND PLAYER SFX
    public void PlayShipDamagedSFX()
    {
        if (!allowSFXs)
            return;
        if (!allowShipDamagedSFX)
            return;
        audioSource.PlayOneShot(shipDamagedSFX, shipDamagedSFXVolume);
        allowShipDamagedSFX = false;
        StartCoroutine(AllowShipDamagedSFXs());
    }


    private IEnumerator AllowShipDamagedSFXs()
    {
        yield return new WaitForSeconds(sfxCooldown);
        allowShipDamagedSFX = true;
    }

    public void PlayPlunchSFX(bool isPlayerPlunch)
    {
        if (isPlayerPlunch)
            audioSource.PlayOneShot(projectileDisappearSFX, projectileDisappearVolumePlayer);
        else
            audioSource.PlayOneShot(projectileDisappearSFX, projectileDisappearVolumeEnemy);
    }
    #endregion
    #region PLAYER SFX METHODS
    public void PlayPlayerCannonSFX()
    {
        if (!allowSFXs)
            return;
        if (!allowNextPlayerCannonSFX)
            return;
        allowNextPlayerCannonSFX = false;
        audioSource.PlayOneShot(cannonSFX, cannonVolume);
        StartCoroutine(AllowPlayerCannonSFXs());
    }

    public void PlayEnemyKilledSFX()
    {
        if (!allowSFXs)
            return;
        if (!allowEnemyKilledSFX)
            return;
        allowEnemyKilledSFX = false;
        if (Random.Range(0,101) < chanceToPlayEnemyKillSFX)
        {
            audioSource.PlayOneShot(enemyKilledSFX, enemyKilledSFXVolume);
        }
        StartCoroutine(AllowEnemyKilledSFXs());
    }

    private IEnumerator AllowPlayerCannonSFXs()
    {
        yield return new WaitForSeconds(sfxCooldown);
        allowNextPlayerCannonSFX = true;
    }

    private IEnumerator AllowEnemyKilledSFXs()
    {
        yield return new WaitForSeconds(sfxCooldown);
        allowEnemyKilledSFX = true;
    }
    #endregion

    #region PICKUP SFX METH
    public void PlayPickupAmmoSFX()
    {
        audioSource.PlayOneShot(ammoPickupSFX, ammoPickupSFXVolume);
    }
    #endregion
}
