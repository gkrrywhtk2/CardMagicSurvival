using System.Collections.Generic;
using UnityEngine;
using System.Linq; // LINQ 사용

public class DataManager : MonoBehaviour
{
    //백엔드 서버와 이어주는 중간 연결다리
    public float goldPoint;//현재 플레이어의 골드량
    public int rubyPoint;//현재 플레이어의 루비 소지량


    [Header("#Player Info_UpgradeLevel")]
    //훈련 스탯
    public Traning_PlayerStatData traningData;
    public Main_PlayerStatData mainData;


    [Header("#Stage Info")]
    public int stageProgressLevel;//저장된 스테이지 레벨


    [Header("#Equiped Info")]
    public List<Weapon> weaponList = new List<Weapon>();//웨폰 데이터 저장
    public List<Accessory> acceossryList = new List<Accessory>();//악세 데이터 저장
    public int upgradePostionCount;//강화 포션 보유량


    // [Header("#Card Info")]
    //  public List<Card> allCardList = new List<Card>(); //모든 카드 모음
    //  public List<int>[] savedDeck = new List<int>[5]; //덱 저장 항상 8개 유지, cardId - 1은 null값
    // public int selectedPresetDeck = 0;//현재 선택된 프리셋 넘버
    // public List<Card> havedCardsList = new List<Card>(); //현재 보유한 모든 카드 모음

    //Header("#Rank Datas")]
    //public RankDatas rankDatas;

    private void Awake()
    {
        //임시로 레벨 세팅

        StageSetting();
        UpgradePostionCountSetting(1000);
        SettingPlayerStatData_Traning();
        SettingPlayerMainData();
    }
    public void Start()
    {
        //HavedDeckSetting();
        //SavedDeckSetting();
        SyncStageLevelFromServer();
        SyncWeaponData();
        SyncAccessoryData();
        rubyPoint = 3000;
    }


    public void SettingPlayerStatData_Traning()
    {
        traningData = new Traning_PlayerStatData();//훈련 스탯 우선 0으로 전부 초기화 
        traningData.level = 1;//레벨은 1로 고정
    }
    public void SettingPlayerMainData()
    {
        mainData = new Main_PlayerStatData();//훈련 스탯 우선 0으로 전부 초기화 
        mainData.atk = 10;
    }


    public void StageSetting()
    {
        stageProgressLevel = 1;//저장된 스테이지 레벨
    }

    public void UpgradePostionCountSetting(int value)
    {
        //서버에서 강화 포션 보유량을 받아서 적용
        upgradePostionCount = value;
    }




    //  public void HavedDeckSetting(){
    //   //서버에서 현재 저장된 덱을 받아서 적용, 가진 모든 카드
    //     havedCardsList.Add(new Card(0,1,10));
    //     havedCardsList.Add(new Card(1,1,20));
    //     havedCardsList.Add(new Card(2, 1, 22));
    //     havedCardsList.Add(new Card(3,1,22));
    //     havedCardsList.Add(new Card(4, 1, 6));
    //     havedCardsList.Add(new Card(5, 1, 6));
    //     havedCardsList.Add(new Card(6, 1, 6));
    //     havedCardsList.Add(new Card(7, 1, 6));
    // }
    //  public void SyncDeckData()
    // {
    //     //데이터 메니저에서 받은 웨폰 데이터를 실제 웨폰 메니저에 적용
    //     HavedDeckSetting();
    // }









    /**
카드 머지 로직, 카드 도감에서 호출
    **/


    //    

    // 디버그 출력
    // Debug.Log("Merged BaseList:");
    //foreach (Card card in baseList)
    // {
    //   Debug.Log($"ID: {card.ID}, count: {card.COUNT}");
    //}

    //     return baseList;
    // }


    // /**
    // 카드 머지 로직 끝
    // **/



