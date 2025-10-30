// using UnityEngine;
// using System.Collections.Generic;
// using TMPro;
// using UnityEngine.UI;

// public class AccessoryManager : MonoBehaviour
// {
//     [Header("# DATA")]
//     public AccessoryData[] accessoryData;//악세 데이타 모음
//      public Icon_Acc[] icon_Acc;//장비 아이콘 UI
//     public Icon_Acc info_Acc;//상세 보기 아이콘
//     public EffectPooling effectPooling;

   


//     [Header("# Main UI")]
//     public GameObject mainUI; //UI 부모 오브젝트
//     public TMP_Text text_Name;
//     public TMP_Text text_Rank;
//     public TMP_Text text_EquipEffectName; //장착 효과 이름
//     public TMP_Text text_EquipEffectDesc; //변하는 장착 효과
//     public Image EquipButton; //장착 버튼
//     public Image levelUpButton; //장착 버튼
//     public TMP_Text text_Equip; //장착 or 장착중

//     [Header("# Owned Effect UI")]
//     public TMP_Text[] text_OwnedEffectName; //변하는 보유 효과 이름
//     public TMP_Text[] text_OwnedEffectDesc; //변하는 보유 효과 설명

//     [Header("# Level UI")]
//     public Image levelButton_AccessorySrptie; //버튼에 있는 이미지 변경
//     public TMP_Text text_RequireLevelUpCount;//레벨업에 필요한 아이템의 수

//     [Header("# ETC")]
//     public int saveNowId; //현재 켜져있는 아이템 UI ID
//     [Header("# COLOR_PRESET")]
//     public Color commonColor_W, commonColor;
//     public Color rareColor_W, rareColor;
//     public Color epicColor_W, epicColor;
//     public Color legendColor_W, legendColor;
//     public Color mythicColor_W, mythicColor;         
//     public Color primordialColor_W, primordialColor;
//     public Color blackColor;
//     public Color whiteColor;
//     public Color alphaColor;
//       public Color grayColor;
//         private void Awake()
//     {
//         // 밝은 색상 (획득한 무기)
//         commonColor_W = new Color(0.7f, 0.7f, 0.7f, 1f);
//         rareColor_W = new Color(0.3f, 0.5f, 0.9f, 1f);
//         epicColor_W = new Color(0.6f, 0.3f, 0.8f, 1f);
//         legendColor_W = new Color(1f, 0.7f, 0.2f, 1f);
//         mythicColor_W = new Color(0.639f, 0.086f, 0.129f, 1f);       // 신화: 붉은색 계열
//         primordialColor_W = new Color(0f, 0.721f, 0.580f, 1f);       // 태초: 신비로운 초록

//         // 어두운 색상 (미획득 무기)
//         commonColor = new Color(0.4f, 0.4f, 0.4f, 1f);
//         rareColor = new Color(0.2f, 0.3f, 0.6f, 1f);
//         epicColor = new Color(0.4f, 0.2f, 0.5f, 1f);
//         legendColor = new Color(0.8f, 0.5f, 0.1f, 1f);
//         mythicColor = new Color(0.4f, 0.07f, 0.1f, 1f);               // 어두운 신화: 진한 붉은색
//         primordialColor = new Color(0f, 0.4f, 0.3f, 1f);             // 어두운 태초: 어두운 청록

//         //검흰
//         blackColor = new Color(0f, 0f, 0f, 200f / 255f); // 검은색, 알파 200
//         whiteColor = new Color(1f, 1f, 1f, 1f); // 흰색, 알파 255
//         alphaColor = new Color(1f, 1f, 1f, 0.3f); // 흰색, 알파 255
//          grayColor = new Color(0.8f, 0.8f, 0.8f, 1f); // 밝은 회색
//     }

//      public void IconsSetting(){
//         //아이콘 24개 세팅하는 함수
//         List<Accessory> accessories = GameManager.instance.dataManager.acceossryList;
//         //아이콘 세팅
//             for (int i = 0; i < accessories.Count; i++)
//         {
//             icon_Acc[i].Init(i);
//         }
//     }








