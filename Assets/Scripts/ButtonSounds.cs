using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        audioSource.clip = clickSound;
        audioSource.Play();
    }
    
    public void PlayHoverSound() {
        audioSource.clip = hoverSound;
        audioSource.Play();
    }
}