    //     public void SavedDeckSetting(){
    //     // 모든 덱 초기화
    //     for (int i = 0; i < savedDeck.Length; i++)
    //     {
    //         savedDeck[i] = Enumerable.Repeat(-1, 8).ToList(); // -1이 8개인 리스트 생성
    //     }

    //     // savedDeck[선택된 덱 프리셋]만 특정 값으로 임의 설정
    //     savedDeck[selectedPresetDeck] = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 };

    //     // 모든 savedDeck 디버깅
    //     //for (int i = 0; i < savedDeck.Length; i++)
    //    // {
    //        // string deckStatus = string.Join(", ", savedDeck[i]);
    //         //Debug.Log("Deck " + i + ": " + deckStatus);
    //    // }

    //     // 덱 정보 갱신
    //     GameManager.instance.deckManager.GetSavedDeck(savedDeck[selectedPresetDeck]);
    // }


    //    /// <summary>
    // /// savedDeck 리스트에서 -1 값을 제거하고 유효한 카드들을 앞으로 정렬한 후,
    // /// 뒤쪽에 -1을 채워 항상 8장을 유지하는 함수
    // /// </summary>
    // public void ReorderSavedDeck(int selected)
    // {
    //     // -1이 아닌 카드들만 새 리스트에 추가
    //     List<int> validCards = savedDeck[selected].Where(cardId => cardId != -1).ToList();

    //     // savedDeck[selected] 초기화
    //     savedDeck[selected].Clear();

    //     // 유효한 카드들 추가
    //     savedDeck[selected].AddRange(validCards);

    //     // 유효한 카드 수 출력
    //     //Debug.Log("유효한 카드 수: " + validCards.Count);

    //     // 부족한 부분 -1로 채우기
    //     while (savedDeck[selected].Count < 8)
    //     {
    //         savedDeck[selected].Add(-1);
    //     }

    //     // 최종적으로 저장된 덱 출력
    //    // Debug.Log("최종 덱 상태: " + string.Join(",", savedDeck[selected]));
    // }



    //      public int ReturnCardCount(int cardId){
    //       //카드 아이디를 입력하면 현재 해당 카드의 보유량을 리턴해주는 함수
    //       int returnNum = 0;//초기화
    //       for(int i = 0; i < havedCardsList.Count; i++){
    //         if(havedCardsList[i].ID == cardId){
    //           Card targetCard = GameManager.instance.dataManager.havedCardsList[i];
    //           returnNum = targetCard.COUNT;
    //         }
    //       }
    //       return returnNum;
    //      }

    // /**
    // public void ChageToRealValue(){
    //     //Upgrade 레벨을 받아서 실제 플레이어에게 적용되는 값으로 바꿔주는 함수.

    //     float real_ATK = level_ATK * 2;//1레벨당 증가량 2
    //     float real_HP = 100 + (level_Hp * 10);//1레벨당 증가량 10
    //     float real_HPRecovery = level_HpRecovery * 0.1f;//1레벨당 증가량 0.1%
    //     float real_CriticalDamage = level_CriticalDamage;//1레벨당 증가량 1%
    //     float real_criticalPer = level_CriticalPer * 0.1f;//1레벨당 증가량 0.1%

    //     //무기 장착 효과
    //     float equipWeaponEffectValue =  GameManager.instance.weaponManager.ReturnEquipEffect();

    //     //무기 보유 효과
    //     float ownedWeaponEffectValue = GameManager.instance.weaponManager.ReturnOwnedEffect();

    //     float finalWeaponEffectValue = equipWeaponEffectValue + ownedWeaponEffectValue;

    //     Player_Status player =  GameManager.instance.player.playerStatus;
    //     //ATK 참고 Real_ATK = 성장, 플레이어 레벨 ATK의 합
    //     player.ATK = real_ATK * (1 + (finalWeaponEffectValue / 100f));
    //     player.maxHealth = real_HP;
    //    // player.health += 10;//최대체력 증가량만큼 현재체력 회복
    //     player.healthRecoveryPer = real_HPRecovery;
    //     player.CriticalDamagePer = real_CriticalDamage;
    //     player.CriticalPer = real_criticalPer;

