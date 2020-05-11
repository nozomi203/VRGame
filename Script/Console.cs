using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Console : MonoBehaviour
{
    [SerializeField]
    private float moveDist;
    [SerializeField]
    private AudioClip startSound;
    private Material[] materials;
    private GameController gc;
    private bool isPlaying = false;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gc = GameObject.FindWithTag("GameController").GetComponent<GameController>() ;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        materials = new Material[renderers.Length];
        for(int i = 0; i < renderers.Length; i++)
        {
            materials[i] = renderers[i].material;
        }
    }

    public void ChangeBright(float s, float g, Action callback = null)
    {
        float emission = s;
        DOVirtual.Float(s, g, 3f, value =>
        {
            emission = value;
            foreach(Material m in materials)
            {
                m.SetFloat("_EmissionRate", emission);
            }

        }).OnComplete(()=> { callback?.Invoke(); });
    }
    public void MoveConsole(float dist)
    {
        PlaySound(startSound);
        Transform console = transform.Find("Console");
        console.DORotate(new Vector3(0, 180, 0), 2f);
        console.DOMove(console.transform.position + console.transform.up * dist, 3f).OnComplete(()=>
        {
            ChangeBright(10f, 0f);
        });
    }
    public void StartGame()
    {
        MoveConsole(moveDist);
        gc.StartGame();
    }

    private void PlaySound(AudioClip ac)
    {
        audioSource.clip = ac;
        audioSource.Play();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.NameToLayer("PlayerWeapon") == other.gameObject.layer)
        {
            if (!isPlaying)
            {
                isPlaying = true;
                StartGame();
            }
        }
    }
}
