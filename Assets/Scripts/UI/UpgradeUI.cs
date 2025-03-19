using UnityEngine;
using TMPro;

public class UpgradeUI : MonoBehaviour
{

    public enum UpgradeType{ATK, MaxHp, HpRecovery, CriticalPer , CriticalDamage, PlayerLevel};
    UpgradeType upgradeType;
  [Header("Text_MainName")]
  //다국어 지원용
  public TMP_Text ATK_Text;
  public TMP_Text MaxHp_Text;
  public TMP_Text HpRecovery_Text;
  public TMP_Text CriticalDamage_Text;
  public TMP_Text CriticalPer_Text;

  [Header("Level")]
    public TMP_Text ATK_Text_level;
     public TMP_Text MaxHp_Text_level;
  public TMP_Text HpRecovery_Text_level;
  public TMP_Text CriticalDamage_Text_level;
  public TMP_Text CriticalPer_Text_level;
  [Header("Desc")]
   public TMP_Text ATK_Text_Desc;
     public TMP_Text MaxHp_Text_Desc;
  public TMP_Text HpRecovery_Text_Desc;
  public TMP_Text CriticalDamage_Text_Desc;
  public TMP_Text CriticalPer_Text_Desc;

  [Header("GoldValue")]
   public TMP_Text ATK_Text_Gold;
  public TMP_Text MaxHp_Text_Gold;
  public TMP_Text HpRecovery_Text_Gold;
  public TMP_Text CriticalDamage_Text_Gold;
  public TMP_Text CriticalPer_Text_Gold;
  //

  public EffectPooling effectPooling;
  public RectTransform[] effectIcon; // 🔹 화면 왼쪽 끝에 있는 아이콘 (Inspector에서 설정)

    void Awake()
    {
         UpgradeEffectAnim(0);//이펙트 버그 수정용(미리 하나 만들어야 처음부터 이펙트 연출됨)
    }


    public void ATK_Setting(){
    DataManager data = GameManager.instance.dataManager;
    int nowLevel = data.level_ATK;
    float desc_Now = nowLevel * 2;
    float desc_After = (nowLevel + 1) * 2;

    //text setting
    ATK_Text_level.text = "Lv." + nowLevel;
    ATK_Text_Desc.text = desc_Now + "->" + desc_After;
    ATK_Text_Gold.text = GetGoldForLevel(UpgradeType.ATK).ToString();
  }

   public void MaxHp_Setting(){
    DataManager data = GameManager.instance.dataManager;
    int nowLevel = data.level_Hp;
    float desc_Now = 100 + (nowLevel * 10);
    float desc_After = 100 + ((nowLevel + 1) * 10);

    //text setting
    MaxHp_Text_level.text = "Lv." + nowLevel;
    MaxHp_Text_Desc.text = desc_Now + "->" + desc_After;
    MaxHp_Text_Gold.text = GetGoldForLevel(UpgradeType.MaxHp).ToString();
  }

  public void HpRecovery_Setting(){
    DataManager data = GameManager.instance.dataManager;
    int nowLevel = data.level_HpRecovery;
    float desc_Now = nowLevel * 0.1f;
    float desc_After = (nowLevel + 1) * 0.1f;

    //text setting
    HpRecovery_Text_level.text = "Lv." + nowLevel;
    HpRecovery_Text_Desc.text = desc_Now + "%" + "->" + desc_After + "%/sec";
    HpRecovery_Text_Gold.text = GetGoldForLevel(UpgradeType.HpRecovery).ToString();
  }
  public void CriticalDamage_Setting(){
    DataManager data = GameManager.instance.dataManager;
    int nowLevel = data.level_CriticalDamage;
    float desc_Now = nowLevel;
    float desc_After = nowLevel+ 1;

    //text setting
    CriticalDamage_Text_level.text = "Lv." + nowLevel;
    CriticalDamage_Text_Desc.text = desc_Now + "%" + "->" + desc_After + "%";
    CriticalDamage_Text_Gold.text = GetGoldForLevel(UpgradeType.CriticalDamage).ToString();
  }
 public void CriticalPer_Setting(){
    DataManager data = GameManager.instance.dataManager;
    int nowLevel = data.level_CriticalPer;
    float desc_Now = nowLevel * 0.1f;
    float desc_After = (nowLevel+ 1) * 0.1f;

    //text setting
    CriticalPer_Text_level.text = "Lv." + nowLevel;
    CriticalPer_Text_Desc.text = desc_Now + "%" + "->" + desc_After + "%";
    CriticalPer_Text_Gold.text = GetGoldForLevel(UpgradeType.CriticalPer).ToString();
  }

