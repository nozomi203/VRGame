using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] target;
    [SerializeField]
    private int startCount = 3;

    private bool isPlaying;
    private InfoBoard infoBoard;
    private EnemyGeneratorHub egHub;


    public void SetIsPlaying(bool b)
    {
        isPlaying = b;
    }
    public bool GetIsPlaying(bool b)
    {
        return isPlaying;
    }

    void Start()
    {
        infoBoard = GameObject.FindWithTag("InfoBoard").GetComponent<InfoBoard>();
        egHub = GameObject.FindWithTag("EnemyGeneratorHub").GetComponent<EnemyGeneratorHub>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }

    public void EndGame(string loser)
    {
        isPlaying = false;
        SetColliderActive(false);
        infoBoard.EndGame(loser);
    }
    public void KillEnemy()
    {
        infoBoard.UpdateKillCount();
    }
    private void SetColliderActive(bool b)
    {
        foreach(GameObject obj in target)
        {
            foreach(Collider col in obj.GetComponentsInChildren<Collider>())
            {
                col.enabled = b;
            }
        }
    }
    private void ReadyGame()
    {
        infoBoard.Count(startCount);
        Invoke("StartGame", startCount);

    }
    public void StartGame()
    {
        if (!isPlaying)
        {
            isPlaying = true;
            SetColliderActive(true);
            egHub.StartGenerating();
            infoBoard.StartGame();
        }
    }
}
