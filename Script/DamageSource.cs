using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DamageSource : MonoBehaviour {

    [SerializeField]
    private int damage;
    [SerializeField]
    private GameObject repelEffect;
    [SerializeField]
    private GameObject hitEffect;
    [SerializeField]
    private bool isWeaponActive;
    [SerializeField]
    private string damageLayerName;
    [SerializeField]
    private bool isPlayerWeapon;
    [SerializeField]
    private float maxSpeed;
    [SerializeField, Space(15)]
    private AudioClip[] weaponHitSound;
    [SerializeField]
    private AudioClip repelSound;


    private AudioSource audioSource;
    private int damageLayerCount = 0;
    private float weaponSpeed;
    private Vector3 prePos;
    private Vector3 curPos;

    private void Start()
    {
        audioSource = GetComponents<AudioSource>()[0];
    }
    private void Update()
    {
        UpdateVelocity();
    }

    public int GetDamage()
    {
        return damage; 
    }

    public void SetWeaponActive(bool b)
    {
        isWeaponActive = b;
    }
    public bool GetWeaponActive()
    {
        return isWeaponActive;
    }
    /*
    private void OnTriggerStay(Collider other)
    {
        if(LayerMask.LayerToName(other.gameObject.layer) == "PlayerWeapon")
        {
            if (isWeaponActive)
            {
                RepelWeapon();
            }
        }
    }
    */

    private void OnTriggerEnter(Collider other)
    {
        if(LayerMask.LayerToName(other.gameObject.layer) == damageLayerName)
        {
            damageLayerCount++;

            if (isWeaponActive)
            {
                isWeaponActive = false;
                float damageRate = Mathf.Clamp01(weaponSpeed / maxSpeed);
                PlayHitSound(damageRate, other.ClosestPointOnBounds(transform.position), other.GetComponent<HitCollider>().GetIsInvincible());
                other.GetComponent<HitCollider>().Damage(damage * damageRate, other.ClosestPointOnBounds(this.transform.position));
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == damageLayerName)
        {
            damageLayerCount--;
            if(damageLayerCount < 1)
            {
                StartCoroutine(DelayMethod(0.2f, SetWeaponActive, true));
            }
        }
    }
    /*
    private void RepelWeapon()
    {
        Debug.Log("Weapon Hit");
        weaponClashSound.Play();
        Instantiate(sparkEffect, transform.position, Quaternion.identity);
        isWeaponActive = false;
        unityChanController.RepelWeapon();
    }
    */

    private IEnumerator DelayMethod(float t, Action<bool> func, bool b)
    {
        yield return new WaitForSeconds(t);
        func(b);
    }

    private void UpdateVelocity()
    {
        curPos = transform.position;
        weaponSpeed = (curPos - prePos).magnitude;
        prePos = curPos;
    }
    private void PlayHitSound(float speedRate, Vector3 hitPos, bool isRepel)
    {
        if (isRepel)
        {

            Instantiate(repelEffect, hitPos, Quaternion.identity);
            audioSource.clip = repelSound;
            audioSource.Play();
            return;
        }
        int len = weaponHitSound.Length;
        if (len != 0)
        {
            Instantiate(hitEffect, hitPos, Quaternion.identity);
            audioSource.clip = weaponHitSound[(int)((len - 1) * speedRate)];
            audioSource.Play();
        }
    }
}
