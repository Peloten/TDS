using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject enemyObj;
    public GameObject enemyObj_1;
    public Transform monsterSpawnPos;
    public float monsterSpawnTime;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MonsterSpawn());
    }

    IEnumerator MonsterSpawn()
    {
        while (true)
        {
            int random = (int)Random.Range(0, 2);
            if (random == 0)
            {
                Instantiate(enemyObj, monsterSpawnPos);
            }
            else
            {
                Instantiate(enemyObj_1, monsterSpawnPos);
            }
            yield return new WaitForSeconds(monsterSpawnTime);
        }
    }
}
