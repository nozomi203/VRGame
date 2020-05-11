using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class LifeManager : MonoBehaviour {

    public enum CharacterType
    {
        Enemy,
        Player
    }
    [SerializeField]
    private int maxLife = 10;
    [SerializeField]
    private string damageLayerName = "Weapon";
    //[SerializeField]
    //private GameObject[] damageEffects;
    [SerializeField]
    private CharacterType characterType;

    private float curLife;
    private bool isDead = false;
    private bool isInvincible = false;
    private HPGageManager hpGageManager;
    protected GameController gameController;
    protected AudioSource audioSource;
    protected List<GameObject> hitColliders = new List<GameObject>();

    // Use this for initialization
    void Start () {
        Init();
    }
	
    public void SetHitCollider(GameObject hitCollider)
    {
        hitColliders.Add(hitCollider);
    }
    public float GetCurLife()
    {
        return curLife;
    }
    public bool GetIsDead()
    {
        return isDead;
    }
    public void SetIsInvincible(bool b)
    {
        isInvincible = b;
    }
    public bool GetIsInvincible()
    {
        return isInvincible;
    }
    public string GetDamageLayerName()
    {
        return damageLayerName;
    }
    public void Recover(int recover)
    {
        if (!isDead)
        {
            curLife = Mathf.Min(curLife + recover, maxLife);

        }
    }
    public void Damage(float damage)
    {
        if (!isInvincible)
        {
            curLife = Mathf.Max(curLife - damage, 0);
            hpGageManager.GetDamage(curLife / maxLife);
        }

        if(curLife == 0)
        {
            
            if (!isDead)
            {
                isDead = true;
                Die();
            }
        }
    }
    /*
    public void GetAttacked(float damage, Vector3 hitPos)
    {
            Damage(damage);
            if (damageEffects != null)
            {
                foreach (GameObject effect in damageEffects)
                {
                    Instantiate(effect, hitPos, Quaternion.identity);
                }
            }
    }
    */
    private void Init()
    {
        curLife = maxLife;
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        audioSource = GetComponent<AudioSource>();
        hpGageManager = GetComponentInChildren<HPGageManager>();
    }
    protected  void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
    protected virtual void Die()
    {
        Debug.Log("DontCallme");
        //Let's override!
    }
}
