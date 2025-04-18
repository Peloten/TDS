using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckCtrl : MonoBehaviour
{
    // 닿은 몬스터 확인
    public int monsterCount;

    // 닿은 몬스터를 가져가기 위한 싱글톤
    public static TruckCtrl instance;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        monsterCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    // 몬스터가 닿았을 경우 플러스
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            monsterCount++;
        }
        if (collision.gameObject.CompareTag("Monster_1"))
        {
            monsterCount++;
        }
    }

    // 몬스터가 떨어졌을 경우 마이너스
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            monsterCount--;
        }
        if (collision.gameObject.CompareTag("Monster_1"))
        {
            monsterCount--;
        }
    }
}
