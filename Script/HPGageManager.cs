using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HPGageManager : MonoBehaviour {
    [SerializeField]
    private float shakeStrength;
    private const float maxRate = 0.923f;
    private const float minRate = 0.077f;
    private const float rateWidth = 0.846f;

    private Material greenGageMat;
    private Material redGageMat;

    //ダメージが入るときのコルーチン
    private IEnumerator damageCoroutine;

    private int displayRateID;
	// Use this for initialization
	void Start () {
        greenGageMat = transform.Find("HPGageGreen").GetComponent<MeshRenderer>().material;
        redGageMat = transform.Find("HPGageRed").GetComponent<MeshRenderer>().material;

        displayRateID = Shader.PropertyToID("_DisplayRate");
        greenGageMat.SetFloat(displayRateID, maxRate);
        redGageMat.SetFloat(displayRateID, maxRate);
	}
	
	public void GetDamage(float damageRate)
    {
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
        }
        damageCoroutine = null;
        damageCoroutine = UpdateGageLength(damageRate);
        StartCoroutine(UpdateGageLength(damageRate));
    }

    private IEnumerator UpdateGageLength(float damageRate)
    {
        transform.DOShakePosition(1, shakeStrength, 20);

        float redGageRate = redGageMat.GetFloat(displayRateID);
        float targetRate = minRate + damageRate * rateWidth;
        greenGageMat.SetFloat(displayRateID, targetRate);
        while(redGageRate - targetRate > 0.001f)
        {
            redGageRate = Mathf.Lerp(redGageRate, targetRate, 0.04f);
            redGageMat.SetFloat(displayRateID, redGageRate);
            yield return null;
        }

    }
}
