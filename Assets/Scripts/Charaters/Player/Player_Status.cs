using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Player_Status : MonoBehaviour
{
    public DataManager dataManager;
    public UpgradeUI upgradeUI;
    AccessoryManager accessoryManager;
    [Header("#플레이어의 상태값")]
    public bool isLive;
    public float health;//현재 체력
    private Coroutine regenCoroutine; // 실행 중인 체력 회복 코루틴
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
    public Slider attackBar;
    public Slider hpBar;
    public Slider MagicArrow;

    public Slider hpBar_UI;
    public TMP_Text nowHpText_UI;

  void Start()
  {
    accessoryManager = GameManager.instance.accessoryManager;
  }

  public void PlayerInit(){
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
        hpBar_UI.value = hpBar.value;

        // 🔹 체력을 자연수로 표시
        nowHpText_UI.text = Mathf.FloorToInt(health).ToString();
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
        AttackBarUpdate();
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
            if (!GameManager.instance.GamePlayState || GameManager.instance.ItemSelectState)
                return;

            float traningManaRecovery = upgradeUI.Traning_MRP_Setting();

            // 기본 회복량 + 추가 회복량 적용
            mana += (baseManaRecovery + totalFlatManaRecovery + traningManaRecovery) * Time.deltaTime;
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
        float finalDamage = isCritical ? basicDamage + (basicDamage * (GetTotalCriDamage() / 100f)) : basicDamage;
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
    public float GetTotalATK()
    {
        // 기본 성장ATK 및 훈련ATK 계산
        float totalATK = (dataManager.mainData.atk * 2) + (dataManager.traningData.atk * 5);
        //Debug.Log("ToTalATK = " + totalATK);

        // 무기 장착 효과 및 보유 효과를 한 번만 계산하여 저장
        float equipWeaponEffectValue = GameManager.instance.weaponManager.ReturnEquipEffect();
        float ownedWeaponEffectValue = GameManager.instance.weaponManager.ReturnOwnedATKEffect();
        Debug.Log("ownedWeaponEffectValueATK : " + ownedWeaponEffectValue);

        // 무기 효과 값 계산
        float finalWeaponEffectValue = equipWeaponEffectValue + ownedWeaponEffectValue;

        // 최종 공격력에 적용
        totalATK *= 1 + (finalWeaponEffectValue / 100f); // 백분율로 변환하여 적용
       // Debug.Log("finalWeaponvalue = "+finalWeaponEffectValue );
        //  Debug.Log("ToTalATK + WeaponValue= " + totalATK);
        this.totalATK = totalATK;
        return totalATK;
    }
    public float GetTotalCriDamage()
    {
        float baseCriDamage = upgradeUI.CriticalDamage_Setting() + upgradeUI.Traning_CRI_Setting();

        float ownedWeaponEffectValue = GameManager.instance.weaponManager.ReturnOwnedCRIEffect(); // ex: 50
        float ownedAccEffectValue = accessoryManager.ReturnOwnedCRIEffect(); // ex: 50
        float equipedAccEffectValue = accessoryManager.ReturnEquipEffect_CRI();

        float totalBonusPercent = ownedWeaponEffectValue + ownedAccEffectValue + equipedAccEffectValue;
        Debug.Log("ownedWeaponEffectValue : " + ownedWeaponEffectValue);
        Debug.Log("ownedAccEffectValue : " + ownedAccEffectValue);
        Debug.Log("equipedAccEffectValue : " + equipedAccEffectValue);

        totalCriDamage = baseCriDamage + totalBonusPercent;
       
        return totalCriDamage;
    }

    public float GetMaxHealth(){
        int baseHp = 100;//기본 체력 100
        float upgradeHp = upgradeUI.MaxHp_Setting();
        float traningHp = upgradeUI.Traning_HP_Setting();
        float accValue0 = accessoryManager.ReturnEquipEffect_HP();
        float accValue1 = accessoryManager.ReturnOwnedHPEffect();
        maxHealth = baseHp + upgradeHp + traningHp + accValue0 + accValue1;
        return maxHealth;
    }
    public int GetLUK(){
        float traningValue = upgradeUI.Traning_LUK_Setting();
        float accValue = accessoryManager.ReturnEquipEffect_LUK() + 
            accessoryManager.ReturnOwnedLUKEffect();

        LUK = (int)traningValue + (int)accValue;
        return LUK;
    }
    public float GetCriChance(){
        float totalCriPer = upgradeUI.CriticalPer_Setting();
        totalCriChance = totalCriPer + totalCriticalMultiplier;
        return totalCriChance;
    }
    public float GetVIT(){
        float recoveryAmount0 = upgradeUI.HpRecovery_Setting() + upgradeUI.Traning_VIT_Setting();//성장 수치
        float recoveryAmount1 = accessoryManager.ReturnOwnedVITEffect();//악세 보유 수치
        float recoveryAmount2 = accessoryManager.ReturnEquipEffect_VIT();//악세 착용 수치
        this.VIT  = recoveryAmount0 + recoveryAmount1 + recoveryAmount2;
        return VIT;
    }

    public void InitALLStat()
    {
        //모든 스탯 초기화
        GetTotalATK();
        GetTotalCriDamage();
        GetMaxHealth();
        GetLUK();
        GetCriChance();
        GetVIT();
    }
    
    public void ATKTest()
    {
        totalATK += 10;
    }
}
