using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCollider : MonoBehaviour {

    [SerializeField]
    private bool isInvincible = false;

    private LifeManager lifeManager;
    //private string damageLayerName;

	// Use this for initialization
	void Start () {
        FindLifeManager();
        //damageLayerName = lifeManager.GetDamageLayerName();
	}
	
    public bool GetIsInvincible()
    {
        return isInvincible;
    }

    private void FindLifeManager()
    {
        LifeManager lm = null;
        GameObject curObj = this.gameObject;
        while(lm == null)
        {
            lm = curObj.GetComponent<LifeManager>();
            if (curObj.transform.parent != null)
            {
                curObj = curObj.transform.parent.gameObject;
            }
            else
            {
                break;
            }
        }
        lifeManager = lm;
        lifeManager.SetHitCollider(this.gameObject);

    }

    public void Damage(float damage, Vector3 hitPos)
    {
        if (!isInvincible)
        {
            lifeManager.Damage(damage);
        }   
        //lifeManager.GetAttacked(damage, hitPos);
    }


}
