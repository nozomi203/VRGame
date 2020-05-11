using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Gate : MonoBehaviour
{
    [SerializeField]
    private Vector3 openOffset;
    [SerializeField]
    private float openDuration = 5;
    private Vector3 initialPos;
    private GameObject door;
    private Material[] materials;
    private float emissionRate = 0;
    private AudioSource audioSource;

    
    
    public void ChangeEmission(float start, float goal)
    {
        DOVirtual.Float(start, goal, openDuration, value =>
        {
            foreach (Material material in materials)
            {
                material.SetFloat("_EmissionRate", emissionRate);
                emissionRate = value;
            }
        });
    }
    public void OpenDoor()
    {
        audioSource.Play();
        door.transform.DOMove(initialPos + openOffset.y * transform.up + openOffset.z * transform.forward, openDuration).OnComplete(() =>
        {
            Invoke("CloseDoor", 3f);
        });
        ChangeEmission(0, 5);
    }
    public void CloseDoor()
    {
        audioSource.Play();
        door.transform.DOMove(initialPos, openDuration);
    }
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        door = transform.Find("Door").gameObject;
        initialPos = door.transform.position;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        materials = new Material[renderers.Length];
        for(int i = 0; i < materials.Length; i++)
        {
            materials[i] = renderers[i].material;
        }
    }

}
