using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guidelines : AbstractPowerUp
{
    [SerializeField] private PlayerPowerUp powerUpManager;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject guidelinesObject;

    [Header("Settings")]
    [SerializeField] private float duration;
    [SerializeField] private float length;

    public IEnumerator RunGuidelines() {
        Transform objTrans = guidelinesObject.transform;
        Vector3 objScale = objTrans.localScale;
        Vector3 objPos = objTrans.localPosition;
        float t = 0f;

        //active state
        powerUpManager.State = PowerUpState.Active;
        
        
        //extend outwards
        guidelinesObject.SetActive(true);
        while (t <= 1) {
            t += 2f * Time.deltaTime;
            objScale.x = Mathf.Lerp(0, length, t);
            objPos.x = (objScale.x / 2f) + 0.4f;

            objTrans.localScale = objScale;
            objTrans.localPosition = objPos;
            yield return null;
        }

        //wait
        powerUpManager.ticker = duration;
        while (powerUpManager.ticker > 0) {
            powerUpManager.TopBar.value = (powerUpManager.ticker / duration);
            yield return null;
        }

        //retract
        t = 1f;
        while (t >= 0) {
            t -= 4f * Time.deltaTime;
            objScale.x = Mathf.Lerp(0, length, t);
            objPos.x = (objScale.x / 2f) + 0.4f;

            objTrans.localScale = objScale;
            objTrans.localPosition = objPos;
            yield return null;
        }
        
        guidelinesObject.SetActive(false);
        powerUpManager.RemovePowerUp();
    }
}