    // }


    public void SyncStageLevelFromServer()
    {
        // 데이터 매니저에서 받은 스테이지 레벨을 실제 스테이지 매니저에 적용
        GameManager.instance.stageManager.currentStageLevel = stageProgressLevel;
    }

    /**
장비-무기 관련 코드
    **/

    public void SyncWeaponData()
    {
        //데이터 메니저에서 받은 웨폰 데이터를 실제 웨폰 메니저에 적용
        weaponList = GetWeaponsData();
    }

    public List<Weapon> GetWeaponsData()
    {
        List<Weapon> baseWeapons = GetInitialWeaponsData();
        List<Weapon> overrideWeapons = GetOverrideWeaponsData();
        return MergeWeapons(baseWeapons, overrideWeapons);
    }

    private List<Weapon> GetInitialWeaponsData()
    {
        return new List<Weapon>
    {
        new Weapon(0, 1, false, 0, false),   // 기본값
        new Weapon(1, 1, false, 0, false),
        new Weapon(2, 1, false, 0, false),
        new Weapon(3, 1, false, 0, false),
        new Weapon(4, 1, false, 0, false),
        new Weapon(5, 1, false, 0, false),
        new Weapon(6, 1, false, 0, false),
        new Weapon(7, 1, false, 0, false),
        new Weapon(8, 1, false, 0, false),
        new Weapon(9, 1, false, 0, false),
        new Weapon(10, 1, false, 0, false),
        new Weapon(11, 1, false, 0, false),
        new Weapon(12, 1, false, 0, false),
        new Weapon(13, 1, false, 0, false),
        new Weapon(14, 1, false, 0, false),
        new Weapon(15, 1, false, 0, false),
        new Weapon(16, 1, false, 0, false),
        new Weapon(17, 1, false, 0, false),
        new Weapon(18, 1, false, 0, false),
        new Weapon(19, 1, false, 0, false),
        new Weapon(20, 1, false, 0, false),
        new Weapon(21, 1, false, 0, false),
        new Weapon(22, 1, false, 0, false),
        new Weapon(23, 1, false, 0, false),

    };
    }
    // public Weapon(int id, int level, WeaponGrade grade, int stack, bool equipped, int weaponCount, bool isAcquired)
    private List<Weapon> GetOverrideWeaponsData()
    {
        // 서버나 외부 데이터로부터 받은 갱신된 무기 정보
        return new List<Weapon>
    {
        new Weapon(0, 1, true, 50, true),   // Common 무기
        new Weapon(1, 1, false, 50, true),  // 기본값
        new Weapon(2, 1, false, 50, true),  // 기본값
        new Weapon(3, 1, false, 50, true),  // 기본값
        new Weapon(4, 1, false, 300, true),  // Rare 무기
        new Weapon(5, 1, false, 300, true),  // Rare 무기
        new Weapon(6, 1, false, 300, true),  // 기본값
        new Weapon(7, 1, false, 300, true),  // 기본값
        new Weapon(8, 1, false, 300, true),  // Epic 무기
        new Weapon(9, 1, false, 300, true),  // Epic 무기
        new Weapon(10, 1, false, 300, true), // 기본값
        new Weapon(11, 1, false, 300, true), // Epic 무기
        new Weapon(12, 1, false, 300, true), // 기본값
        new Weapon(13, 1, false, 300, true), // Legendary 무기
        /**
        new Weapon(14, 0, false, 300, true), // 기본값
        new Weapon(15, 0, false, 300, true), // Legendary 무기
        new Weapon(16, 0, false, 300, true), // Mythic 무기
        new Weapon(17, 0, false, 300, true), // Mythic 무기
        new Weapon(18, 0, false, 300, true), // Mythic 무기
        new Weapon(19, 0, false, 300, true), // 기본값
        new Weapon(20, 0, false, 300, true), // Primordial 무기
        new Weapon(21, 0, false, 0, false), // Primordial 무기
        new Weapon(22, 0, false, 0, false), // Primordial 무기
        new Weapon(23, 0, false, 0, false), // Primordial 무기
        **/
    };
    }

