using System.Collections;
using UnityEngine;
using MonsterType;
using TMPro;

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
    bool isLive;//생존 상태
    bool nowHit;//피격 상태
    bool nowStop;//정지 상태
    float hittime = 0.1f;
    public float speed;
    public float health;
    public float maxHealth;
    public float damage;
    private bool isCoroutineRunning_Hit = false;//Hit코루틴 중복실행 방지

    [Header("poison")]
    public int toxicity;//독성 중첩 상태. 0 이면 평시
    public float toxicityDamage;//독 데미지
    public float toxicityInterval = 1;//이 시간초마다 도트 데미지 입음
    public GameObject toxicityEffect;//중독 상태일때 머리위에 독 표시
    public TMP_Text toxicityStackText;//중독 중첩 수치 표시
    private Coroutine toxicityCoroutine;//중독 코루틴
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

        //중독 상태 초기화
        RemovePoison();

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
        if(nowStop == true)
            return;
        if(GameManager.instance.player.playerStatus.isLive != true)
            return;
        if(GameManager.instance.GamePlayState != true)
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
        transform.localScale = new Vector3(1,1,1);
        health = maxHealth;
        coll.enabled = true;
        rigid.simulated = true;
        isLive = true;
        nowHit = false;
        nowStop = false;
        toxicity = 0;//독성 초기화
        sprite.color = originalColor;//색상 초기화
        coll.isTrigger = false;
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
    public void DamageCalculator(float damage, bool isCritical){

        int damageOffSet = Random.Range(0,10);
        float finalDamage = (damage + damageOffSet);
        finalDamage = Mathf.Max(finalDamage, 1); // finalDamage 1 이하로 내려가지 않도록 설정
        health -= finalDamage;

        ShowDamageText(finalDamage,isCritical);
        if(health <= 0){
        death();

        }
        
    }

    public void ShowDamageText(float damage, bool isCritical)
    {
        Vector3 position = transform.position; // 기본 위치
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        
            if (spriteRenderer != null)
            {
                //position.y += spriteRenderer.bounds.extents.y;
                position = damageTextPos.transform.position;
            }

        DamageText damageText = GameManager.instance.damageTextPooling.Get(0).GetComponent<DamageText>(); // 오브젝트 풀에서 가져오기
        damageText.transform.position = position;
        damageText.value = damage;
        damageText.Init(isCritical);
    }
     public void ShowPosionDamageText(float damage)
    {
        Vector3 position = transform.position; // 기본 위치
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        
            if (spriteRenderer != null)
            {
                //position.y += spriteRenderer.bounds.extents.y;
                position = damageTextPos.transform.position;
            }
        bool nonCritical = false;
        DamageText damageText = GameManager.instance.damageTextPooling.Get(0).GetComponent<DamageText>(); // 오브젝트 풀에서 가져오기
        damageText.transform.position = position;
        damageText.value = damage;
        damageText.GetComponentInChildren<TMP_Text>().color = Color.green;
        damageText.Init(nonCritical);
       
    }

public void CallHitStop(){
 StartCoroutine(HitStop());
}
    IEnumerator HitStop(){

         if (isCoroutineRunning_Hit) yield break; // 중복 실행 방지
        isCoroutineRunning_Hit = true;
        //피격시 일시정지
        nowHit = true;
        Vector3 playerpos = GameManager.instance.playerMove.transform.position;
        Vector3 dirvec = transform.position - playerpos;
        rigid.AddForce(dirvec.normalized * 0.1f, ForceMode2D.Impulse);
        sprite.color = hitColor;
        yield return new WaitForSeconds(hittime);  // 0.1초 동안 빨갛게 변함
        nowHit = false;
        sprite.color = originalColor;
        isCoroutineRunning_Hit = false;
    }

    public void ApplyPoison()
    {
        if (toxicityCoroutine == null)
        {
            toxicityCoroutine = StartCoroutine(ApplyPoisonDamage());
            toxicityEffect.gameObject.SetActive(true);//중독 이펙트
        }
        
    }
     public void RemovePoison()
    {
        //몬스터 생성시 호출하여 초기화
        if (toxicityCoroutine != null)
        {
            StopCoroutine(toxicityCoroutine);
            toxicityCoroutine = null;
        }
            //중독 효과 초기화
            toxicityEffect.gameObject.SetActive(false);
            toxicity = 0;
            toxicityStackText.text = "";
        
    }
     private IEnumerator ApplyPoisonDamage()
    {
        while (toxicity >= 1)
        {
            float atk = GameManager.instance.player.playerStatus.ATK;
            int damage = Mathf.CeilToInt(atk * 0.1f * toxicity);
            TakePosionDamage(damage);
            yield return new WaitForSeconds(toxicityInterval);
        }

        toxicityCoroutine = null;
    }

    public void toxicityPlus(){
        //독성 상태 추가 중첩
        toxicity++;
        toxicityStackText.text = toxicity.ToString();
    }
     private void TakePosionDamage(int damage)
    {
        Debug.Log($"몬스터가 {damage} 중독 피해를 입음!");
        // HP 감소 로직 추가
        health -= damage;
        ShowPosionDamageText(damage);
        if(health <= 0){
        death();
            }
    }

    public void death(){
        isLive = false;
        nowHit = true; 
        coll.isTrigger = true;
        anim.SetBool("Dead", true);
        GameManager.instance.dataManager.expPoint += 103;
    }
   public void Deletemob(){
    switch(mobType){
        case MobType.normal:
                  //아이템 드랍
      if (Random.Range(0, 2) == 0) // 0 또는 1 반환, 50% 확률
    {
        int goldCoinPoolNum = 6;
        int randomOffSet =  Random.Range(1,5);
        GoldCoin gold = GameManager.instance.poolManager.Get(goldCoinPoolNum).GetComponent<GoldCoin>();
        gold.value = randomOffSet;
        gold.transform.position = transform.position;
    }
   // GameManager.instance.spawnManager.mobCount--;
    GameManager.instance.stageManager.CheckStageProgress();//스테이지 진행률 증가
    // 현재 오브젝트 비활성화
    gameObject.SetActive(false);
        break;

        case MobType.boss :
        GameManager.instance.stageManager.BossDeadEvent();
        gameObject.SetActive(false);
        break;

    }
    
  
   }
   private void OnCollisionStay2D(Collision2D other) {
        if(other.gameObject.CompareTag("Player")){
            player.playerCol.HitCalCulator(damage);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Stop")){
            anim.speed = 0;
            nowStop = true;
            rigid.linearVelocity = Vector2.zero; // Rigidbody2D 정지
            rigid.bodyType = RigidbodyType2D.Kinematic;// 물리적 반응 비활성화
        }
        if(other.gameObject.CompareTag("Cleaner")){
            gameObject.SetActive(false);//다음 스테이지 진입시 클리너 발동, 몬스터 전부 삭제 
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
         if(other.gameObject.CompareTag("Stop")){
            anim.speed = 1;
            nowStop = false;
             rigid.bodyType = RigidbodyType2D.Dynamic;// 물리적 반응 비활성화
        }
    }
}
