using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ButtonSounds : MonoBehaviour
{
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip hoverSound;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start() {
        audioSource = GetComponent<AudioSource>();
    }
    
    public void PlayClickSound() {
        if (clickSound == null) return;
        audioSource.clip = clickSound;
        audioSource.Play();
    }
    
    public void PlayHoverSound() {
        if (hoverSound == null) return;
        audioSource.clip = hoverSound;
        audioSource.Play();
    }
}
