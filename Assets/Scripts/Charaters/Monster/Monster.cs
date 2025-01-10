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
    public Color hitColor;
    public RuntimeAnimatorController[] animCon;

    [Header("Scaner")]
    public Rigidbody2D moveTarget;
    public Player_Main player;
   
     [Header("Stat")]
    bool isLive;//생존 상태
    bool nowHit;//피격 상태
    float hittime = 0.1f;
    public float speed;
    public float health;
    public float maxHealth;
    public float damage;

    [Header("poison")]
    public bool nowPoison;//현재 중독 상태이상인지 체크용
    public float posionDamage;//독 데미지
    public float dotDamageCoolTime;//이 시간초마다 도트 데미지 입음
    public GameObject poisonEffect;//중독 상태일때 머리위에 독 표시
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

        //중독 상태 초기화
        nowPoison = false;
        posionDamage = 0;
        dotDamageCoolTime = 0;

        //몬스터들 애니메이션 겹치지 않게끔.
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
        if(GameManager.instance.player.playerStatus.isLive != true)
            return;
        if(GameManager.instance.inGamePlay != true)
        return;

        Vector2 moveVec = moveTarget.position - rigid.position;
        Vector2 nextVec = moveVec.normalized * speed * Time.fixedDeltaTime;
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
        player = GameManager.instance.player.GetComponent<Player_Main>();
        health = maxHealth;
        coll.enabled = true;
        rigid.simulated = true;
        isLive = true;
         nowHit = false;
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
    public void DamageCalculator(float damage){
        health -= damage;
        if(health <= 0){
        death();
        }else if(nowPoison != true){
        StartCoroutine(HitStop());
        }else if(nowPoison == true){
            //여기 할 차례임
        }
    }
    IEnumerator HitStop(){
        //피격시 일시정지
        nowHit = true;
        Vector3 playerpos = GameManager.instance.playerMove.transform.position;
        Vector3 dirvec = transform.position - playerpos;
        rigid.AddForce(dirvec.normalized * 0.1f, ForceMode2D.Impulse);
        sprite.color = hitColor;
        yield return new WaitForSeconds(hittime);  // 0.1초 동안 빨갛게 변함
        nowHit = false;
        sprite.color = originalColor;
    }

    public void PoisonOn(float damage){
        //몬스터 중독 상태 ON
        nowPoison = true;
    }

     //IEnumerator PoisonDamage(){
//
    // }

    public void death(){
        isLive = false;
       nowHit = true;
        anim.SetBool("Dead", true);
    }
    public void destroy(){
        gameObject.SetActive(false);
    }

   private void OnCollisionStay2D(Collision2D other) {
        if(other.gameObject.CompareTag("Player")){
            player.playerCol.HitCalCulator(damage);
        }
    }
}
