using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeEffects : MonoBehaviour
{
    [SerializeField] private AudioSource upgradeSFX;
    [SerializeField] private AudioClip poweringUpSFX;
    [SerializeField] private AudioClip impactSFX;
    [SerializeField] private ParticleSystem upgradeParticle;
    private WaitForSeconds upgFinalEffectWait = new WaitForSeconds(0.5f);

    public void PlayPoweringUp() {
        upgradeSFX.clip = poweringUpSFX;
        upgradeSFX.time = 0.25f;
        upgradeSFX.Play();
    }

    public IEnumerator PlayUpgradedEffect() {
        yield return upgFinalEffectWait;
        
        if (upgradeSFX.isPlaying) upgradeSFX.Stop();

        //plays second sfx
        upgradeSFX.clip = impactSFX;
        upgradeSFX.time = 0;
        upgradeSFX.Play();

        //releases particles
        upgradeParticle.Play();
    }
}
