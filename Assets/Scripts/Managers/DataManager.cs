using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    //백엔드 서버와 이어주는 중간 연결다리
    public float goldPoint;//현재 플레이어의 골드량

    [Header("#Player Info_UpgradeLevel")]
    public int level_ATK;//공격력 레벨
    public int level_Hp;//최대 체력 레벨
    public int level_HpRecovery;//최력 회복량 레벨
    public int level_CriticalDamage;//치명타 배율 레벨
    public int level_CriticalPer;//치명타 확률 레벨
    [Header("#Stage Info")]
    public int stageProgressLevel;//저장된 스테이지 레벨


    [Header("#Weapon Info")]
    public List<Weapon> weaponList = new List<Weapon>();//웨폰 데이터 저장
    public int upgradePostionCount;//강화 포션 보유량

    [Header("#Card Info")]
     public List<Card> savedDeck = new List<Card>(); //덱 저장 항상 8개 유지, cardId - 1은 null값
      public List<Card> havedCardsList = new List<Card>(); //현재 보유한 모든 카드 모음
    


    private void Awake() {
        //임시로 레벨 세팅
        UpgradeLevelSetting();
        StageSetting();
        UpgradePostionCountSetting(1000);
    }
    public void Start()
    {
         DeckSetting();
    }

    public void UpgradeLevelSetting(){
        //게임이 시작될때 저장되어있던 param값을 받아서 UpgradeLevel을 세팅한다.
        level_ATK = 1;
        level_Hp = 1;
        level_HpRecovery = 1;
        level_CriticalDamage = 1;
        level_CriticalPer = 1;
    }

    public void StageSetting(){
        stageProgressLevel = 1;//저장된 스테이지 레벨
    }

    public void UpgradePostionCountSetting(int value){
      //서버에서 강화 포션 보유량을 받아서 적용
      upgradePostionCount = value;
    }
     public void DeckSetting(){
      //서버에서 현재 저장된 덱을 받아서 적용
        savedDeck.Add(new Card(0, 1, 1));
        savedDeck.Add(new Card(1, 1, 1));
        savedDeck.Add(new Card(2, 1, 1));
        savedDeck.Add(new Card(3, 1, 1));
        savedDeck.Add(new Card(4, 1, 1));
        savedDeck.Add(new Card(-1, 1, 1));
        savedDeck.Add(new Card(-1, 1, 1));
        savedDeck.Add(new Card(-1, 1, 1));

      GameManager.instance.deckManager.GetSavedDeck(savedDeck);
     }

     public int ReturnCardCount(int cardId){
      //카드 아이디를 입력하면 현재 해당 카드의 보유량을 리턴해주는 함수
      int returnNum = 0;//초기화
      for(int i = 0; i < savedDeck.Count; i++){
        if(savedDeck[i].ID == cardId){
          returnNum = savedDeck[i].COUNT;
        }
      }
      return returnNum;
     }

    public void ChageToRealValue(){
        //Upgrade 레벨을 받아서 실제 플레이어에게 적용되는 값으로 바꿔주는 함수.
        
        float real_ATK = level_ATK * 2;//1레벨당 증가량 2
        float real_HP = 100 + (level_Hp * 10);//1레벨당 증가량 10
        float real_HPRecovery = level_HpRecovery * 0.1f;//1레벨당 증가량 0.1%
        float real_CriticalDamage = level_CriticalDamage;//1레벨당 증가량 1%
        float real_criticalPer = level_CriticalPer * 0.1f;//1레벨당 증가량 0.1%

        //무기 장착 효과
        float equipWeaponEffectValue =  GameManager.instance.weaponManager.ReturnEquipEffect();

        //무기 보유 효과
        float ownedWeaponEffectValue = GameManager.instance.weaponManager.ReturnOwnedEffect();
        
        float finalWeaponEffectValue = equipWeaponEffectValue + ownedWeaponEffectValue;

        Player_Status player =  GameManager.instance.player.playerStatus;
        //ATK 참고 Real_ATK = 성장, 플레이어 레벨 ATK의 합
        player.ATK = real_ATK * (1 + (finalWeaponEffectValue / 100f));
        player.maxHealth = real_HP;
       // player.health += 10;//최대체력 증가량만큼 현재체력 회복
        player.healthRecoveryPer = real_HPRecovery;
        player.CriticalDamagePer = real_CriticalDamage;
        player.CriticalPer = real_criticalPer;

    }
    public void SyncStageLevelFromServer()
{
    // 데이터 매니저에서 받은 스테이지 레벨을 실제 스테이지 매니저에 적용
    GameManager.instance.stageManager.currentStageLevel = stageProgressLevel;
}

    public void SyncWeaponData()
    {
        //데이터 메니저에서 받은 웨폰 데이터를 실제 웨폰 메니저에 적용
      weaponList = GetWeaponsData();
      
    }
    public List<Weapon> GetWeaponsData(){
        //백엔드 서버에서 WeaponList를 받아옴, 우선 임시로 데이터 세팅
         List<Weapon> loadedWeapons = new List<Weapon>
        {
            new Weapon(0, 0, WeaponGrade.Common, 0, true, 100, true),
            new Weapon(1, 0, WeaponGrade.Common, 0, false, 0,false),
            new Weapon(2, 0, WeaponGrade.Common, 0, false, 0,false),
              new Weapon(3, 0, WeaponGrade.Common, 0, false, 0,false),
              //
                new Weapon(4, 0, WeaponGrade.Rare, 0, false, 50,true),
                  new Weapon(5, 0, WeaponGrade.Rare, 0, false, 0,false),
                    new Weapon(6, 0, WeaponGrade.Rare, 0, false, 0,false),
                      new Weapon(7, 0, WeaponGrade.Rare, 0, false, 0,false),
                      //
                        new Weapon(8, 0, WeaponGrade.Epic, 0, false, 30,true),
                          new Weapon(9, 0, WeaponGrade.Epic, 0, false, 0,false),
                            new Weapon(10, 0, WeaponGrade.Epic, 0, false, 0,false),
                              new Weapon(11, 0, WeaponGrade.Epic, 0, false, 0,false),
                              //
                                new Weapon(12, 0, WeaponGrade.Legendary, 0, false, 10,true),
                                  new Weapon(13, 0, WeaponGrade.Legendary, 0, false, 0,false),
                                    new Weapon(14, 0, WeaponGrade.Legendary, 0, false, 0,false),
                                      new Weapon(15, 0, WeaponGrade.Legendary, 0, false, 0,false)
        };
        return loadedWeapons;
    }
}


//Waepon Info
public enum WeaponGrade { Common, Rare, Epic, Legendary }
public class Weapon
{
    public int weaponId;      // 고유 번호
    public int upgradeLevel;  // 강화 수치
    public WeaponGrade grade; // 무기 등급
    public int stackCount;    // 중첩 수치
    public bool isEquipped;   // 장착 여부
     public int weaponCount;    // 보유량 수치
     public bool isAcquired; //획득 여부

     // 🔹 생성자 추가 (5개의 인수를 받도록 설정)
    public Weapon(int id, int level, WeaponGrade grade, int stack, bool equipped, int weaponCount, bool isAcquired)
    {
        this.weaponId = id;
        this.upgradeLevel = level;
        this.grade = grade;
        this.stackCount = stack;
        this.isEquipped = equipped;
        this.weaponCount = weaponCount;
        this.isAcquired = isAcquired;
    }
}
