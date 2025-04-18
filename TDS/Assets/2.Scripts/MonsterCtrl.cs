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

    // ���� ü��
    public int zombieHp = 100;
    // ���� �ӵ�
    public float monsterSpeed;
    // ���Ϳ� �ٸ� ���� �Ÿ�
    public float monsterDistance;

    // ������ UI ����
    public RectTransform damageRact;
    public Text damageTxt;
    public float upTxt = 0.3f;
    public float downTxt = 0.3f;

    // �����ɽ�Ʈ ��ġ
    private Vector2 yPosition;
    // ������ ������ Ȯ��
    private bool isClimbing;

    private Collider2D myCollider;
    private Rigidbody2D rb;

    // �߷� ����
    private float gravity;
    // ���ǵ� ����
    private float saveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        isClimbing = false;
        myCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        // �߷°��� ���ǵ� ����
        gravity = rb.gravityScale;
        saveSpeed = monsterSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        // ������ �߿��� ������Ʈ ����
        if (isClimbing) return;

        // ���� ��ġ ����
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
        // ��: �߷� ���� ���̰ų� ���� �ӵ� ����
        rb.velocity += Vector2.down * gravity * Time.fixedDeltaTime;
    }

    public void Damage(int damage)
    {
        zombieHp -= damage;
        damageTxt.text = damage.ToString();

        // ������ UI ����
        StartCoroutine(DamageObjMov());

        // hp�� 0�̸� ����
        if (zombieHp <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hero"))
        {
            // Ʈ���� ������ �ӵ��� 0���� ����
            this.monsterSpeed = 0f;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Monster") || collision.gameObject.CompareTag("Monster_1"))
        {
            // �Ʒ� ���� ���̿� �ڽ��� ���� Ȯ��
            float otherY = collision.transform.position.y;
            float myY = transform.position.y;
            MonsterCtrl otherMonster = collision.collider.GetComponent<MonsterCtrl>();
            // �ڽ��� ���̺��� ���� �̵��ӵ��� 0�̸� ����
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

    // �տ� ���� Ȯ��
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

    // ���� ���ͼ� Ȯ��
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

    // ������ ���ͼ� Ȯ��
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

    // ������ �ڷ�ƾ
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

    // ������ UI �ڷ�ƾ
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
