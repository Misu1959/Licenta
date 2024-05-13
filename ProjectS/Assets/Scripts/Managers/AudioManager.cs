using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource_Music;
    [SerializeField] private AudioSource audioSource_Sounds;

    [SerializeField] private AudioClip backgroundMusic;

    [SerializeField] private AudioClip[] sounds;

    void Start()
    {
        audioSource_Music.clip = backgroundMusic;
        audioSource_Music.volume = audioSource_Music.maxDistance;

        audioSource_Music.Play();        
    }

}