  public void AllUpgradeSetting(){
    //모든 능력치 세팅 한번에 모아둔 함수
    ATK_Setting();
    MaxHp_Setting();
    HpRecovery_Setting();
    CriticalDamage_Setting();
    CriticalPer_Setting();
  }
    public static int GetGoldForLevel(UpgradeType type)
    {
      //업그레이드 항목마다 등차 설정
        DataManager data = GameManager.instance.dataManager;
        int level;//업그레이드 레벨
        int d;//등차

        switch(type){
            case UpgradeType.ATK:
            level = data.level_ATK;
            d = 2;
            break;
            case UpgradeType.MaxHp:
            level = data.level_Hp;
            d = 2;
            break;
            case UpgradeType.HpRecovery:
            level = data.level_HpRecovery;
            d = 2;
            break;
            case UpgradeType.CriticalDamage:
            level = data.level_CriticalDamage;
            d = 2;
            break;
            case UpgradeType.CriticalPer:
            level = data.level_CriticalPer;
            d = 2;
            break;
            default:
            level = 0;
            d = 2;
            break;
        }

        int firstTerm = 10; // 첫 번째 레벨의 필요 골드
        int commonDifference = 1; // 초기 등차
        int requiredGold = firstTerm; // 첫 번째 레벨 필요 골드

        for (int i = 1; i < level; i++)
        {
            requiredGold += commonDifference;
            commonDifference += d; // 등차 2씩 증가
        }

        return requiredGold;
    }



  public void UpgradeButton(int Uptype)
{
    upgradeType = (UpgradeType)Uptype;
    DataManager data = GameManager.instance.dataManager;
    int requiredGold = GetGoldForLevel(upgradeType); // 골드 요구량
    int effectPos = 0;

    if (data.goldPoint < requiredGold) return;
    data.goldPoint -= requiredGold;

    switch (upgradeType)
    {
        case UpgradeType.ATK:
            GameManager.instance.dataManager.level_ATK += 1;
            effectPos = 0;
        break;

        case UpgradeType.MaxHp:
            GameManager.instance.dataManager.level_Hp += 1;
            GameManager.instance.player.playerStatus.health += 10; // 최대 체력 증가량만큼 현재 체력 회복
             effectPos = 1;
        break;

        case UpgradeType.HpRecovery:
            GameManager.instance.dataManager.level_HpRecovery += 1;
             effectPos = 2;
        break;

        case UpgradeType.CriticalPer:
            GameManager.instance.dataManager.level_CriticalPer += 1;
             effectPos = 3;
        break;

        case UpgradeType.CriticalDamage:
            GameManager.instance.dataManager.level_CriticalDamage += 1;
             effectPos = 4;
        break;

        default:
        break;
    }

    UpgradeEffectAnim(effectPos);

    AllUpgradeSetting();
    data.ChageToRealValue(); // 캐릭터 stats에 실제로 변경된 값 적용
}

    public void OffUpgradeUI(){
      GameManager.instance.boardUI.buttomTapUI.gameObject.SetActive(false);
      gameObject.SetActive(false);
    }

    public void UpgradeEffectAnim(int index){
        // 🔹 이펙트 생성
            RectTransform effect = effectPooling.Get(0).GetComponent<RectTransform>();

            // 1️⃣ 아이콘의 월드 좌표 가져오기
            Vector3 worldPosition = effectIcon[index].position; 

            // 2️⃣ 이펙트도 월드 좌표로 변경
            effect.position = worldPosition;
    }
}