    private List<Weapon> MergeWeapons(List<Weapon> baseList, List<Weapon> overrideList)
    {
        foreach (Weapon newWeapon in overrideList)
        {
            int index = baseList.FindIndex(w => w.weaponId == newWeapon.weaponId);
            if (index >= 0)
            {
                baseList[index] = newWeapon;
            }
            else
            {
                baseList.Add(newWeapon);
            }
        }
        return baseList;
    }

    /**
장비-무기 관련 끝
    **/

    /**
장비-악세 관련 시작
    **/
    public void SyncAccessoryData()
    {
        //데이터 메니저에서 받은 웨폰 데이터를 실제 웨폰 메니저에 적용
        acceossryList = GetAccessoriesData();

        // 디버그 로그 출력
        Debug.Log("=== 악세서리 데이터 동기화 완료 ===");
        foreach (var acc in acceossryList)
        {
            Debug.Log($"ID: {acc.id}, 레벨: {acc.level}, 장착 여부: {acc.isEquipped}, 보유 여부: {acc.isAcquired}");
        }
    }

    public List<Accessory> GetAccessoriesData()
    {
        List<Accessory> baseAcc = GetInitialAccessoriesData();
        List<Accessory> overrideAcc = GetOverrideAccessoriesData();
        return MergeAccessories(baseAcc, overrideAcc);
    }

    private List<Accessory> GetInitialAccessoriesData()
    {
        return new List<Accessory>
    {
        new Accessory(0, 1, false, 0, false), // 기본값
        new Accessory(1, 1, false, 0, false), // 기본값
        new Accessory(2, 1, false, 0, false), // 기본값
        new Accessory(3, 1, false, 0, false), // 기본값
        new Accessory(4, 1, false, 0, false), // 기본값
        new Accessory(5, 1, false, 0, false), // 기본값
        new Accessory(6, 1, false, 0, false), // 기본값
        new Accessory(7, 1, false, 0, false), // 기본값
        new Accessory(8, 1, false, 0, false), // 기본값
        new Accessory(9, 1, false, 0, false), // 기본값
        new Accessory(10, 1, false, 0, false), // 기본값
        new Accessory(11, 1, false, 0, false), // 기본값
        new Accessory(12, 1, false, 0, false), // 기본값
        new Accessory(13, 1, false, 0, false), // 기본값
        new Accessory(14, 1, false, 0, false), // 기본값
        new Accessory(15, 1, false, 0, false), // 기본값
        new Accessory(16, 1, false, 0, false), // 기본값
        new Accessory(17, 1, false, 0, false), // 기본값
        new Accessory(18, 1, false, 0, false), // 기본값
        new Accessory(19, 1, false, 0, false), // 기본값
        new Accessory(20, 1, false, 0, false), // 기본값
        new Accessory(21, 1, false, 0, false), // 기본값
        new Accessory(22, 1, false, 0, false), // 기본값
        new Accessory(23, 1, false, 0, false), // 기본값
    };

    }
    private List<Accessory> GetOverrideAccessoriesData()
    {
        // 서버나 외부 데이터로부터 받은 갱신된 무기 정보
        return new List<Accessory>
    {
         new Accessory(0, 1, true, 100, true), // 기본값
        new Accessory(1, 1, false, 0, false), // 기본값
        new Accessory(2, 1, false, 0, false), // 기본값
        new Accessory(3, 1, false, 0, false), // 기본값
        new Accessory(4, 1, false, 0, true), // 기본값
        new Accessory(5, 1, false, 0, false), // 기본값
        new Accessory(6, 1, false, 0, false), // 기본값
        new Accessory(7, 1, false, 100, true), // 기본값
        new Accessory(8, 1, false, 0, false), // 기본값
        new Accessory(9, 1, false, 0, false), // 기본값
        new Accessory(10, 1, false, 0, true), // 기본값
        new Accessory(11, 1, false, 0, false), // 기본값
        new Accessory(12, 1, false, 0, false), // 기본값
        new Accessory(13, 1, false, 100, true), // 기본값
        new Accessory(14, 1, false, 0, false), // 기본값
        new Accessory(15, 1, false, 0, false), // 기본값
        new Accessory(16, 1, false, 0, false), // 기본값
        new Accessory(17, 1, false, 0, false), // 기본값
        new Accessory(18, 1, false, 0, false), // 기본값
        new Accessory(19, 1, false, 0, false), // 기본값
        new Accessory(20, 1, false, 0, false), // 기본값
        new Accessory(21, 1, false, 0, false), // 기본값
        new Accessory(22, 1, false, 0, false), // 기본값
        new Accessory(23, 1, false, 0, false), // 기본값
    };
    }

