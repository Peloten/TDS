using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    int randomDamage;

    // Start is called before the first frame update
    void Start()
    {
        randomDamage = Random.Range(10, 30);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ÃÑ¾ËÀÌ ¶¥¹Ù´Ú¿¡ ´êÀ» °æ¿ì Á¦°Å
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        { 
                // ÃÑ¾Ë »èÁ¦
                Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Monster"))
        {
            // ÃÑ¾Ë »èÁ¦
            Destroy(gameObject);
            MonsterCtrl monCtrl = collision.gameObject.GetComponent<MonsterCtrl>();
            if (monCtrl != null) 
            {
                monCtrl.Damage(randomDamage);
            }
        }
        if (collision.gameObject.CompareTag("Monster_1"))
        {
            // ÃÑ¾Ë »èÁ¦
            Destroy(gameObject);
            MonsterCtrl monCtrl = collision.gameObject.GetComponent<MonsterCtrl>();
            if (monCtrl != null)
            {
                monCtrl.Damage(randomDamage);
            }
        }
    }
}
