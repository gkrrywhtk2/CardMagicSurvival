// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using System.Collections;

// public class UpgradeUI : MonoBehaviour
// {
//   public DataManager mainData;

//     public enum UpgradeType{ATK, MaxHp, HpRecovery, CriticalPer , CriticalDamage, PlayerLevel};
//     UpgradeType upgradeType;
//   [Header("Text_MainName")]
//   //다국어 지원용
//   public TMP_Text ATK_Text;
//   public TMP_Text MaxHp_Text;
//   public TMP_Text HpRecovery_Text;
//   public TMP_Text CriticalDamage_Text;
//   public TMP_Text CriticalPer_Text;

//   [Header("Level")]
//     public TMP_Text ATK_Text_level;
//      public TMP_Text MaxHp_Text_level;
//   public TMP_Text HpRecovery_Text_level;
//   public TMP_Text CriticalDamage_Text_level;
//   public TMP_Text CriticalPer_Text_level;
//   [Header("Desc")]
//    public TMP_Text ATK_Text_Desc;
//      public TMP_Text MaxHp_Text_Desc;
//   public TMP_Text HpRecovery_Text_Desc;
//   public TMP_Text CriticalDamage_Text_Desc;
//   public TMP_Text CriticalPer_Text_Desc;

//   [Header("GoldValue")]
//    public TMP_Text ATK_Text_Gold;
//   public TMP_Text MaxHp_Text_Gold;
//   public TMP_Text HpRecovery_Text_Gold;
//   public TMP_Text CriticalDamage_Text_Gold;
//   public TMP_Text CriticalPer_Text_Gold;
//   //

//   public EffectPooling effectPooling;
//   public RectTransform[] effectIcon; // 🔹 화면 왼쪽 끝에 있는 아이콘 (Inspector에서 설정)
//    public RectTransform[] traning_EffectIcon; // 🔹 화면 왼쪽 끝에 있는 아이콘 (Inspector에서 설정)


//   //훈련 스탯 관련
//     public enum TraningType{ATK, HP, VIT, CRI , LUK, MRP, DCD};
//         TraningType traningType;
//     [Header("TraningStat")]
    
//     public TMP_Text traning_ATK_Level;//훈련 ATK 레벨
//     public TMP_Text traning_ATK_Desc;//
//     public TMP_Text traning_HP_Level;//
//     public TMP_Text traning_HP_Desc;//훈련 Desc
//     public TMP_Text traning_VIT_Level;//
//     public TMP_Text traning_VIT_Desc;//훈련 Desc
//     public TMP_Text traning_CRI_Level;//
//     public TMP_Text traning_CRI_Desc;//훈련 Desc
//     public TMP_Text traning_LUK_Level;//
//     public TMP_Text traning_LUK_Desc;//훈련 Desc
//     public TMP_Text traning_MRP_Level;//
//     public TMP_Text traning_MRP_Desc;//훈련 Desc
//     public TMP_Text traning_DCD_Level;//
//     public TMP_Text traning_DCD_Desc;//훈련 Desc


//   // 플레이어 레벨 관련
    
//      public TMP_Text text_PlayerLevel;
//     public TMP_Text text_expUnderFill;//게이지 위에 있는 경험치 텍스트
//     public TMP_Text text_expPer;//경험치 몇 퍼?
//     public TMP_Text text_statPoint;//잔여 스탯 포인트 TEXT
//       public Slider expBar;
//       public Image playerLevelUpButton;
//       RedDotController redDot;
//       public Animator flashAnim;//레벨업 애니메이션 효과
//       private void Awake() {
//         redDot = GetComponentInChildren<RedDotController>();
//          UpgradeEffectAnim(0);//이펙트 버그 수정용(미리 하나 만들어야 처음부터 이펙트 연출됨)
//       }


//     void FixedUpdate()
//     {
//         //EXPUpdate(); 코루틴으로 변경
//     }

