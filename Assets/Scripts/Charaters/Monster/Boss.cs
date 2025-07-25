using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
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
    bool nowStop;//정지 상태
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

      public void Init(BossSpawnData data)
    {   
       
        anim.runtimeAnimatorController = animCon[data.boss_id];
        speed = data.speed;
        maxHealth = data.maxHealth;
        health = maxHealth;
        damage = data.damage;

        //중독 상태 초기화
        nowPoison = false;
        posionDamage = 0;
        dotDamageCoolTime = 0;

       
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
        health = maxHealth;
        coll.enabled = true;
        rigid.simulated = true;
        isLive = true;
        nowHit = false;
        nowStop = false;
    }
     public void DamageCalculator(float damage, bool isCritical){
        health -= damage;
        ShowDamageText(damage,isCritical);
        if(health <= 0){
        death();
        }else if(nowPoison != true){
        StartCoroutine(HitStop());
        }else if(nowPoison == true){
            //여기 할 차례임
        }
    }
      public void ShowDamageText(float damage, bool isCritical)
{
    Vector3 position = transform.position; // 기본 위치
    if (coll != null)
    {
     
        position.y += (coll.bounds.extents.y) * 3; // 몬스터의 상단에 텍스트 배치
    }
    else
    {
        position.y += 1f; // 기본값 (콜라이더 없을 때)
    }

    DamageText damageText = GameManager.instance.damageTextPooling.Get(0).GetComponent<DamageText>(); // 오브젝트 풀에서 가져오기
    damageText.transform.position = position;
    damageText.value = damage;
    damageText.Init(isCritical);
    

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
   public void Deletemob(){
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
    }
    private void OnTriggerExit2D(Collider2D other) {
         if(other.gameObject.CompareTag("Stop")){
            anim.speed = 1;
            nowStop = false;
             rigid.bodyType = RigidbodyType2D.Dynamic;// 물리적 반응 비활성화
        }
    }
   
}
