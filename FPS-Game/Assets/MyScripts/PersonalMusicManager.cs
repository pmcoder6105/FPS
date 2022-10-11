using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PersonalMusicManager : MonoBehaviour
{
    [SerializeField] AudioClip musicChoice;

    AudioSource audioSource;

    PhotonView PV;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        PV = GetComponentInParent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine == false)
            return;

        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(musicChoice);
        }        
    }
}
