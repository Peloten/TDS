using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum MonsterNum
{
    First = 0,
    Last = 1,
}

public class MonsterCtrl : MonoBehaviour
{
    public MonsterNum mn;

    // 몬스터 체력
    public int zombieHp = 100;
    // 몬스터 속도
    public float monsterSpeed;
    // 몬스터와 다른 몬스터 거리
    public float monsterDistance;

    // 데미지 UI 관련
    public RectTransform damageRact;
    public Text damageTxt;
    public float upTxt = 0.3f;
    public float downTxt = 0.3f;

    // 레이케스트 위치
    private Vector2 yPosition;
    // 오르는 중인지 확인
    private bool isClimbing;

    private Collider2D myCollider;
    private Rigidbody2D rb;

    // 중력 저장
    private float gravity;
    // 스피드 저장
    private float saveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        isClimbing = false;
        myCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        gravity = rb.gravityScale;
        saveSpeed = monsterSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (isClimbing) return;

        yPosition = (Vector2)transform.position + Vector2.up * 0.5f + Vector2.left * 0.6f;
        transform.position += Vector3.left * monsterSpeed * Time.deltaTime;

        if(mn == MonsterNum.First)
        {
            RaycastHit2D hit = Physics2D.Raycast(yPosition, Vector2.left, monsterDistance, LayerMask.GetMask("Monster"));
            Raycast(hit);
        }
        else if(mn == MonsterNum.Last)
        {
            RaycastHit2D hit = Physics2D.Raycast(yPosition, Vector2.left, monsterDistance, LayerMask.GetMask("Monster_1"));
            Raycast(hit);
        }

        Debug.DrawRay(yPosition, Vector2.left * monsterDistance, Color.red);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hero"))
        {
            this.monsterSpeed = 0f;
        }
        else if (collision.gameObject.CompareTag("Bullet"))
        {
            Destroy(collision.gameObject);
            RemoveMonster();
            int randomDamage = Random.Range(10, 30);
            zombieHp -= randomDamage;
            damageTxt.text = randomDamage.ToString();
            StartCoroutine(DamageObjMov());
            if (zombieHp <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hero"))
        {
            RemoveMonster();
        }
    }

    private void Raycast(RaycastHit2D hit)
    {
        if (hit.collider != null && hit.collider != myCollider)
        {
            MonsterCtrl otherMonster = hit.collider.GetComponent<MonsterCtrl>();
            if (otherMonster != null && Mathf.Approximately(otherMonster.monsterSpeed, 0f))
            {
                Debug.Log("앞에 몬스터 감지");
                StartCoroutine(ClimbMonster());
            }
            else
            {
                RemoveMonster();
            }
        }
    }

    private void RemoveMonster()
    {
        StopCoroutine(ClimbMonster());
        monsterSpeed = saveSpeed;
        rb.gravityScale = gravity;
    }

    IEnumerator ClimbMonster()
    {
        isClimbing = true;

        rb.gravityScale = 0;

        float climbHeight = 1.3f;
        float climbSpeed = 2f;
        float targetY = transform.position.y + climbHeight;

        while (transform.position.y < targetY)
        {
            transform.position += Vector3.up * climbSpeed * Time.deltaTime;
            transform.position += Vector3.left * 1f * Time.deltaTime;
            yield return null; 
        }

        rb.gravityScale = gravity;

        isClimbing = false;
    }

    IEnumerator DamageObjMov()
    {
        Vector2 startPos = damageRact.anchoredPosition;
        Vector2 upPos = startPos + Vector2.up * 60f;
        Vector2 downPos = upPos + Vector2.down * Mathf.Abs(-30f);

        float n = 0f;
        while(n < upTxt)
        {
            n += Time.deltaTime;
            float t = n / upTxt;
            damageRact.anchoredPosition = Vector2.Lerp(startPos, upPos, t);
            yield return null;
        }

        n = 0f;
        while (n < downTxt)
        {
            n += Time.deltaTime;
            float t = n / downTxt;
            damageRact.anchoredPosition = Vector2.Lerp(upPos, downPos, t);
            yield return null;
        }

        damageTxt.text = "";
    }
}
