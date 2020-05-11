using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    [SerializeField]
    private AudioClip walkSoundClip;

    private AudioSource walkSoundSource;
    private GameObject weapon;
    

    private void Start()
    {
        weapon = GameObject.FindWithTag("EnemyWeapon");

        walkSoundSource = gameObject.AddComponent<AudioSource>();
        walkSoundSource.clip = walkSoundClip;
        walkSoundSource.playOnAwake = false;
        walkSoundSource.volume = 0.2f;
    }

    public void SetWeaponActiveTrue(int audioId)
    {
        AudioSource[] audioSource = weapon.GetComponentsInChildren<AudioSource>();

        audioSource[audioId].Play();

        weapon.GetComponent<DamageSource>().SetWeaponActive(true);
    }

    public void SetWeaponActiveFalse()
    {
        weapon.GetComponent<DamageSource>().SetWeaponActive(false);
    }

    public void WalkSound()
    {
        walkSoundSource.Play();
    }
}
