using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGameobject : MonoBehaviour
{
    AudioManager audioManager;

    private void Start()
    {
        audioManager = GameManager.instance.gameObject.GetComponent<AudioManager>();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void PlayPlunchSFX()
    {
        //audioManager = GameManager.instance.gameObject.GetComponent<AudioManager>();
        //audioManager.PlayPlunchSFX();
    }
}