//     public void AccessoryInfo_UISetting(int id){
//         //상세보기 UI 세팅

//         //악세사리 데이터 가져오기
//         AccessoryData data_Staic = accessoryData[id];

//         //데이터 메니저에서 플레이어가 보유한 악세 데이터 리스트 불러오기
//         List<Accessory> data_VarLoad = GameManager.instance.dataManager.acceossryList;

//         //불러온 데이터를 캐시에 저장 data_var은 해당 변수의 값이 동적으로 고정되지 않는 수라는 뜻
//         Accessory data_Var = data_VarLoad[id];

//         //유저가 터치한(불러오고자) 악세의 아이디를 캐싱
//         saveNowId = data_Var.id;

//         //InfoUI 활성화
//         mainUI.gameObject.SetActive(true);


//         // 텍스트 색상 설정, 프레임 색상 설정, 보유, 미보유에 따른 색상 톤 변경
//     Dictionary<AccessoryData.AccessoryGrade, (Color acquired, Color defaultColor)> gradeColors =
//             new Dictionary<AccessoryData.AccessoryGrade, (Color, Color)>
//     {
//         { AccessoryData.AccessoryGrade.Common, (commonColor_W, commonColor) },
//         { AccessoryData.AccessoryGrade.Rare, (rareColor_W, rareColor) },
//         { AccessoryData.AccessoryGrade.Epic, (epicColor_W, epicColor) },
//         { AccessoryData.AccessoryGrade.Legendary, (legendColor_W, legendColor) },
//         { AccessoryData.AccessoryGrade.Mythic, (mythicColor_W, mythicColor) },
//         { AccessoryData.AccessoryGrade.Primordial, (primordialColor_W, primordialColor) }
//     };

//     // 등급 설정
//     text_Rank.text = data_Staic.rank switch
//     {
//         AccessoryData.AccessoryGrade.Common => "일반",
//         AccessoryData.AccessoryGrade.Rare => "희귀",
//         AccessoryData.AccessoryGrade.Epic => "영웅",
//         AccessoryData.AccessoryGrade.Legendary => "전설",
//         AccessoryData.AccessoryGrade.Mythic => "신화",
//         AccessoryData.AccessoryGrade.Primordial => "태초",
//         _ => "알 수 없음"
//     };

//     // 색상 적용
//     (Color acquiredColor, Color defaultColor) = gradeColors[data_Staic.rank];
//     Color selectedColor = data_Var.isAcquired ? acquiredColor : defaultColor;

//     text_Rank.color = selectedColor;


//         //이름 세팅
//         text_Name.text = data_Staic.Name_KOR;

//         //아이콘 세팅
//         info_Acc.Init(id);

//         //Require = 레벨업시 필요한 아이템 수
       
//             //장착 효과 텍스트 세팅
//             SetEquipedEffectTexts(data_Staic, data_Var.level, data_Var.level + 1, true);
//             SetOwnedEffectTexts(data_Staic, data_Var.level, data_Var.level + 1, true);

//             //레벨업 버튼 세팅
//             int Require = ReturnLevelUpRequire(saveNowId);
//             text_RequireLevelUpCount.text = Require.ToString();
//             int count = data_Var.count_Owned;
//             levelUpButton.color = count >= Require? whiteColor : grayColor;
        


//         //장착 여부 
//         if(data_Var.isEquipped == true){
//             EquipButton.gameObject.SetActive(true);
//             levelUpButton.gameObject.SetActive(true);
//             text_Equip.text = "장착중";
//             EquipButton.color = alphaColor;
//         }
//         else if(data_Var.isEquipped != true && data_Var.isAcquired == true){
//             EquipButton.gameObject.SetActive(true);
//              levelUpButton.gameObject.SetActive(true);
//             text_Equip.text = "장착";
//             EquipButton.color = whiteColor;
//         }
//         else if(data_Var.isEquipped != true && data_Var.isAcquired != true){
//             text_Equip.text = "미획득";
//             EquipButton.color = alphaColor;
//             EquipButton.gameObject.SetActive(false);
//              levelUpButton.gameObject.SetActive(false);
//         }

