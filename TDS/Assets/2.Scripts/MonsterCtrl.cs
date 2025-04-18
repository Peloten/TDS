using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
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

        // 중력값과 스피드 저장
        gravity = rb.gravityScale;
        saveSpeed = monsterSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        // 오르는 중에는 업데이트 리턴
        if (isClimbing) return;

        // 레이 위치 조정
        yPosition = (Vector2)transform.position + Vector2.up * 0.5f + Vector2.left * 0.5f;
        transform.position += Vector3.left * monsterSpeed * Time.deltaTime;

        if (mn == MonsterNum.First)
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

    void FixedUpdate()
    {
        // 예: 중력 적용 중이거나 직접 속도 제어
        rb.velocity += Vector2.down * gravity * Time.fixedDeltaTime;
    }

    public void Damage(int damage)
    {
        zombieHp -= damage;
        damageTxt.text = damage.ToString();

        // 데미지 UI 실행
        StartCoroutine(DamageObjMov());

        // hp가 0이면 제거
        if (zombieHp <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hero"))
        {
            // 트럭과 닿으면 속도를 0으로 고정
            this.monsterSpeed = 0f;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Monster") || collision.gameObject.CompareTag("Monster_1"))
        {
            // 아래 몬스터 높이와 자신의 높이 확인
            float otherY = collision.transform.position.y;
            float myY = transform.position.y;
            MonsterCtrl otherMonster = collision.collider.GetComponent<MonsterCtrl>();
            // 자신의 높이보다 낮고 이동속도가 0이면 실행
            if (myY < otherY - 0.1f && Mathf.Approximately(otherMonster.monsterSpeed, 0f))
            {
                int aboveCount = CountMonstersAbove();
                int behindCount = CountMonstersBehind();
                int total = Mathf.Clamp(aboveCount + behindCount, 2, 6);

                if (total > 0)
                {
                    float forceAmount = 0.35f * total;
                    Vector2 pushDir = new Vector2(1f, 0);
                    rb.AddForce(pushDir * forceAmount, ForceMode2D.Impulse);
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hero"))
        {
            monsterSpeed = saveSpeed;
            rb.gravityScale = gravity;
        }
    }

    // 앞에 몬스터 확인
    private void Raycast(RaycastHit2D hit)
    {
        if (hit.collider != null && hit.collider != myCollider)
        {
            MonsterCtrl otherMonster = hit.collider.GetComponent<MonsterCtrl>();
            if (otherMonster != null && Mathf.Approximately(otherMonster.monsterSpeed, 0f))
            {
                StartCoroutine(ClimbMonster());
            }
        }
    }

    // 위에 몬스터수 확인
    private int CountMonstersAbove()
    {
        if (mn == MonsterNum.First)
        {
            int count = 0;
            Collider2D[] hits = Physics2D.OverlapBoxAll((Vector2)transform.position + Vector2.up, new Vector2(1f, 1.5f), 0f);
            foreach (var hit in hits)
            {
                if (hit.gameObject == gameObject) continue;
                if (hit.CompareTag("Monster") && hit.transform.position.y > transform.position.y + 0.1f)
                {
                    count++;
                }
            }
            return count;
        }
        if (mn == MonsterNum.Last)
        {

            int count = 0;
            Collider2D[] hits = Physics2D.OverlapBoxAll((Vector2)transform.position + Vector2.up, new Vector2(1f, 1.5f), 0f);
            foreach (var hit in hits)
            {
                if (hit.gameObject == gameObject) continue;
                if (hit.CompareTag("Monster_1") && hit.transform.position.y > transform.position.y + 0.1f)
                {
                    count++;
                }
            }
            return count;
        }
        return 0;
    }

    // 오른쪽 몬스터수 확인
    private int CountMonstersBehind()
    {
        if (mn == MonsterNum.First)
        {
            int count = 0;
            Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(2f, 1f), 0f);
            if(hits.Length > 2)
            {
                foreach (var hit in hits)
                {
                    if (hit.gameObject == gameObject) continue;
                    Vector2 dir = hit.transform.position - transform.position;
                    if (hit.CompareTag("Monster") && dir.x > 0.5f && Mathf.Abs(dir.y) < 0.5f)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        if (mn == MonsterNum.Last)
        {
            int count = 0;
            Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(2f, 1f), 0f);
            if(hits.Length > 2)
            {
                foreach (var hit in hits)
                {
                    if (hit.gameObject == gameObject) continue;
                    Vector2 dir = hit.transform.position - transform.position;
                    if (hit.CompareTag("Monster_1") && dir.x > 0.5f && Mathf.Abs(dir.y) < 0.5f)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        return 0;
    }

    // 오르기 코루틴
    IEnumerator ClimbMonster()
    {
        isClimbing = true;

        rb.gravityScale = 0;

        float climbHeight = 1.3f;
        float climbSpeed = 2f;
        float targetY = transform.position.y + climbHeight;

        while (transform.position.y < targetY)
        {
            if (mn == MonsterNum.First)
            {
                RaycastHit2D hit = Physics2D.Raycast(yPosition, Vector2.left, 1f, LayerMask.GetMask("Monster"));
                if (hit.collider == null || hit.collider.GetComponent<MonsterCtrl>() == null)
                {
                    monsterSpeed = saveSpeed;
                    rb.gravityScale = gravity;
                    break;
                }
            }
            if (mn == MonsterNum.Last)
            {
                RaycastHit2D hit = Physics2D.Raycast(yPosition, Vector2.left, 1f, LayerMask.GetMask("Monster_1"));
                if (hit.collider == null || hit.collider.GetComponent<MonsterCtrl>() == null)
                {
                    monsterSpeed = saveSpeed;
                    rb.gravityScale = gravity;
                    break;
                }
            }

            transform.position += Vector3.up * climbSpeed * Time.deltaTime;
            transform.position += Vector3.left * 1f * Time.deltaTime;
            yield return null; 
        }

        rb.gravityScale = gravity;
        isClimbing = false;
    }

    // 데미지 UI 코루틴
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
