using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeroCtrl : MonoBehaviour
{
    // 샷건 오브젝트
    public GameObject heroGun;
    // 발사될 총알 오브젝트
    public GameObject bulletObj;
    // 샷건 발사 위치
    public Transform firePoint;
    // 최대 발사 총알 갯수
    public int bulletCount = 3;
    // 샷건 발사 범위 각도
    public float spreadAngle = 30f;

    // 총알 발사 확인
    private bool fireCheck;

    // Start is called before the first frame update
    void Start()
    {
        fireCheck = true;
    }

    // Update is called once per frame
    void Update()
    {
        // 마우스 위치에 따라 샷건 각도 조절
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector3 direction = mousePos - heroGun.transform.position; 
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        heroGun.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // 마우스 왼쪽 클릭으로 발사
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

    // 샷건 특정 각도로 랜덤으로 발사
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

    // 샷건 쿨타임 코루틴
    IEnumerator FireCoolTime()
    {
        yield return new WaitForSeconds(1f);
        fireCheck = true;
    }
}