//         //선택한 무기에 따른 중첩 버튼 이미지 세팅
//         levelButton_AccessorySrptie.sprite = data_Staic.MainSprite;
//         string colorCode = count >= Require ? "#00FF00" : "#FF9999"; // 초록색 또는 옅은 붉은색
//         text_RequireLevelUpCount.text = $"<color={colorCode}>{count}</color> / {Require}";



//         IconsSetting();//웨펀 스크롤에 값 적용
        
//     }
//     private void SetEquipedEffectTexts(AccessoryData data_Static, int nowUpgrade, int nextUpgrade, bool showNext)
//     {
//         //초기화
//         text_EquipEffectName.text = "";
//         text_EquipEffectDesc.text = ""; 


    
//             text_EquipEffectName.text = data_Static.equipedTag switch
//             {
//                 AccessoryData.EffectTag.HP => "최대 체력",
//                 AccessoryData.EffectTag.VIT => "초당 체력 회복",
//                 AccessoryData.EffectTag.CRI => "추가 치명타 피해량",
//                 AccessoryData.EffectTag.LUK => "골드 추가 획득량",
//                 _ => "알 수 없음"
//             };

//             text_EquipEffectDesc.text = data_Static.equipedTag switch
//             {
//                 AccessoryData.EffectTag.HP => showNext
//                     ? $"+{data_Static.GetEuipedHP(nowUpgrade)} <color=#00FF00>-> </color><color=#00FF00>+{data_Static.GetEuipedHP(nextUpgrade)}</color>"
//                     : $"+{data_Static.GetEuipedHP(nextUpgrade)}",

//                 AccessoryData.EffectTag.VIT => showNext
//                     ? $"+{data_Static.GetEuipedVIT(nowUpgrade)} <color=#00FF00>-> </color><color=#00FF00>+{data_Static.GetEuipedVIT(nextUpgrade)}</color>"
//                     : $"+{data_Static.GetEuipedVIT(nextUpgrade)}",

//                 AccessoryData.EffectTag.CRI => showNext
//                     ? $"{data_Static.GetEuipedCRI(nowUpgrade)}%<color=#00FF00>-> </color><color=#00FF00>{data_Static.GetEuipedCRI(nextUpgrade)}%</color>"
//                     : $"{data_Static.GetEuipedCRI(nextUpgrade)}%",

//                 AccessoryData.EffectTag.LUK => showNext
//                     ? $"{data_Static.GetEuipedLUK(nowUpgrade)}%<color=#00FF00>-> </color><color=#00FF00>{data_Static.GetEuipedLUK(nextUpgrade)}%</color>"
//                     : $"{data_Static.GetEuipedLUK(nextUpgrade)}%",

//                 _ => "알 수 없음"
//             };
//     }
//     private void SetOwnedEffectTexts(AccessoryData data_Static, int nowLevel, int nextLevel, bool showNext)
// {
//     int lineCount = data_Static.ownedTags.Length;
//     //초기화
//     for (int i = 0; i < text_OwnedEffectName.Length; i++)
//     {
//         text_OwnedEffectName[i].text = "";
//         text_OwnedEffectDesc[i].text = "";
//     }

//     for (int i = 0; i < lineCount; i++)
//     {
//         text_OwnedEffectName[i].text = data_Static.ownedTags[i] switch
//         {
//             AccessoryData.EffectTag.HP => "최대 체력",
//             AccessoryData.EffectTag.VIT => "초당 체력 회복",
//             AccessoryData.EffectTag.CRI => "추가 치명타 피해량",
//             AccessoryData.EffectTag.LUK => "골드 추가 획득량",
//             _ => "알 수 없음"
//         };

