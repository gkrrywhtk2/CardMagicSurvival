using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player_Status : MonoBehaviour
{
    [Header("#플레이어의 상태값")]
    public bool isLive;
    public float health;//현재 체력
    [Header("#일반 성장 능력치 ")]
    public float ATK;//공격력
    public float maxHealth = 100;//최대 체력
    public float healthRecoveryPer;//초당 체력 회복량
    public float CriticalDamagePer;//치명타 배율
    public float CriticalPer;//치명타 확율

    [Header("#특수 성장 능력치 ")]
    public float mana;
    public float maxMana = 10;
    public float manaRecovery;
    public float manaRecoveryPlus;
    public float maxexp;
    public float nowexp;
    public int playLevel;
    [Header("Bar")]
    public Slider attackBar;
    public Slider hpBar;
    public Slider manaBar;
    public Slider expBar;
    public TMP_Text manaText;
    public Slider MagicArrow;

    private void Awake() {
        isLive = true;
        health = maxHealth;
      
    }

    private void HpBarUpdate()
    {
        hpBar.value = health / maxHealth;

        if (health < maxHealth)
            return;

        if (health > maxHealth)
        {
            health = maxHealth;
            return;
        }

    }

     private void ExpBarUpdate()
    {
        expBar.value = nowexp / maxexp;

        if(nowexp >= maxexp){
            nowexp = 0;
            playLevel += 1;
            LevelUpEvent();
        }

    }
    public void LevelUpEvent(){
        GameManager.instance.Pause();
        //GameManager.instance.spawnManager.spawnAllow = false; //소환 중지를 gameplayerstae에 종속시켰음.
        //GameManager.instance.itemManager.SpawnItems_(); 아이템 스폰 기능 짜쳐서 버림
        GameManager.instance.deckManager.StartUpgradeEvent();//카드 랜덤 선택 이벤트

    }
    private void AttackBarUpdate(){
        float automaxpoint = GameManager.instance.player.autoAttack.autoAttackMaxPoint;
        float autocurpoint = GameManager.instance.player.autoAttack.autoAttackCurrentPoint;
        attackBar.value = autocurpoint/automaxpoint;
        MagicArrow.value = autocurpoint/automaxpoint;

    }

    private void ManaBarUpdate()
    {
        int currentMana = Mathf.FloorToInt(mana); // 정수로 변환
        manaText.text = currentMana.ToString();
        manaBar.value = mana / maxMana;

        if (mana < maxMana)
            return;

        if (mana > maxMana)
        {
            mana = maxMana;
            return;
        }

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isLive != true)
            return;
        HpBarUpdate();
        AttackBarUpdate();
        ManaBarUpdate();
        ManaRecovery();
       // ExpBarUpdate(); 경험치 삭제 예정
    }

    public void ManaRecovery(){
        if(GameManager.instance.GamePlayState != true)
        return;
        if(GameManager.instance.ItemSelectState == true)
        return;

        mana += (manaRecovery+ manaRecoveryPlus) * Time.deltaTime;
    }

    public float DamageReturn(float skillPower) {
    // 캐릭터의 공격력 가져오기
    float ATK = GameManager.instance.player.playerStatus.ATK;

    // 랜덤 오프셋 적용 (0%~10% 변동)
    float randomOffset = Random.Range(0, ATK * 0.1f);

    // 최종 데미지 계산 (기본 공격력 + 변동 값) * 스킬 배율
    float finalDamage = (ATK + randomOffset) * skillPower;

    return finalDamage;
}
}
