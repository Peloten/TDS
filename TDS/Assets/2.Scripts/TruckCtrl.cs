using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckCtrl : MonoBehaviour
{
    // ���� ���� Ȯ��
    public int monsterCount;

    // ���� ���͸� �������� ���� �̱���
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
    
    // ���Ͱ� ����� ��� �÷���
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

    // ���Ͱ� �������� ��� ���̳ʽ�
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