//         text_OwnedEffectDesc[i].text = data_Static.ownedTags[i] switch
//         {
//             AccessoryData.EffectTag.HP => showNext
//                 ? $"+{data_Static.GetOwnedHP(nowLevel)} <color=#00FF00>-> </color><color=#00FF00>+{data_Static.GetOwnedHP(nextLevel)}</color>"
//                 : $"+{data_Static.GetOwnedHP(nowLevel)}",

//             AccessoryData.EffectTag.VIT => showNext
//                 ? $"+{data_Static.GetOwnedVIT(nowLevel)} <color=#00FF00>-> </color><color=#00FF00>+{data_Static.GetOwnedVIT(nextLevel)}</color>"
//                 : $"+{data_Static.GetOwnedVIT(nowLevel)}",

//             AccessoryData.EffectTag.CRI => showNext
//                 ? $"{data_Static.GetOwnedCRI(nowLevel)}%<color=#00FF00>-> </color><color=#00FF00>{data_Static.GetOwnedCRI(nextLevel)}%</color>"
//                 : $"{data_Static.GetOwnedCRI(nowLevel)}%",

//             AccessoryData.EffectTag.LUK => showNext
//                 ? $"{data_Static.GetOwnedLUK(nowLevel)}%<color=#00FF00>-> </color><color=#00FF00>{data_Static.GetOwnedLUK(nextLevel)}%</color>"
//                 : $"{data_Static.GetOwnedLUK(nowLevel)}%",

//             _ => "알 수 없음"
//         };
//     }
// }


//     public void EquipedButton(){
//         //장착 버튼 로직

//         //데이터 세팅
//         List<Accessory> data_VarLoad = GameManager.instance.dataManager.acceossryList;
//         Accessory data_Var = data_VarLoad[saveNowId];

//          //장착 여부 
//         if(data_Var.isEquipped == true){
//             //아이템을 장착한 경우 아무일도 일어나지 않음
//         }else if(data_Var.isEquipped != true && data_Var.isAcquired == true){
    
//             for(int i = 0; i < data_VarLoad.Count; i++){
//                 //모든 아이템을 장착해제한 후
//                 data_VarLoad[i].isEquipped = false;
//             }
//                 //선택한 아이템을 장착
//             data_VarLoad[saveNowId].isEquipped = true;
//         }
//         else if(data_Var.isEquipped != true && data_Var.isAcquired != true){
//             //아이템을 미보유한 경우 아무일도 일어나지 않음
//         }
//         GameManager.instance.dataManager.acceossryList = data_VarLoad;//변경된 값 리스트에 적용
//         AccessoryInfo_UISetting(saveNowId);//적용된 값 UI 변경
//         GameManager.instance.player.playerStatus.InitALLStat();//데이터 갱신 
//     }

//      public void LevelUpButton()
//     {
//         //레벨업 버튼, 데이터 세팅
//         List<Accessory> data_VarLoad = GameManager.instance.dataManager.acceossryList;
//         Accessory data_Var = data_VarLoad[saveNowId];

//         // 현재 중첩 수에 따라 필요한 무기량 계산
//         int stackRequire = 2 + data_Var.level;

//         if (data_Var.count_Owned >= stackRequire)
//         {
//             data_Var.count_Owned -= stackRequire; // 무기 수 감소
//             data_Var.level += 1;             // 중첩 1 증가
//             UpgradeEffectAnim();
            
//         }
//         else
//         {
//             // 재료가 부족할 경우 경고 UI 표시
//             GameManager.instance.WarningText("재료가 부족합니다!");
//         }

//         GameManager.instance.dataManager.acceossryList = data_VarLoad; // 변경 사항 저장
//         AccessoryInfo_UISetting(saveNowId);// UI 갱신
//          GameManager.instance.player.playerStatus.InitALLStat();//데이터 갱신 
//     }