    private List<Accessory> MergeAccessories(List<Accessory> baseList, List<Accessory> overrideList)
    {
        foreach (Accessory newAcc in overrideList)
        {
            int index = baseList.FindIndex(w => w.id == newAcc.id);
            if (index >= 0)
            {
                baseList[index] = newAcc;
            }
            else
            {
                baseList.Add(newAcc);
            }
        }
        return baseList;
    }

    /**
장비 - 악세 관련 코드 끝
    **/


    //     public void LogDeckStatus()
    // {
    //     Debug.Log("=== Haved Deck ===");
    //     foreach (var card in havedCardsList)
    //     {
    //         Debug.Log($"ID: {card.ID}, COUNT: {card.COUNT}, Stack: {card.STACK}");
    //     }

    //     Debug.Log("=== Saved Deck ===");
    //     foreach (var num in savedDeck)
    //     {
    //         Debug.Log($"saveDeckList: " + num);
    //     }
    // }

    // }


    //Waepon Info
    public enum WeaponGrade { Common, Rare, Epic, Legendary, Mythic, Primordial }
    public class Weapon
    {
        public int weaponId;      // 고유 번호
        public int level;  // 레벨(강화) 수치
        public bool isEquipped;   // 장착 여부
        public int weaponCount;    // 보유량 수치
        public bool isAcquired; //획득 여부

        // 🔹 생성자 추가 (5개의 인수를 받도록 설정)
        public Weapon(int id, int level, bool equipped, int count, bool isAcquired)
        {
            this.weaponId = id;
            this.level = level;
            this.isEquipped = equipped;
            this.weaponCount = count;
            this.isAcquired = isAcquired;
        }
    }
    public class Accessory
    {
        public int id;      // 고유 번호
        public int level;    // 레벨 수치
        public int count_Owned;    // 보유량 수치
        public bool isAcquired; //획득 여부
        public bool isEquipped;   // 장착 여부

        // 🔹 생성자 추가 (5개의 인수를 받도록 설정)
        public Accessory(int id, int level, bool equipped, int ownedCount, bool isAcquired)
        {
            this.id = id;
            this.level = level;
            this.isEquipped = equipped;
            this.count_Owned = ownedCount;
            this.isAcquired = isAcquired;
        }
    }

    [System.Serializable]
    public class Traning_PlayerStatData
    {
        public int level;
        public int atk;
        public int hp;
        public int vit;
        public int cri;
        public int luk;
        public int mrp;
        public int dcd;
        public int point;
        public int expPoint;
    }

    [System.Serializable]
    public class Main_PlayerStatData
    {
        public int atk;//공격력 레벨
        public int hp;//최대 체력 레벨
        public int vit;//최력 회복량 레벨
        public int criticalDamage;//치명타 배율 레벨
        public int criticalPer;//치명타 확률 레벨
    }
}