//   public void EXPUpdate(){
//     int nowPlayerLevel = mainData.traningData.level;
//     int maxEXP = nowPlayerLevel * 1000; // 임시, 필요 경험치 함수
//     int nowEXP = mainData.traningData.expPoint;

//     expBar.value = (float)nowEXP / maxEXP; // 정수 나눗셈 방지 (float 변환)
//     text_PlayerLevel.text = "LV. " + nowPlayerLevel.ToString();
//     text_expUnderFill.text = nowEXP.ToString() + " / " + maxEXP.ToString();
    
//     // 🔹 백분율로 변환 & 소수점 1자리까지 표시
//     float percentage = (float)nowEXP / maxEXP * 100;
//     text_expPer.text = Mathf.Min(percentage, 100f).ToString("F1") + "%";

//     // 잔여 스탯 포인트 표기
//     text_statPoint.text = "POINT : " + mainData.traningData.point.ToString();

//     // 레벨업이 가능하면 화이트로, 불가능하면 회색(A2A2A2)로 설정
//     if (nowEXP > maxEXP) // 레벨업이 가능하면
//     {
//         playerLevelUpButton.color = Color.white; // 레벨업 가능 상태
//         redDot.UpdateRedDot(true);
//     }
//     else // 레벨업이 불가능하면
//     {
//         playerLevelUpButton.color = new Color(0xA2 / 255f, 0xA2 / 255f, 0xA2 / 255f); // A2A2A2 색상
//         redDot.UpdateRedDot(false);
//     }
// }

//  private Coroutine expUpdateCoroutine;
//     public void ShowLevelUpAnimation(){
//       flashAnim.SetTrigger("Flash");
//     }

//     private void OnEnable()
//     {
//         // 이미 코루틴이 실행 중이라면 중복 실행을 방지
//         if (expUpdateCoroutine == null)
//         {
//             expUpdateCoroutine = StartCoroutine(ExpUpdateCoroutine());
//         }
//     }

//     private void OnDisable()
//     {
//         // GameObject가 비활성화될 때 코루틴을 중지
//         if (expUpdateCoroutine != null)
//         {
//             StopCoroutine(expUpdateCoroutine);
//             expUpdateCoroutine = null;
//         }
//     }

//     private IEnumerator ExpUpdateCoroutine()
//     {
//         while (true)
//         {
//             EXPUpdate();
//             Debug.Log("Updating EXP...");//0.5초 마다 초기화
//             yield return new WaitForSeconds(0.5f);
//         }
//     }

  

//     public void ATK_Setting(){
//     DataManager data = GameManager.instance.dataManager;
//     int nowLevel = data.mainData.atk;
//     float desc_Now = nowLevel * 2;
//     float desc_After = (nowLevel + 1) * 2;

//     //text setting
//     ATK_Text_level.text = "Lv." + nowLevel;
//     ATK_Text_Desc.text = desc_Now + "->" + desc_After;
//     ATK_Text_Gold.text = GetGoldForLevel(UpgradeType.ATK).ToString();
//   }

//    public float MaxHp_Setting(){
//     DataManager data = GameManager.instance.dataManager;
//     int nowLevel = data.mainData.hp;
//     float desc_Now = 100 + (nowLevel * 10);
//     float desc_After = 100 + ((nowLevel + 1) * 10);

//     //text setting
//     MaxHp_Text_level.text = "Lv." + nowLevel;
//     MaxHp_Text_Desc.text = desc_Now + "->" + desc_After;
//     MaxHp_Text_Gold.text = GetGoldForLevel(UpgradeType.MaxHp).ToString();
//     return desc_Now;
//   }

//   public float HpRecovery_Setting(){
//     DataManager data = GameManager.instance.dataManager;
//     int nowLevel = data.mainData.vit;
//     float desc_Now = nowLevel * 2;
//     float desc_After = (nowLevel + 1) * 2;

