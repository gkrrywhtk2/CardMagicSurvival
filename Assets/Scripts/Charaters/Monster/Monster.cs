using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
[Header("Components")]
    SpriteRenderer sprite;
    Rigidbody2D rigid;
    Collider2D coll;
    Animator anim;
    private Color originalColor;
    public RuntimeAnimatorController[] animCon;

    [Header("Scaner")]
    public Rigidbody2D moveTarget;
     [Header("Stat")]
    bool isLive;//생존 상태
    bool nowHit;//피격 상태
    public float speed;
    public float health;
    public float maxHealth;
    public float attackPower;
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
        RandomizeAnimation();
    }
     private void FixedUpdate()
    {
        MoveToPlayer();
    }
     private void MoveToPlayer()
    {
        if (isLive != true)
            return;
        if (nowHit == true)
            return;

        Vector2 dirVec = moveTarget.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        //rigid.velocity = Vector2.zero;
    }
     private void LateUpdate()
    {
        //진행 방향에 따른 좌우 반전
        sprite.flipX = moveTarget.position.x < rigid.position.x;
    }
    private void OnEnable()
    {
        //재생성 될때마다 초기화
        moveTarget = GameManager.instance.player.GetComponent<Rigidbody2D>();
        health = maxHealth;
        coll.enabled = true;
        rigid.simulated = true;
        isLive = true;
    }

     private void RandomizeAnimation()
    {
        if (anim != null)
        {
            // 0~1 사이의 랜덤 값을 생성하여 애니메이션 진행 상태를 랜덤화
            float randomStartTime = Random.Range(0f, 1f);
            anim.Play(0, -1, randomStartTime);
        }
    }
}