//      public void LevelUpAllButton()
//     {
//         //레벨업 가능한 모든 악세를 레벨업하는 딸깍 버튼
//         List<Accessory> accessoriesList = GameManager.instance.dataManager.acceossryList;
//         bool didStack = false; // 중첩이 하나라도 일어났는지 체크용

//         for (int i = 0; i < accessoriesList.Count; i++)
//         {
//             Accessory acc = accessoriesList[i];

//             // 미획득 무기는 건너뛰기
//             if (!acc.isAcquired)
//                 continue;

//             // 가능한 만큼 중첩
//             while (true)
//             {
//                 int stackRequire = 1 + acc.level;

//                 if (acc.count_Owned >= stackRequire)
//                 {
//                     acc.count_Owned -= stackRequire;
//                     acc.level += 1;
//                     didStack = true;
//                 }
//                 else
//                 {
//                     break;
//                 }
//             }
//         }

//         // 변경 사항 저장
//         GameManager.instance.dataManager.acceossryList = accessoriesList;

//         // UI 갱신
//         IconsSetting();
//         // 데이터 갱신
//         GameManager.instance.player.playerStatus.InitALLStat();

//         // 하나도 중첩되지 않았을 경우 경고 출력
//         if (!didStack)
//         {
//             GameManager.instance.WarningText("모두 레벨업 하였습니다!");
//         }
//     }

//      public void XButton(){
//         mainUI.gameObject.SetActive(false);
//     }





//     /**
//     장착 & 보유 효과 Return메소드 모음
//     **/

//     public float ReturnEquipEffect_HP()
//     {
//         List<Accessory> data_VarLoad = GameManager.instance.dataManager.acceossryList;
//         float valueFinal = 0;

//         for (int i = 0; i < data_VarLoad.Count; i++)
//         {
//             if (data_VarLoad[i].isEquipped)
//             {
//                 AccessoryData data_Static = accessoryData[i];
//                 int nowUpgradeLevel = data_VarLoad[i].level;
//                 valueFinal =  data_Static.GetEuipedHP(nowUpgradeLevel);
//                 break; // 장착은 1개만 가능하다고 가정
//             }
//         }

//         return valueFinal;
//     }

//     public float ReturnEquipEffect_VIT()
//     {
//         List<Accessory> data_VarLoad = GameManager.instance.dataManager.acceossryList;
//         float valueFinal = 0;

//         for (int i = 0; i < data_VarLoad.Count; i++)
//         {
//             if (data_VarLoad[i].isEquipped)
//             {
//                 AccessoryData data_Static = accessoryData[i];
//                 int nowUpgradeLevel = data_VarLoad[i].level;
//                 valueFinal = data_Static.GetEuipedVIT(nowUpgradeLevel);
//                 break; // 장착은 1개만 가능하다고 가정
//             }
//         }

//         return valueFinal;
//     }

//      public float ReturnEquipEffect_CRI()
//     {
//         List<Accessory> data_VarLoad = GameManager.instance.dataManager.acceossryList;
//         float valueFinal = 0;

//         for (int i = 0; i < data_VarLoad.Count; i++)
//         {
//             if (data_VarLoad[i].isEquipped)
//             {
//                 AccessoryData data_Static = accessoryData[i];
//                 int nowUpgradeLevel = data_VarLoad[i].level;
//                 valueFinal = data_Static.GetEuipedCRI(nowUpgradeLevel);
//                 break; // 장착은 1개만 가능하다고 가정
//             }
//         }

//         return valueFinal;
//     }

//      public float ReturnEquipEffect_LUK()
//     {
//         List<Accessory> data_VarLoad = GameManager.instance.dataManager.acceossryList;
//         float valueFinal = 0;

//         for (int i = 0; i < data_VarLoad.Count; i++)
//         {
//             if (data_VarLoad[i].isEquipped)
//             {
//                 AccessoryData data_Static = accessoryData[i];
//                 int nowUpgradeLevel = data_VarLoad[i].level;
//                 valueFinal =data_Static.GetEuipedLUK(nowUpgradeLevel);
//                 break; // 장착은 1개만 가능하다고 가정
//             }
//         }

