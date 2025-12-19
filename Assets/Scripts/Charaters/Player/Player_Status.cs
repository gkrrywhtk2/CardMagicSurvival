using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Player_Status : MonoBehaviour
{
    public DataManager dataManager;

    //AccessoryManager accessoryManager;
    [Header("#플레이어의 상태값")]
    public bool isLive;
    public float health;//현재 체력
    public bool gameStop;//게임이 일시정지 중이면 true 입니다.
    private Coroutine regenCoroutine; // 실행 중인 체력 회복 코루틴
    public Player_EXP playerEXP;//플레이어 경험치 스크립트 연동
    [Header("#능력치 ")]
    public float totalATK;//공격력
    public float maxHealth;//최대 체력
    public float totalVIT;//초당 체력 회복량
    public float totalCriDamage;//치명타 배율
    public float totalCriChance;//치명타 확율
    public int LUK;//골드 추가 획득량
    public float VIT;

    [Header("#특수 성장 능력치 ")]
    public float _mana;//내부 변수
    public float mana
    {
        get => _mana;
        set
        {
            // 최소 0, 최대 maxMana 사이로 제한
            _mana = Mathf.Clamp(value, 0f, maxMana);
        }
    }
    public float maxMana = 9;
    public float baseManaRecovery;//기본 마나회복량; 일단 0.5로 세팅하였음 초당 0.5회복
    [Header("Bar")]
    public Slider hpBar;
    public void PlayerInit()
    {
        //게임 시작시 플레이어 변수 초기화
        isLive = true;
        GetMaxHealth();
        health = maxHealth;
        StartHealthRegen();
    }
    private void HpBarUpdate()
    {
        // 체력 바 갱신
        hpBar.value = health / maxHealth;
    }

    
    public void LevelUpEvent(){
        GameManager.instance.Pause();
        //GameManager.instance.spawnManager.spawnAllow = false; //소환 중지를 gameplayerstae에 종속시켰음.
        //GameManager.instance.itemManager.SpawnItems_(); 아이템 스폰 기능 짜쳐서 버림
      //  GameManager.instance.deckManager.StartUpgradeEvent();//카드 랜덤 선택 이벤트

    }
    // private void AttackBarUpdate(){
    //     float automaxpoint = GameManager.instance.player.autoAttack.autoAttackMaxPoint;
    //     float autocurpoint = GameManager.instance.player.autoAttack.autoAttackCurrentPoint;
    //     attackBar.value = autocurpoint/automaxpoint;
    //     MagicArrow.value = autocurpoint/automaxpoint;

    // }

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
        health = Mathf.Min(health + GetVIT(), maxHealth);
      // Debug.Log("Heal : " + GetVIT());
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
        //ManaBarUpdate();
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
        public List<float> manaRecoveryFlatBonusList = new List<float>(); // 고정 마나 회복 보너스
        private float totalFlatManaRecovery = 0f; // 총 고정 회복량

        public void ManaRecovery()
        {
            if (!GameManager.instance.GamePlayState)
                return;

            // 기본 회복량 + 추가 회복량 적용
            mana += (baseManaRecovery + totalFlatManaRecovery ) * Time.deltaTime;
        }

        // 🔹 고정 마나 회복량 추가
        public void AddManaRecoveryFlat(float value)
        {
            manaRecoveryFlatBonusList.Add(value);
            UpdateFlatManaRecovery();
        }

        // 🔹 고정 마나 회복량 제거
        public void RemoveManaRecoveryFlat(float value)
        {
            manaRecoveryFlatBonusList.Remove(value);
            UpdateFlatManaRecovery();
        }

        // 🔹 총 고정 마나 회복량 업데이트
        private void UpdateFlatManaRecovery()
        {
            totalFlatManaRecovery = 0f;
            foreach (float value in manaRecoveryFlatBonusList)
            {
                totalFlatManaRecovery += value;
            }
        }


// 마나 회복 관련 로직 끝***********************//







    public float DamageReturn(float skillPower, out bool isCritical)
    {
        // 랜덤 오프셋 적용 (-5% ~ +5% 변동)
        float randomOffset = Random.Range(totalATK * -0.05f, totalATK * 0.05f);

        // 기본 데미지 계산
        float basicDamage = (totalATK + randomOffset) * (skillPower / 100f);

        // 치명타 확률 계산 (totalCriticalMultiplier 포함)
        isCritical = CriticalReturn();

        // 최종 데미지 계산 (치명타 적용)
        float finalDamage = isCritical ? basicDamage + basicDamage  : basicDamage;
        //Debug.Log("CriticalDamage :" + GetTotalCriDamage());
        return finalDamage;
    }

  // 총 공격력 계산
    



    //추가 치명타 확률 로직 ***********************//
     public List<float> criticalEffects = new List<float>(); // 추가 치명타 효과 리스트
    private float totalCriticalMultiplier = 0f; // 추가 치명타 확률

    public bool CriticalReturn()
    {
        // 치명타 확률 계산 (기본 확률 + 추가 확률)
        return Random.Range(0f, 100f) < totalCriChance;
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


    /**
    Get 함수 모음
    **/
    


    public float GetMaxHealth(){
        int baseHp = 100;//기본 체력 100
        return baseHp;
    }
    public int GetLUK(){
        float traningValue = 1;
        // float accValue = accessoryManager.ReturnEquipEffect_LUK() + 
        //     accessoryManager.ReturnOwnedLUKEffect();

        LUK = (int)traningValue;
        return LUK;
    }
    public float GetCriChance(){
        float totalCriPer = 1;
        totalCriChance = totalCriPer + totalCriticalMultiplier;
        return totalCriChance;
    }
    public float GetVIT(){
        float recoveryAmount0 = 1;
        return recoveryAmount0;
    }

    public void InitALLStat()
    {
        //모든 스탯 초기화

        GetMaxHealth();
    }
    
}