//     //text setting
//     HpRecovery_Text_level.text = "Lv." + nowLevel;
//     HpRecovery_Text_Desc.text = "+" + desc_Now + "-> " + "+" + desc_After;
//     HpRecovery_Text_Gold.text = GetGoldForLevel(UpgradeType.HpRecovery).ToString();
//     return desc_Now;
//   }
//   public float CriticalDamage_Setting(){
//     DataManager data = GameManager.instance.dataManager;
//     int nowLevel = data.mainData.criticalDamage;
//     float desc_Now = nowLevel;
//     float desc_After = nowLevel+ 1;

//     //text setting
//     CriticalDamage_Text_level.text = "Lv." + nowLevel;
//     CriticalDamage_Text_Desc.text = desc_Now + "%" + "->" + desc_After + "%";
//     CriticalDamage_Text_Gold.text = GetGoldForLevel(UpgradeType.CriticalDamage).ToString();
//     return desc_Now;
//   }
//  public float CriticalPer_Setting(){
//     DataManager data = GameManager.instance.dataManager;
//     int nowLevel = data.mainData.criticalPer;
//     float desc_Now = nowLevel * 0.1f;
//     float desc_After = (nowLevel+ 1) * 0.1f;

//     //text setting
//     CriticalPer_Text_level.text = "Lv." + nowLevel;
//     CriticalPer_Text_Desc.text = desc_Now + "%" + "->" + desc_After + "%";
//     CriticalPer_Text_Gold.text = GetGoldForLevel(UpgradeType.CriticalPer).ToString();
//     return desc_Now;
//   }

//   //훈련 스탯 세팅 함수
//     public void Traning_ATK_Setting(){
//     int nowLevel = mainData.traningData.atk;
//     float desc_Now = nowLevel * 5;
//     float desc_After = (nowLevel + 1) * 5;

//     //text setting
//     traning_ATK_Level.text = "Lv." + nowLevel;
//     traning_ATK_Desc.text = "공격력 " + "+" + desc_Now + "-> " + "+" + desc_After;
//   }

//   public float Traning_HP_Setting(){
//     int nowLevel = mainData.traningData.hp;
//     float desc_Now = nowLevel * 30;
//     float desc_After = (nowLevel + 1) * 30;

//     //text setting
//     traning_HP_Level.text = "Lv." + nowLevel;
//     traning_HP_Desc.text = "체력 " + "+" + desc_Now + "-> " + "+" + desc_After;
//     return desc_Now;
//   }
//   public float Traning_VIT_Setting(){
//     int nowLevel = mainData.traningData.vit;
//     float desc_Now = nowLevel * 5;
//     float desc_After = (nowLevel + 1) * 5;

//     //text setting
//     traning_VIT_Level.text = "Lv." + nowLevel;
//     traning_VIT_Desc.text = "초당 체력 회복량 " + "+" + desc_Now + "-> " + "+" + desc_After;
//     return desc_Now;
//   }
//   public float Traning_CRI_Setting(){
//     int nowLevel = mainData.traningData.cri;
//     float desc_Now = nowLevel * 3;
//     float desc_After = (nowLevel + 1) * 3;

//     //text setting
//     traning_CRI_Level.text = "Lv." + nowLevel;
//     traning_CRI_Desc.text = "치명타 공격력 " + "+" + desc_Now + "%" + " -> " + "+" + desc_After + "%";
//     return desc_Now;
//   }
//    public float Traning_LUK_Setting(){
//     int nowLevel = mainData.traningData.luk;
//     float desc_Now = nowLevel * 0.5f;
//     float desc_After = (nowLevel + 1) * 0.5f;

//     //text setting
//     traning_LUK_Level.text = "Lv." + nowLevel;
//     traning_LUK_Desc.text = "골드 추가 획득량 " + "+" + desc_Now + "%" + " -> " + "+" + desc_After + "%";
//     return desc_Now;
//   }
//   public float Traning_MRP_Setting()
//   {
//       int nowLevel = mainData.traningData.mrp;
//       float desc_Now = nowLevel * 0.001f;
//       float desc_After = (nowLevel + 1) * 0.001f;

