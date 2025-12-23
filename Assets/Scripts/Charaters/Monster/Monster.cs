using System.Collections;
using UnityEngine;
using MonsterType;
using TMPro;

public enum DamageType
{
    Normal,
    Poison
}

public class Monster : MonoBehaviour
{
    [Header("Components")]
    public MobType mobType;
    SpriteRenderer sprite;
    Rigidbody2D rigid;
    Collider2D coll;
    Animator anim;
    private Color originalColor;
    public Color hitColor;
    public RuntimeAnimatorController[] animCon;
    public Transform damageTextPos;

    [Header("Scaner")]
    public Rigidbody2D moveTarget;
    public Player_Main player;

    [Header("Stat")]
    [SerializeField] private bool isLive; // 외부에서 상태 읽을 수 있게 프로퍼티 제공
    bool nowHit;
    bool nowStop;
    float hittime = 0.1f;
    public float speed;
    public float health;
    public float maxHealth;
    public float damage;
    private bool isCoroutineRunning_Hit = false;

    // ✅ 외부 컴포넌트(PoisonStatus 등)에서 읽기만
    public bool IsLive => isLive;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        originalColor = sprite.color;
    }

    public void Init(MobSpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.mob_id];
        speed = data.speed;
        maxHealth = data.maxHealth;
        health = maxHealth;
        damage = data.damage;
        mobType = data.mobType;

        RandomizeAnimation();
    }

    private void FixedUpdate()
    {
        MoveToPlayer();
    }

    private void MoveToPlayer()
    {
        if (!isLive) return;
        if (nowHit) return;
        if (nowStop) return;
        if (!GameManager.instance.player.playerStatus.playerHP.isLive) return;
        if (!GameManager.instance.GamePlayState) return;

        Vector2 moveVec = moveTarget.position - rigid.position;
        Vector2 nextVec = moveVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    private void LateUpdate()
    {
        sprite.flipX = moveTarget.position.x < rigid.position.x;
    }

    private void OnEnable()
    {
        moveTarget = GameManager.instance.player.GetComponent<Rigidbody2D>();
        player = GameManager.instance.player.GetComponent<Player_Main>();

        transform.localScale = new Vector3(1, 1, 1);
        health = maxHealth;

        coll.enabled = true;
        rigid.simulated = true;

        isLive = true;
        nowHit = false;
        nowStop = false;

        sprite.color = originalColor;
        coll.isTrigger = false;

        // ✅ 상태이상 컴포넌트 초기화가 필요하면 컴포넌트가 스스로 OnDisable/Clear 하게 두는게 베스트
        // (원하면 여기서 GetComponent<PoisonStatus>()?.Clear(); 호출해도 역할 분리 유지됨)
    }

    private void RandomizeAnimation()
    {
        if (anim != null)
        {
            float randomStartTime = Random.Range(0f, 1f);
            anim.Play(0, -1, randomStartTime);
        }
    }

    // =========================
    // 피해 처리 (통합)
    // =========================

    // ✅ 기존 코드 호환용: 다른 곳에서 DamageCalculator 호출하고 있을 수 있어서 남겨둠
    public void DamageCalculator(float damage, bool isCritical)
    {
        TakeDamage(damage, isCritical, DamageType.Normal);
    }

    // ✅ 앞으로는 이거로 통일 추천
    public void TakeDamage(float baseDamage, bool isCritical, DamageType type)
    {
        if (!isLive) return;

        float finalDamage = baseDamage;

        // 정책: 일반 데미지에만 랜덤 오프셋
        if (type == DamageType.Normal)
        {
            int damageOffSet = Random.Range(0, 10);
            finalDamage += damageOffSet;
        }

        finalDamage = Mathf.Max(finalDamage, 1f);
        health -= finalDamage;

        if (type == DamageType.Poison)
            ShowPoisonDamageText(finalDamage);
        else
            ShowDamageText(finalDamage, isCritical);

        if (health <= 0) death();
    }

    public void ShowDamageText(float damage, bool isCritical)
    {
        Vector3 position = damageTextPos != null ? damageTextPos.position : transform.position;

        DamageText dt = GameManager.instance.damageTextPooling.Get(0).GetComponent<DamageText>();
        dt.transform.position = position;
        dt.value = damage;
        dt.Init(DamageType.Normal, isCritical);
    }

    public void ShowPoisonDamageText(float damage)
    {
        Vector3 position = damageTextPos != null ? damageTextPos.position : transform.position;

        DamageText dt = GameManager.instance.damageTextPooling.Get(0).GetComponent<DamageText>();
        dt.transform.position = position;
        dt.value = damage;
        dt.Init(DamageType.Poison, false); 
    }

    // =========================
    // 피격 연출
    // =========================

    public void CallHitStop()
    {
        StartCoroutine(HitStop());
    }

    IEnumerator HitStop()
    {
        if (isCoroutineRunning_Hit) yield break;
        isCoroutineRunning_Hit = true;

        nowHit = true;

        Vector3 playerpos = GameManager.instance.player.transform.position;
        Vector3 dirvec = transform.position - playerpos;
        rigid.AddForce(dirvec.normalized * 0.1f, ForceMode2D.Impulse);

        sprite.color = hitColor;
        yield return new WaitForSeconds(hittime);

        nowHit = false;
        sprite.color = originalColor;
        isCoroutineRunning_Hit = false;
    }

    public void death()
    {
        isLive = false;
        nowHit = true;
        coll.isTrigger = true;
        anim.SetBool("Dead", true);
    }

    public void Deletemob()
    {
        switch (mobType)
        {
            case MobType.normal:
                if (Random.Range(0, 2) == 0)
                {
                    int ExpGemNum = 5;
                    int randomOffSet = Random.Range(80, 100);
                    EXP_GEM expGem = GameManager.instance.objectPooling.Get(ExpGemNum).GetComponent<EXP_GEM>();
                    expGem.value = randomOffSet;
                    expGem.transform.position = transform.position;
                }

                gameObject.SetActive(false);
                break;

            case MobType.boss:
                // boss 처리
                break;
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player.playerCol.HitCalCulator(damage);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Stop"))
        {
            anim.speed = 0;
            nowStop = true;
            rigid.linearVelocity = Vector2.zero;
            rigid.bodyType = RigidbodyType2D.Kinematic;
        }

        if (other.gameObject.CompareTag("Cleaner"))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Stop"))
        {
            anim.speed = 1;
            nowStop = false;
            rigid.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}
