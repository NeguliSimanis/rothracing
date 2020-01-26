using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    AudioClip seaShanty;

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
}