//       // text setting (소수점 3자리 고정)
//       traning_MRP_Level.text = "Lv." + nowLevel;
//       traning_MRP_Desc.text = $"마나 추가 회복량 +{desc_Now:F3} -> +{desc_After:F3}";

//       return desc_Now;
//   }

//   public float Traning_DCD_Setting()
//   {
//       int nowLevel = mainData.traningData.dcd;
//       float desc_Now = nowLevel * 0.001f;
//       float desc_After = (nowLevel + 1) * 0.001f;

//       // text setting (소수점 3자리 고정)
//       traning_DCD_Level.text = "Lv." + nowLevel;
//       traning_DCD_Desc.text = $"카드 뽑기 대기시간 -{desc_Now:F3}s -> -{desc_After:F3}s";

//       return desc_Now;
//   }

//   public void AllUpgradeSetting(){
//     //모든 능력치 세팅 한번에 모아둔 함수
//     ATK_Setting();
//     MaxHp_Setting();
//     HpRecovery_Setting();
//     CriticalDamage_Setting();
//     CriticalPer_Setting();
//     //훈련 스탯
//     Traning_ATK_Setting();
//     Traning_HP_Setting();
//     Traning_VIT_Setting();
//     Traning_CRI_Setting();
//     Traning_LUK_Setting();
//     Traning_MRP_Setting();
//     Traning_DCD_Setting();
//   }
//     public static int GetGoldForLevel(UpgradeType type)
//     {
//       //업그레이드 항목마다 등차 설정
//         DataManager data = GameManager.instance.dataManager;
//         int level;//업그레이드 레벨
//         int d;//등차

//         switch(type){
//             case UpgradeType.ATK:
//             level = data.mainData.atk;
//             d = 2;
//             break;
//             case UpgradeType.MaxHp:
//             level = data.mainData.hp;
//             d = 2;
//             break;
//             case UpgradeType.HpRecovery:
//             level = data.mainData.vit;
//             d = 2;
//             break;
//             case UpgradeType.CriticalDamage:
//             level = data.mainData.criticalDamage;
//             d = 2;
//             break;
//             case UpgradeType.CriticalPer:
//             level = data.mainData.criticalPer;
//             d = 2;
//             break;
//             default:
//             level = 0;
//             d = 2;
//             break;
//         }

//         int firstTerm = 10; // 첫 번째 레벨의 필요 골드
//         int commonDifference = 1; // 초기 등차
//         int requiredGold = firstTerm; // 첫 번째 레벨 필요 골드

//         for (int i = 1; i < level; i++)
//         {
//             requiredGold += commonDifference;
//             commonDifference += d; // 등차 2씩 증가
//         }

//         return requiredGold;
//     }



//   public void UpgradeButton(int Uptype)
// {
//     upgradeType = (UpgradeType)Uptype;
//     DataManager data = GameManager.instance.dataManager;
//     int requiredGold = GetGoldForLevel(upgradeType); // 골드 요구량
//     int effectPos = 0;

//     if (data.goldPoint < requiredGold){
//       GameManager.instance.WarningText("골드가 부족합니다");
//       return;
//     } 
//     data.goldPoint -= requiredGold;

//     switch (upgradeType)
//     {
//         case UpgradeType.ATK:
//             GameManager.instance.dataManager.mainData.atk += 1;
//             effectPos = 0;
//         break;

//         case UpgradeType.MaxHp:
//             GameManager.instance.dataManager.mainData.hp += 1;
//             GameManager.instance.player.playerStatus.health += 10; // 최대 체력 증가량만큼 현재 체력 회복
//              effectPos = 1;
//         break;

//         case UpgradeType.HpRecovery:
//             GameManager.instance.dataManager.mainData.vit += 1;
//              effectPos = 2;
//         break;

