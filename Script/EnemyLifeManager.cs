using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyLifeManager : LifeManager
{

    [SerializeField]
    private Color dieColor;
    [SerializeField]
    private GameObject meshBreakParticle;
    [SerializeField]
    private float maxVerSpeed = 1f;
    [SerializeField]
    private float maxHorSpeed = 1f;
    [SerializeField, Space(15)]
    private AudioClip breakSound1;
    [SerializeField]
    private AudioClip breakSound2;

    private Color curColor = Color.black;

    private Renderer FindRenderer()
    {
        Renderer renderer = null;
        foreach (Transform transform in GetComponentInChildren<Transform>())
        {
            renderer = transform.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                break;
            }
        }
        return renderer;
    }

    protected override void Die()
    {
        gameController.KillEnemy();
        GetComponent<Enemy>().Die();
        StartCoroutine(DieCrtn());
    }

    private IEnumerator DieCrtn()
    {

        foreach (GameObject obj in hitColliders)
        {
            //コリダーを遠くまで吹っ飛ばす（消してしまうと不具合）
            obj.transform.DOMove(obj.transform.position + 5 * Vector3.up, 2f);
        }

        Material material = FindRenderer().material;
        float alpha = 1;
        float emissionRate = 0;
        //輝き始める
        PlaySound(breakSound1);
        DOVirtual.Float(0f, 1f, 1f, value =>
        {
            emissionRate = value;
            material.SetFloat("_EmissionRate", emissionRate);
        });
        yield return new WaitForSeconds(1f);

        //爆散
        PlaySound(breakSound2);
        Instantiate(meshBreakParticle, transform.position, Quaternion.identity);
        material.shader = Shader.Find("Unlit/MeshBreak");
        material.SetFloat("_TimeOffset", Time.time);
        material.SetColor("_Emission", dieColor);
        material.SetInt("_RandomSeed", (int)Time.time);
        material.SetFloat("_MaxVerSpeed", maxVerSpeed);
        material.SetFloat("_MaxHorSpeed", maxHorSpeed);

        //透明になり消えていく
        DOVirtual.Float(1f, 0f, 2f, value =>
        {
            alpha = value;
            material.SetFloat("_Alpha", alpha);
        });
        yield return new WaitForSeconds(3f);
        /*
        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        */
        Destroy(gameObject);
    }

}
