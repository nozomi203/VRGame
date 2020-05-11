using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{

    private EnemyGeneratorHub hub;
    private Gate gate;
    private bool isEnemyAlive = false;
    private int generatorID;

    private void Start()
    {
        hub = GameObject.FindWithTag("EnemyGeneratorHub").GetComponent<EnemyGeneratorHub>();
        gate = GetComponentInChildren<Gate>();
    }

    public void GenerateEnemy(GameObject enemyObject)
    {
        if (!isEnemyAlive)
        {
            isEnemyAlive = true;
            gate.OpenDoor();
            GameObject obj = Instantiate(enemyObject, transform.position, transform.rotation);
            obj.GetComponent<Enemy>().SetEnemyGenerator(this.GetComponent<EnemyGenerator>());
        }
    }
    public void EnemyDie()
    {
        isEnemyAlive = false;
        gate.ChangeEmission(5f, 0f);
        hub.EnemyDie(generatorID);
    }
    public void SetGeneratorID(int id)
    {
        generatorID = id;
    }
}
