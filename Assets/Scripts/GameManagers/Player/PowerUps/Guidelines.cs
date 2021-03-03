using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guidelines : AbstractPowerUp
{
    [SerializeField] private PlayerPowerUp powerUpManager;
    [SerializeField] private float duration;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject guidelinesObject;

    // Update is called once per frame
    void Update() {
        
    }
}