//         return valueFinal;
//     }


//     public float ReturnOwnedHPEffect(){
//         //효과 총량 계산해서 리턴해주는 함수
//         List<Accessory> data_VarLoad = GameManager.instance.dataManager.acceossryList;
//         float valueFinal = 0;
//         for(int i =0; i<data_VarLoad.Count; i++){
//             AccessoryData data_Staic = accessoryData[i];
//             int nowUpgradeLevel = data_VarLoad[i].level;
//             float ownedValue = data_Staic.GetOwnedHP(nowUpgradeLevel);

//             if(data_VarLoad[i].isAcquired == true)
//                 valueFinal = ownedValue;
//         }
//         return valueFinal;
//     }

//     public float ReturnOwnedVITEffect(){
//         //무기 보유 효과 총량 계산해서 리턴해주는 함수
//         List<Accessory> data_VarLoad = GameManager.instance.dataManager.acceossryList;
//         float valueFinal = 0;
//         for(int i =0; i<data_VarLoad.Count; i++){
//             AccessoryData data_Staic = accessoryData[i];
//             int nowUpgradeLevel = data_VarLoad[i].level;
//             float ownedValue = data_Staic.GetOwnedVIT(nowUpgradeLevel);

//             if(data_VarLoad[i].isAcquired == true)
//                 valueFinal = ownedValue;
//         }
//         return valueFinal;
//     }

//     public float ReturnOwnedCRIEffect(){
//         //무기 보유 효과 총량 계산해서 리턴해주는 함수
//         List<Accessory> data_VarLoad = GameManager.instance.dataManager.acceossryList;
//         float valueFinal = 0;
//         for(int i =0; i<data_VarLoad.Count; i++){
//             AccessoryData data_Staic = accessoryData[i];
//             int nowUpgradeLevel = data_VarLoad[i].level;
//             float ownedValue = data_Staic.GetOwnedCRI(nowUpgradeLevel);
//             if(data_VarLoad[i].isAcquired == true)
//                 valueFinal = ownedValue;
//         }
//         return valueFinal;
//     }

//     public float ReturnOwnedLUKEffect(){
//         //무기 보유 효과 총량 계산해서 리턴해주는 함수
//         List<Accessory> data_VarLoad = GameManager.instance.dataManager.acceossryList;
//         float valueFinal = 0;
//         for(int i =0; i<data_VarLoad.Count; i++){
//             AccessoryData data_Staic = accessoryData[i];
//             int nowUpgradeLevel = data_VarLoad[i].level;
//             float ownedValue = data_Staic.GetOwnedLUK(nowUpgradeLevel);
//             if(data_VarLoad[i].isAcquired == true)
//                 valueFinal = ownedValue;
//         }
//         return valueFinal;
//     }
//     public int ReturnLevelUpRequire(int id)
//     {
//         Accessory data_Var = GameManager.instance.dataManager.acceossryList[id];
//         int require = Mathf.Min(15, 1 + data_Var.level);
//         return require;
//     }

//     /**
//     리턴 함수 종료
//     **/

//      public void OnPointerDown()
//     {
//        InvokeRepeating(nameof(LevelUpButton), 0.5f, 0.1f); // 0.3초마다 반복 실행
//     }

//     public void OnPointerUp()
//     {
//        CancelInvoke(nameof(LevelUpButton)); // 업그레이드 중단
//     }
//     public void UpgradeEffectAnim(){
//         // 🔹 이펙트 생성
//             RectTransform effect = effectPooling.Get(0).GetComponent<RectTransform>();

//             // 1️⃣ 아이콘의 월드 좌표 가져오기
//            Vector3 worldPosition = info_Acc.frame.transform.position;

//             // 2️⃣ 이펙트도 월드 좌표로 변경
//             effect.position = worldPosition;
//     }


// }




























