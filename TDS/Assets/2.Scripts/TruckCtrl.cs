using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckCtrl : MonoBehaviour
{
    public int monsterCount;

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            monsterCount++;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            monsterCount--;
        }
    }
}