//         case UpgradeType.CriticalPer:
//             GameManager.instance.dataManager.mainData.criticalPer += 1;
//              effectPos = 3;
//         break;

//         case UpgradeType.CriticalDamage:
//             GameManager.instance.dataManager.mainData.criticalDamage += 1;
//              effectPos = 4;
//         break;

//         default:
//         break;
//     }

//     UpgradeEffectAnim(effectPos);

//     AllUpgradeSetting();
//     GameManager.instance.player.playerStatus.InitALLStat();
//     //data.ChageToRealValue(); // 캐릭터 stats에 실제로 변경된 값 적용
// }

//     public void OffUpgradeUI(){
//       GameManager.instance.boardUI.buttomTapUI.gameObject.SetActive(false);
//       gameObject.SetActive(false);
//     }

    

//     public void UpgradeEffectAnim(int index){
//         // 🔹 이펙트 생성
//             RectTransform effect = effectPooling.Get(0).GetComponent<RectTransform>();

//             // 1️⃣ 아이콘의 월드 좌표 가져오기
//             Vector3 worldPosition = effectIcon[index].position; 

//             // 2️⃣ 이펙트도 월드 좌표로 변경
//             effect.position = worldPosition;
//     }

//      public void Training_UpgradeEffectAnim(int index){
//         // 🔹 이펙트 생성
//             RectTransform effect = effectPooling.Get(1).GetComponent<RectTransform>();

//             // 1️⃣ 아이콘의 월드 좌표 가져오기
//             Vector3 worldPosition = traning_EffectIcon[index].position; 

//             // 2️⃣ 이펙트도 월드 좌표로 변경
//             effect.position = worldPosition;
//     }

//       public void Traning_UpgradeButton(int Type)
//   {
//       traningType = (TraningType)Type;//컴포넌트에서 설정한 int값을 TraningType으로 변환
//       int effectPos = 0;
//       int traningStatPoint = mainData.traningData.point;

//       if (traningStatPoint < 1){//필요 포인트량 1
//         //Debug.Log("잔여 포인트가 부족합니다!");
//         GameManager.instance.WarningText("잔여포인트가 부족합니다");
//         return;
//       }
//         switch (traningType)
//         {
//             case TraningType.ATK:
//                 mainData.traningData.atk++;
//                 effectPos = 0;
//             break;

//             case TraningType.HP: 
//                 mainData.traningData.hp++;
//                 effectPos = 1;
//             break;

//             case TraningType.VIT:
//                 mainData.traningData.vit++;
//                 effectPos = 2;
//             break;

//             case TraningType.CRI:
//               mainData.traningData.cri++;
//                 effectPos = 3;
//             break;

//             case TraningType.LUK:
//                 mainData.traningData.luk++;
//                 effectPos = 4;
//             break;

//             case TraningType.MRP:
//               mainData.traningData.mrp++;
//                 effectPos = 5;
//             break;

//             case TraningType.DCD:
//                 mainData.traningData.dcd++;
//                 effectPos = 6;
//             break;

//             default:
//             break;
//         }
//       mainData.traningData.point--; //포인트 감소
//       Training_UpgradeEffectAnim(effectPos);
//       AllUpgradeSetting();
//       GameManager.instance.player.playerStatus.InitALLStat();
//   }

//   public void ButtonResetPoint(){
//     //스탯 포인트 초기화 버튼
//     GameManager.instance.purchaseUI.gameObject.SetActive(true);
//     GameManager.instance.purchaseUI.Init(PurchaseUI.PurchaseList.ResetStatPoint);
//   }
//   public void ResetTraningPoint(){
//     mainData.traningData.atk = 0;
//     mainData.traningData.hp = 0;
//     mainData.traningData.cri = 0;
//     mainData.traningData.luk = 0;
//     mainData.traningData.mrp = 0;
//     mainData.traningData.dcd = 0;
//     mainData.traningData.point = mainData.traningData.level - 1;
//     AllUpgradeSetting();//UI 세팅
//   }
// }
