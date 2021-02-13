using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hardened : AbstractPowerUp
{
    [SerializeField] private AudioSource audioSource;
    public GameObject shieldObject;

    public void PlaySound() {
        audioSource.clip = Sounds[0];
        audioSource.time = SoundStartTimes[0];
        audioSource.Play();
    }

    //NOTE: The actual shield mechanism is handled in PlayerCollision.cs
}
