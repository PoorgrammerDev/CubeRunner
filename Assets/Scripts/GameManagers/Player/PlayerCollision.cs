﻿using UnityEngine;
using System.Collections;
using TMPro;

/// <summary>
/// Handles player collision with objects, like obstacles or power-ups.
/// </summary>
public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private GameValues gameValues;
    [SerializeField] private EndGame gameEndManager;
    [SerializeField] private PowerUpSpawner powerUpSpawner;
    [SerializeField] private TextMeshProUGUI bitsText;
    [SerializeField] private AudioSource bitsSound;
    [SerializeField] private Hardened hardenedPUP;
    private PlayerPowerUp playerPowerUp;    
    private new Collider collider;

    void Start() {
        playerPowerUp = GetComponent<PlayerPowerUp>();
        collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other) {
        GameObject otherObject = other.gameObject;
        if (otherObject != gameObject) {

            //OBSTACLES ----- ----- ----- 
            if (otherObject.CompareTag(TagHolder.OBSTACLE_TAG)) {
                if (otherObject.transform.position.x > 0) {
                    CollideWithObstacle();
                }
            }
            
            //POWER UPS ----- ----- ----- 
            else if (otherObject.CompareTag(TagHolder.POWERUP_TAG)) {
                Physics.IgnoreCollision(collider, other, true);
                
                PowerUp powerUpObject;
                if (otherObject.TryGetComponent<PowerUp>(out powerUpObject)) {
                    Row row;
                    if (powerUpObject.transform.parent.TryGetComponent<Row>(out row)) {
                        if (playerPowerUp.AddPowerUp(powerUpObject.Type)) { 
                            powerUpSpawner.DespawnPowerUp(row);
                        }
                    }
                }
            }

            //IGNORE GIBS ----- ----- ----- 
            else if (otherObject.CompareTag(TagHolder.GIBS_TAG)) {
                Physics.IgnoreCollision(collider, other, true);
            }
            
            //BITS ----- ----- -----
            else if (otherObject.CompareTag(TagHolder.BITS_TAG)) {
                Physics.IgnoreCollision(collider, other, true);
                other.gameObject.SetActive(false);
                
                //increment bits, update text
                gameValues.Bits += gameValues.BitsMultiplier;
                bitsText.text = gameValues.Bits.ToString();

                //play sound
                bitsSound.Play();
            }
        }
    }

    //Prevent bits physical collision
    void OnControllerColliderHit(ControllerColliderHit hit) {
        if (hit.collider.CompareTag(TagHolder.BITS_TAG)) {
            Physics.IgnoreCollision(hit.controller, hit.collider, true);
        }
    }

    private void CollideWithObstacle() {
        //HARDENED POWER-UP OVERRIDE OBSTACLE HITTING
        if (playerPowerUp.GetActivePowerUp() == PowerUpType.Hardened) {
            hardenedPUP.Impact(this.transform);
            return;
        }

        gameEndManager.endGame((PlayerPrefs.GetInt(TagHolder.PREF_SKIP_ANIM) == 0) && gameValues.Divide != 0);
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
    }
    
}
