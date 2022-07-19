using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalMusicManager : MonoBehaviour
{

    [SerializeField] AudioClip musicChoice1;

    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(musicChoice1);
        }        
    }
}
