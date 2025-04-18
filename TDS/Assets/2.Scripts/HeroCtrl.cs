using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeroCtrl : MonoBehaviour
{
    public GameObject heroGun;
    public GameObject bulletObj;
    public Transform firePoint;
    public int bulletCount = 3;
    public float spreadAngle = 30f;

    private bool fireCheck;

    // Start is called before the first frame update
    void Start()
    {
        fireCheck = true;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Vector3 direction = mousePos - heroGun.transform.position; 

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        heroGun.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (fireCheck)
            {
                fireCheck = false;
                FireShotgun();
                StartCoroutine(FireCoolTime());
            }
        }
    }

    private void FireShotgun()
    {
        for (int i =0; i < bulletCount; i++)
        {
            float randomAngle = Random.Range(-spreadAngle / 2, spreadAngle / 2);
            Quaternion rot = firePoint.rotation * Quaternion.Euler(0, 0, randomAngle);
            GameObject bullet = Instantiate(bulletObj, firePoint.position, rot);
            Rigidbody2D rd = bullet.GetComponent<Rigidbody2D>();
            rd.velocity = rot * Vector2.right * 25f;
        }
    }

    IEnumerator FireCoolTime()
    {
        yield return new WaitForSeconds(1f);
        fireCheck = true;
    }
}
