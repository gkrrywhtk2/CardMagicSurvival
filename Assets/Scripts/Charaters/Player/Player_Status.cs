using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Player_Status : MonoBehaviour
{
    [Header("#플레이어의 상태값")]
    public bool isLive;
    public float health;//현재 체력
    private Coroutine regenCoroutine; // 실행 중인 체력 회복 코루틴
    [Header("#일반 성장 능력치 ")]
    public float ATK;//공격력
    public float maxHealth = 100;//최대 체력
    public float healthRecoveryPer;//초당 체력 회복량
    public float CriticalDamagePer;//치명타 배율
    public float CriticalPer;//치명타 확율

    [Header("#특수 성장 능력치 ")]
    public float mana;
    public float maxMana = 9;
    public float baseManaRecovery;//기본 마나회복량; 일단 0.5로 세팅하였음 초당 0.5회복
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

    //메인 UI 슬라이드_ SlideLine 오브젝트에 있다.
    public Slider hpBar_UI;
    public Slider manaBar_UI;
    public TMP_Text nowHpText_UI;
    public TMP_Text nowManaText_UI;
     

    private void Awake() {
       

    }
    public void PlayerInit(){
        //게임 시작시 플레이어 변수 초기화
        isLive = true;
        health = maxHealth;
        StartHealthRegen();
    }
    private void HpBarUpdate()
    {
        hpBar.value = health / maxHealth;
        hpBar_UI.value = hpBar.value;

        // 🔹 체력을 소수점 1자리까지 표시
        nowHpText_UI.text = health.ToString("F1");

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
      //  GameManager.instance.deckManager.StartUpgradeEvent();//카드 랜덤 선택 이벤트

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
        manaBar_UI.value = manaBar.value;
        nowManaText_UI.text = currentMana.ToString();//UI 마나바에 적용
        

        if (mana < maxMana)
            return;

        if (mana > maxMana)
        {
            mana = maxMana;
            return;
        }

    }
   public void StartHealthRegen()
{
    if (regenCoroutine == null) // 중복 실행 방지
    {
        regenCoroutine = StartCoroutine(HealthRecoveryCoroutine());
    }
}
    private IEnumerator HealthRecoveryCoroutine()
{
    while (true)
    {
         // 일시정지 상태일 경우, 대기 (코루틴이 종료되지 않도록 함)
       yield return new WaitUntil(() => GameManager.instance.GamePlayState == true);

       float recoveryAmount = maxHealth * (healthRecoveryPer * 0.01f);
        health = Mathf.Min(health + recoveryAmount, maxHealth);
   // Debug.Log("gasdasfasf");
        yield return new WaitForSeconds(1f); // 1초 대기 후 반복
    }
}
public void StopHealthRegen()
{
    if (regenCoroutine != null)
    {
        StopCoroutine(regenCoroutine);
        regenCoroutine = null;
    }
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

    //이동속도 변경 관련 로직*****
    public List<float> speedUpEffects = new List<float>(); //이동 속도 증가량 리스트 모음
    private float totalspeedUpMultiplier ; // 기본값0


   
public void AddSpeedUpEffect(float value)
{
    speedUpEffects.Add(value); // % 단위를 배수로 변환 (ex: 100% -> 1.0f)
    UpdateTotalSpeedUpMultiplier(); // 최신 값 반영
}

// 🔹 마나 회복 증가 효과 제거 (ex: 장비 해제, 카드 효과 만료)
public void RemoveSpeedUpEffect(float value)
{
    speedUpEffects.Remove(value);
    UpdateTotalSpeedUpMultiplier(); // 최신 값 반영
}

// 🔹 효과가 변경될 때마다 총 배율을 업데이트
private void UpdateTotalSpeedUpMultiplier()
{
    totalspeedUpMultiplier = 0; // 기본값0
    foreach (float value in speedUpEffects)
    {
        totalspeedUpMultiplier += value;
    }
    GameManager.instance.player.joystickP.totalSpeedUpPlusValue = totalspeedUpMultiplier;//추가 속도 적용
}

  //이동 속도 변경 관련 로직 끝



   // 마나 회복 관련 로직 시작********
public List<float> manaRecoveryEffects = new List<float>(); // 마나 증가량 효과 버프 모음
private float totalManaRecoveryMultiplier = 1f; // 기본값 100% (배수 개념)

public void ManaRecovery()
{
    if (!GameManager.instance.GamePlayState || GameManager.instance.ItemSelectState)
        return;

    // 🔹 매 프레임마다 초기화 후 효과를 다시 계산해야 함
    totalManaRecoveryMultiplier = 1f; // 기본값 100% (누적 방지)

    foreach (float effect in manaRecoveryEffects)
    {
        totalManaRecoveryMultiplier += effect; // 효과를 누적
    }

    // 마나 회복 적용
    mana += baseManaRecovery * totalManaRecoveryMultiplier * Time.deltaTime;
}

// 🔹 마나 회복 증가 효과 추가
public void AddManaRecoveryEffect(float percent)
{
    manaRecoveryEffects.Add(percent / 100f); // % 단위를 배수로 변환 (ex: 100% -> 1.0f)
    UpdateTotalManaRecoveryMultiplier(); // 최신 값 반영
}

// 🔹 마나 회복 증가 효과 제거 (ex: 장비 해제, 카드 효과 만료)
public void RemoveManaRecoveryEffect(float percent)
{
    manaRecoveryEffects.Remove(percent / 100f);
    UpdateTotalManaRecoveryMultiplier(); // 최신 값 반영
}

// 🔹 효과가 변경될 때마다 총 배율을 업데이트
private void UpdateTotalManaRecoveryMultiplier()
{
    totalManaRecoveryMultiplier = 1f; // 기본값 100%
    foreach (float effect in manaRecoveryEffects)
    {
        totalManaRecoveryMultiplier += effect;
    }
}

// 마나 회복 관련 로직 끝***********************//







  public float DamageReturn(float skillPower, out bool isCritical)
{
    // 캐릭터의 공격력 가져오기
    float ATK = GameManager.instance.player.playerStatus.ATK;

    // 랜덤 오프셋 적용 (-5% ~ +5% 변동)
    float randomOffset = Random.Range(ATK * -0.05f, ATK * 0.05f);

    // 기본 데미지 계산
    float Damage_one = (ATK + randomOffset) * (skillPower / 100f);

    // 치명타 확률 계산 (totalCriticalMultiplier 포함)
    isCritical = CriticalReturn();

    // 최종 데미지 계산 (치명타 적용)
    float finalDamage = isCritical ? Damage_one * (CriticalDamagePer / 100f) : Damage_one;

    return finalDamage;
}


    //추가 치명타 확률 로직 ***********************//
     public List<float> criticalEffects = new List<float>(); // 추가 치명타 효과 리스트
    private float totalCriticalMultiplier = 0f; // 추가 치명타 확률

    public bool CriticalReturn()
    {
        // 치명타 확률 계산 (기본 확률 + 추가 확률)
        float finalCriticalChance = CriticalPer + totalCriticalMultiplier;
        return Random.Range(0f, 100f) < finalCriticalChance;
    }

    // 치명타 확률 증가 효과 추가
    public void AddCriticalEffect(float value)
    {
        criticalEffects.Add(value);
        UpdateTotalCriticalMultiplier();
    }

    // 치명타 확률 증가 효과 제거
    public void RemoveCriticalEffect(float value)
    {
        if (criticalEffects.Contains(value)) // 값이 존재하는지 확인 후 제거
        {
            criticalEffects.Remove(value);
            UpdateTotalCriticalMultiplier();
        }
    }

    // 총 치명타 확률 증가량 업데이트
    private void UpdateTotalCriticalMultiplier()
    {
        totalCriticalMultiplier = 0f; // 기본값 0
        foreach (float effect in criticalEffects)
        {
            totalCriticalMultiplier += effect;
        }
    }

    //치명타 로직 종료*****************

}
