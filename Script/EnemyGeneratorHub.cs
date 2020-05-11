using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class EnemyGeneratorHub : MonoBehaviour
{
    [SerializeField]
    private GameObject[] enemyObjects;
    [SerializeField, Space(30)]
    private int maxEnemyCount = 3;
    [SerializeField]
    private float minGenerateInterval = 3f;
    [SerializeField]
    private float intervalRange = 2f;

    private EnemyGenerator[] enemyGenerators;
    private List<int> activeGenerators = new List<int>();
    private int currentEnemyCount = 0;
    private int totalEnemyCount = 0;
    private int totalEnemyDieCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public void StartGenerating()
    {
        StartCoroutine(StartGeneratingCrtn());
    }
    public void EnemyDie(int idx)
    {
        activeGenerators.Add(idx);
        currentEnemyCount--;
        totalEnemyDieCount++;
    }
    private void Init()
    {
        enemyGenerators = GetComponentsInChildren<EnemyGenerator>();
        for(int i = 0; i < enemyGenerators.Length; i++)
        {
            enemyGenerators[i].SetGeneratorID(i);
            activeGenerators.Add(i);
        }
    }
    private void GenerateEnemy(int enemyIdx)
    {
        if (activeGenerators.Count != 0 && currentEnemyCount < maxEnemyCount)
        {
            int idx = (int)(Random.value * (activeGenerators.Count - 1));
            enemyGenerators[activeGenerators[idx]].GenerateEnemy(enemyObjects[enemyIdx]);
            activeGenerators.RemoveAt(idx);
            currentEnemyCount++;
            totalEnemyCount++;
        }
    }
    private IEnumerator StartGeneratingCrtn()
    {
        while (true)
        {
            float interval = minGenerateInterval + Random.value * intervalRange;
            yield return new WaitForSeconds(interval);
            int enemyIdx = (int)(Random.value * (enemyObjects.Length - 1));
            GenerateEnemy(enemyIdx);
        }

    }
   
}
