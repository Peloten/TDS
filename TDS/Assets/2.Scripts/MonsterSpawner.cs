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


    // 랜덤으로 몬스터1과 몬스터2 소환 코루틴
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
