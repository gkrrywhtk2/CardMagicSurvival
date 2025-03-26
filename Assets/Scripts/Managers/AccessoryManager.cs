using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class AccessoryManager : MonoBehaviour
{
    [Header("# DATA")]
    public AccessoryData[] accessoryData;//악세 데이타 모음

    [Header("# UI_LINK")]
    public Image[] Icons;//메인 스프라이트
    public TMP_Text[] upgradeText;//강화 수치
    public TMP_Text[] levelCountText;//레벨 수치
    public Image[] fills;//게이지 수치 fill 오브젝트
    public TMP_Text[] fill_CountText;//0/5 아이템 보유 수치 텍스트
    public GameObject[] getBackGround;//아이템 미획득 백그라운드
    public GameObject[] E_icon;//E 장착표시

    [Header("# COLOR_PRESET")]
    private Color commonColor_W, commonColor;
    private Color rareColor_W, rareColor;
    private Color epicColor_W, epicColor;
    private Color legendColor_W, legendColor;
    private Color mythicColor_W, mythicColor;         
    private Color primordialColor_W, primordialColor;
    private Color blackColor;
    private Color whiteColor;
    private Color alphaColor;


    [Header("# Main UI")]
    public GameObject mainUI; //UI 부모 오브젝트
    public TMP_Text text_Name;
    public TMP_Text text_Rank;
    public TMP_Text text_Upgrade;
    public TMP_Text text_Level;
    public Image MainSprite; //메인 스프라이트
    public Image frame; //등급에 따른 프레임 색상 변경
    public Image fill; //게이지 수치 fill 오브젝트
    public TMP_Text text_FillCount; //0/5 아이템 보유 수치 텍스트
    public GameObject backGround_Unowned; //아이템 미획득 배경

    [Header("# Equip UI")]
    public TMP_Text text_EquipEffectName; //장착 효과 이름
    public TMP_Text text_EquipEffectDesc; //변하는 장착 효과
    public Image EquipButton; //장착 버튼
    public Image EuipIcon; //메인 스프라이트 위에 떠있는 E 표시
    public TMP_Text text_Equip; //장착 or 장착중

    [Header("# Owned Effect UI")]
    public TMP_Text[] text_OwnedEffectName; //변하는 보유 효과 이름
    public TMP_Text[] text_OwnedEffectDesc; //변하는 보유 효과 설명

    [Header("# Upgrade UI")]
    public TMP_Text text_UpgradePostionOwnedCount; //강화 포션 보유량 텍스트
    public TMP_Text text_UpgradePosionRequireCount; //강화 포션 요구량 텍스트
    public Image levelButton_AccessorySrptie; //버튼에 있는 이미지 변경
      public TMP_Text text_RequireLevelUpCount;//레벨업에 필요한 아이템의 수

    [Header("# Warning UI")]
    public GameObject warningCost; //재료가 부족합니다 알림창
    public Animator warningCost_Anim; //알림창 애니메이션

    [Header("# ETC")]
    public int saveNowId; //현재 켜져있는 아이템 UI ID
    public enum UpgradeType { upgrade, levelup };
    public UpgradeType upgradeType = UpgradeType.upgrade;

    [Header("# Buttons")]
    public GameObject levelUpButton;//레벨업 버튼
    public GameObject upgradeButton;//강화 버튼

        private void Awake()
    {
        // 밝은 색상 (획득한 무기)
        commonColor_W = new Color(0.7f, 0.7f, 0.7f, 1f);
        rareColor_W = new Color(0.3f, 0.5f, 0.9f, 1f);
        epicColor_W = new Color(0.6f, 0.3f, 0.8f, 1f);
        legendColor_W = new Color(1f, 0.7f, 0.2f, 1f);
        mythicColor_W = new Color(0.639f, 0.086f, 0.129f, 1f);       // 신화: 붉은색 계열
        primordialColor_W = new Color(0f, 0.721f, 0.580f, 1f);       // 태초: 신비로운 초록

        // 어두운 색상 (미획득 무기)
        commonColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        rareColor = new Color(0.2f, 0.3f, 0.6f, 1f);
        epicColor = new Color(0.4f, 0.2f, 0.5f, 1f);
        legendColor = new Color(0.8f, 0.5f, 0.1f, 1f);
        mythicColor = new Color(0.4f, 0.07f, 0.1f, 1f);               // 어두운 신화: 진한 붉은색
        primordialColor = new Color(0f, 0.4f, 0.3f, 1f);             // 어두운 태초: 어두운 청록

        //검흰
        blackColor = new Color(0f, 0f, 0f, 200f / 255f); // 검은색, 알파 200
        whiteColor = new Color(1f, 1f, 1f, 1f); // 흰색, 알파 255
        alphaColor = new Color(1f, 1f, 1f, 0.3f); // 흰색, 알파 255
    }

     public void IconsSetting(){
        //아이콘 24개 세팅하는 함수
        List<Accessory> accessories = GameManager.instance.dataManager.acceossryList;
        //현재 장비 리스트를 읽어 인게임 이미지를 보여준다
            for (int i = 0; i < accessories.Count; i++)
        {
            // 강화 수치 세팅
            upgradeText[i].text = "+" + accessories[i].count_Upgrade.ToString();

            // 레벨 수치 세팅
            levelCountText[i].text = "Lv." + accessories[i].count_Level.ToString();

            // ✅ 중첩 요구량 계산 (기본값 2 + 현재 스택 수)
            int stackRequire = 2 + accessories[i].count_Level;

            // 게이지 FILL 수치 세팅
            int LvCount = accessories[i].count_Owned;
            fills[i].fillAmount = Mathf.Clamp01((float)LvCount / stackRequire);
            fill_CountText[i].text = LvCount.ToString() + " / " + stackRequire;

            // 미획득 아이템 어둡게 세팅
            getBackGround[i].gameObject.SetActive(true); // 초기화
            Icons[i].color = blackColor;

            if (accessories[i].isAcquired)
            {
                getBackGround[i].gameObject.SetActive(false);
                Icons[i].color = whiteColor;

                // 장착한 무기에 E 표시
                E_icon[i].gameObject.SetActive(accessories[i].isEquipped);
            }
        }
    }








    public void AccessoryInfo_UISetting(int id){
        //상세보기 UI 세팅

        //악세사리 데이터 가져오기
        AccessoryData data_Staic = accessoryData[id];

        //데이터 메니저에서 플레이어가 보유한 악세 데이터 리스트 불러오기
        List<Accessory> data_VarLoad = GameManager.instance.dataManager.acceossryList;

        //불러온 데이터를 캐시에 저장 data_var은 해당 변수의 값이 동적으로 고정되지 않는 수라는 뜻
        Accessory data_Var = data_VarLoad[id];

        //유저가 터치한(불러오고자) 악세의 아이디를 캐싱
        saveNowId = data_Var.id;

        //InfoUI 활성화
        mainUI.gameObject.SetActive(true);


        // 텍스트 색상 설정, 프레임 색상 설정, 보유, 미보유에 따른 색상 톤 변경
    Dictionary<AccessoryData.AccessoryGrade, (Color acquired, Color defaultColor)> gradeColors =
            new Dictionary<AccessoryData.AccessoryGrade, (Color, Color)>
    {
        { AccessoryData.AccessoryGrade.Common, (commonColor_W, commonColor) },
        { AccessoryData.AccessoryGrade.Rare, (rareColor_W, rareColor) },
        { AccessoryData.AccessoryGrade.Epic, (epicColor_W, epicColor) },
        { AccessoryData.AccessoryGrade.Legendary, (legendColor_W, legendColor) },
        { AccessoryData.AccessoryGrade.Mythic, (mythicColor_W, mythicColor) },
        { AccessoryData.AccessoryGrade.Primordial, (primordialColor_W, primordialColor) }
    };

    // 등급 설정
    text_Rank.text = data_Staic.rank switch
    {
        AccessoryData.AccessoryGrade.Common => "일반",
        AccessoryData.AccessoryGrade.Rare => "희귀",
        AccessoryData.AccessoryGrade.Epic => "영웅",
        AccessoryData.AccessoryGrade.Legendary => "전설",
        AccessoryData.AccessoryGrade.Mythic => "신화",
        AccessoryData.AccessoryGrade.Primordial => "태초",
        _ => "알 수 없음"
    };

    // 색상 적용
    (Color acquiredColor, Color defaultColor) = gradeColors[data_Staic.rank];
    Color selectedColor = data_Var.isAcquired ? acquiredColor : defaultColor;

    text_Rank.color = selectedColor;
    frame.color = selectedColor;

        //이름 세팅
        text_Name.text = data_Staic.Name_KOR;

        //강화 수치 세팅
        text_Upgrade.text = "+" + data_Var.count_Upgrade.ToString(); 

        //중첩 수치 세팅
        text_Level.text = "Lv." + data_Var.count_Level.ToString();

        //게이지 FILL 수치 세팅
        // 중첩에 필요한 무기 수 = 2 + 현재 중첩 수치
        int Require = 2 + data_Var.count_Level;
        // 게이지 FILL 수치 세팅
        int ownedCount = data_Var.count_Owned;
        fill.fillAmount = Mathf.Clamp01((float)ownedCount / Require);
        text_FillCount.text = ownedCount + " / " + Require;

        //미획득 어둡게 세팅 + 메인 스프라이트 세팅
        
        backGround_Unowned.gameObject.SetActive(true);//어둡게 초기화
        MainSprite.sprite = data_Staic.MainSprite;//스프라이트 세팅
            if(data_Var.isAcquired == true){//아이템 보유중이라면 
                backGround_Unowned.gameObject.SetActive(false);// 밝게 하고
                  MainSprite.color = whiteColor; // 흰색 + 알파 255
            }else{
                backGround_Unowned.gameObject.SetActive(true);// 밝게 하고
                   MainSprite.color = blackColor; // 흰색 + 알파 255
            }

        //장착 효과 텍스트 세팅 , 분기
       switch (upgradeType)
{
    case UpgradeType.upgrade:
        {
            //장착 효과 텍스트 세팅
            SetEquipedEffectTexts(data_Staic, data_Var.count_Upgrade, data_Var.count_Upgrade + 1, true);
            SetOwnedEffectTexts(data_Staic, data_Var.count_Level, data_Var.count_Level + 1, false);

            //버튼 세팅
            upgradeButton.gameObject.SetActive(true);
            levelUpButton.gameObject.SetActive(false);
        }
        break;

    case UpgradeType.levelup:
        {
           
            SetEquipedEffectTexts(data_Staic, data_Var.count_Upgrade, data_Var.count_Upgrade + 1, false);
            SetOwnedEffectTexts(data_Staic, data_Var.count_Level, data_Var.count_Level + 1, true);

            //버튼 세팅
            upgradeButton.gameObject.SetActive(false);
            levelUpButton.gameObject.SetActive(true);
            text_RequireLevelUpCount.text = Require.ToString();
        }
        break;
}

        //보유 포션량
        text_UpgradePostionOwnedCount.text = GameManager.instance.dataManager.upgradePostionCount.ToString();

       //요구 포션량 공식 = 재료 요구량 + (재료 요구량 * 강화 레벨) * 1.1 (10% 추가), 오류있음
        int baseCost = data_Staic.UpgradeCost * data_Var.count_Upgrade;
        int postionCost = data_Staic.UpgradeCost + (int)(baseCost * 1.1f);
        text_UpgradePosionRequireCount.text = postionCost.ToString();//적용

        //장착 여부 
        if(data_Var.isEquipped == true){
            EquipButton.gameObject.SetActive(true);
            text_Equip.text = "장착중";
            EquipButton.color = alphaColor;
            EuipIcon.gameObject.SetActive(true);
        }
        else if(data_Var.isEquipped != true && data_Var.isAcquired == true){
            EquipButton.gameObject.SetActive(true);
            text_Equip.text = "장착";
            EquipButton.color = whiteColor;
                EuipIcon.gameObject.SetActive(false);
        }
        else if(data_Var.isEquipped != true && data_Var.isAcquired != true){
            text_Equip.text = "미획득";
            EquipButton.color = alphaColor;
            EuipIcon.gameObject.SetActive(false);
            upgradeButton.SetActive(false);
            levelUpButton.SetActive(false);
            EquipButton.gameObject.SetActive(false);
        }

        //선택한 무기에 따른 중첩 버튼 이미지 세팅
        levelButton_AccessorySrptie.sprite = data_Staic.MainSprite;



        IconsSetting();//웨펀 스크롤에 값 적용
        
    }
    private void SetEquipedEffectTexts(AccessoryData data_Static, int nowUpgrade, int nextUpgrade, bool showNext)
    {
        //초기화
        text_EquipEffectName.text = "";
        text_EquipEffectDesc.text = ""; 


    
            text_EquipEffectName.text = data_Static.equipedTag switch
            {
                AccessoryData.EffectTag.HP => "최대 체력",
                AccessoryData.EffectTag.VIT => "초당 체력 회복",
                AccessoryData.EffectTag.CRI => "추가 치명타 피해량",
                AccessoryData.EffectTag.LUK => "골드 추가 획득량",
                _ => "알 수 없음"
            };

            text_EquipEffectDesc.text = data_Static.equipedTag switch
            {
                AccessoryData.EffectTag.HP => showNext
                    ? $"+{data_Static.GetEuipedHP(nowUpgrade)} <color=#00FF00>-> </color><color=#00FF00>+{data_Static.GetEuipedHP(nextUpgrade)}</color>"
                    : $"+{data_Static.GetEuipedHP(nextUpgrade)}",

                AccessoryData.EffectTag.VIT => showNext
                    ? $"+{data_Static.GetEuipedVIT(nowUpgrade)} <color=#00FF00>-> </color><color=#00FF00>+{data_Static.GetEuipedVIT(nextUpgrade)}</color>"
                    : $"+{data_Static.GetEuipedVIT(nextUpgrade)}",

                AccessoryData.EffectTag.CRI => showNext
                    ? $"{data_Static.GetEuipedCRI(nowUpgrade)}%<color=#00FF00>-> </color><color=#00FF00>{data_Static.GetEuipedCRI(nextUpgrade)}%</color>"
                    : $"{data_Static.GetEuipedCRI(nextUpgrade)}%",

                AccessoryData.EffectTag.LUK => showNext
                    ? $"{data_Static.GetEuipedLUK(nowUpgrade)}%<color=#00FF00>-> </color><color=#00FF00>{data_Static.GetEuipedLUK(nextUpgrade)}%</color>"
                    : $"{data_Static.GetEuipedLUK(nextUpgrade)}%",

                _ => "알 수 없음"
            };
    }
    private void SetOwnedEffectTexts(AccessoryData data_Static, int nowLevel, int nextLevel, bool showNext)
{
    int lineCount = data_Static.ownedTags.Length;
    //초기화
    for (int i = 0; i < text_OwnedEffectName.Length; i++)
    {
        text_OwnedEffectName[i].text = "";
        text_OwnedEffectDesc[i].text = "";
    }

    for (int i = 0; i < lineCount; i++)
    {
        text_OwnedEffectName[i].text = data_Static.ownedTags[i] switch
        {
            AccessoryData.EffectTag.HP => "최대 체력",
            AccessoryData.EffectTag.VIT => "초당 체력 회복",
            AccessoryData.EffectTag.CRI => "추가 치명타 피해량",
            AccessoryData.EffectTag.LUK => "골드 추가 획득량",
            _ => "알 수 없음"
        };

        text_OwnedEffectDesc[i].text = data_Static.ownedTags[i] switch
        {
            AccessoryData.EffectTag.HP => showNext
                ? $"+{data_Static.GetOwnedHP(nowLevel)} <color=#00FF00>-> </color><color=#00FF00>+{data_Static.GetOwnedHP(nextLevel)}</color>"
                : $"+{data_Static.GetOwnedHP(nowLevel)}",

            AccessoryData.EffectTag.VIT => showNext
                ? $"+{data_Static.GetOwnedVIT(nowLevel)} <color=#00FF00>-> </color><color=#00FF00>+{data_Static.GetOwnedVIT(nextLevel)}</color>"
                : $"+{data_Static.GetOwnedVIT(nowLevel)}",

            AccessoryData.EffectTag.CRI => showNext
                ? $"{data_Static.GetOwnedCRI(nowLevel)}%<color=#00FF00>-> </color><color=#00FF00>{data_Static.GetOwnedCRI(nextLevel)}%</color>"
                : $"{data_Static.GetOwnedCRI(nowLevel)}%",

            AccessoryData.EffectTag.LUK => showNext
                ? $"{data_Static.GetOwnedLUK(nowLevel)}%<color=#00FF00>-> </color><color=#00FF00>{data_Static.GetOwnedLUK(nextLevel)}%</color>"
                : $"{data_Static.GetOwnedLUK(nowLevel)}%",

            _ => "알 수 없음"
        };
    }
}

      public void UpgradeButton(){
        //데이터 세팅
        AccessoryData data_Staic = accessoryData[saveNowId];
        List<Accessory> data_VarLoad = GameManager.instance.dataManager.acceossryList;
        Accessory data_Var = data_VarLoad[saveNowId];
        DataManager data = GameManager.instance.dataManager;//데이터 메니저 호출


        int nowUpgradeLevel = data_Var.count_Upgrade;
        int baseCost = data_Staic.UpgradeCost * nowUpgradeLevel;
        int postionCost = data_Staic.UpgradeCost + (int)(baseCost * 1.1f);

        if(data_Var.isAcquired != true)
            return;

        //강화 포션 버튼
        if(data.upgradePostionCount >= postionCost){
            data.upgradePostionCount -= postionCost;//재료 감소
            data_Var.count_Upgrade += 1;//강화 수치 1 상승
        }else{
            GameManager.instance.WarningText("재료가 부족합니다!");
        }

        GameManager.instance.dataManager.acceossryList = data_VarLoad;//변경된 값 데이터 메니저에 적용
        AccessoryInfo_UISetting(saveNowId);//적용된 값 UI 변경  
    }

    public void EquipedButton(){
        //장착 버튼 로직

        //데이터 세팅
        List<Accessory> data_VarLoad = GameManager.instance.dataManager.acceossryList;
        Accessory data_Var = data_VarLoad[saveNowId];

         //장착 여부 
        if(data_Var.isEquipped == true){
            //아이템을 장착한 경우 아무일도 일어나지 않음
        }else if(data_Var.isEquipped != true && data_Var.isAcquired == true){
    
            for(int i = 0; i < data_VarLoad.Count; i++){
                //모든 아이템을 장착해제한 후
                data_VarLoad[i].isEquipped = false;
            }
                //선택한 아이템을 장착
            data_VarLoad[saveNowId].isEquipped = true;
        }
        else if(data_Var.isEquipped != true && data_Var.isAcquired != true){
            //아이템을 미보유한 경우 아무일도 일어나지 않음
        }
        GameManager.instance.dataManager.acceossryList = data_VarLoad;//변경된 값 리스트에 적용
        AccessoryInfo_UISetting(saveNowId);//적용된 값 UI 변경
    }

     public void LevelUpButton()
    {
        //레벨업 버튼, 데이터 세팅
        List<Accessory> data_VarLoad = GameManager.instance.dataManager.acceossryList;
        Accessory data_Var = data_VarLoad[saveNowId];

        // 현재 중첩 수에 따라 필요한 무기량 계산
        int stackRequire = 2 + data_Var.count_Level;

        if (data_Var.count_Owned >= stackRequire)
        {
            data_Var.count_Owned -= stackRequire; // 무기 수 감소
            data_Var.count_Level += 1;             // 중첩 1 증가
        }
        else
        {
            // 재료가 부족할 경우 경고 UI 표시
            GameManager.instance.WarningText("재료가 부족합니다!");
        }

        GameManager.instance.dataManager.acceossryList = data_VarLoad; // 변경 사항 저장
        AccessoryInfo_UISetting(saveNowId);// UI 갱신
    }


     public void LevelUpAllButton()
    {
        //레벨업 가능한 모든 악세를 레벨업하는 딸깍 버튼
        List<Accessory> accessoriesList = GameManager.instance.dataManager.acceossryList;
        bool didStack = false; // 중첩이 하나라도 일어났는지 체크용

        for (int i = 0; i < accessoriesList.Count; i++)
        {
            Accessory acc = accessoriesList[i];

            // 미획득 무기는 건너뛰기
            if (!acc.isAcquired)
                continue;

            // 가능한 만큼 중첩
            while (true)
            {
                int stackRequire = 2 + acc.count_Level;

                if (acc.count_Owned >= stackRequire)
                {
                    acc.count_Owned -= stackRequire;
                    acc.count_Level += 1;
                    didStack = true;
                }
                else
                {
                    break;
                }
            }
        }

        // 변경 사항 저장
        GameManager.instance.dataManager.acceossryList = accessoriesList;

        // UI 갱신
        IconsSetting();

        // 하나도 중첩되지 않았을 경우 경고 출력
        if (!didStack)
        {
            GameManager.instance.WarningText("모두 레벨업 하였습니다!");
        }
    }

     public void XButton(){
        mainUI.gameObject.SetActive(false);
    }





    /**
    장착 & 보유 효과 Return메소드 모음
    **/

    public float ReturnEquipEffect()
    {
        List<Accessory> data_VarLoad = GameManager.instance.dataManager.acceossryList;
        float valueFinal = 0;

        for (int i = 0; i < data_VarLoad.Count; i++)
        {
            if (data_VarLoad[i].isEquipped)
            {
                AccessoryData data_Static = accessoryData[i];
                int nowUpgradeLevel = data_VarLoad[i].count_Upgrade;

                valueFinal = data_Static.equipedTag switch
                {
                    AccessoryData.EffectTag.HP => data_Static.GetEuipedHP(nowUpgradeLevel),
                    AccessoryData.EffectTag.VIT => data_Static.GetEuipedVIT(nowUpgradeLevel),
                    AccessoryData.EffectTag.CRI => data_Static.GetEuipedCRI(nowUpgradeLevel),
                    AccessoryData.EffectTag.LUK => data_Static.GetEuipedLUK(nowUpgradeLevel),
                    _ => 0,
                };

                break; // 장착은 1개만 가능하다고 가정
            }
        }

        return valueFinal;
    }


    public float ReturnOwnedHPEffect(){
        //무기 보유 효과 총량 계산해서 리턴해주는 함수
        List<Accessory> data_VarLoad = GameManager.instance.dataManager.acceossryList;
        float valueFinal = 0;
        for(int i =0; i<data_VarLoad.Count; i++){
            AccessoryData data_Staic = accessoryData[i];
            int nowUpgradeLevel = data_VarLoad[i].count_Level;
            float ownedValue = data_Staic.GetOwnedHP(nowUpgradeLevel);
            valueFinal = ownedValue;
        }
        return valueFinal;
    }

    public float ReturnOwnedVITEffect(){
        //무기 보유 효과 총량 계산해서 리턴해주는 함수
        List<Accessory> data_VarLoad = GameManager.instance.dataManager.acceossryList;
        float valueFinal = 0;
        for(int i =0; i<data_VarLoad.Count; i++){
            AccessoryData data_Staic = accessoryData[i];
            int nowUpgradeLevel = data_VarLoad[i].count_Level;
            float ownedValue = data_Staic.GetOwnedVIT(nowUpgradeLevel);
            valueFinal = ownedValue;
        }
        return valueFinal;
    }

    public float ReturnOwnedCRIEffect(){
        //무기 보유 효과 총량 계산해서 리턴해주는 함수
        List<Accessory> data_VarLoad = GameManager.instance.dataManager.acceossryList;
        float valueFinal = 0;
        for(int i =0; i<data_VarLoad.Count; i++){
            AccessoryData data_Staic = accessoryData[i];
            int nowUpgradeLevel = data_VarLoad[i].count_Level;
            float ownedValue = data_Staic.GetOwnedCRI(nowUpgradeLevel);
            valueFinal = ownedValue;
        }
        return valueFinal;
    }

    public float ReturnOwnedLUKEffect(){
        //무기 보유 효과 총량 계산해서 리턴해주는 함수
        List<Accessory> data_VarLoad = GameManager.instance.dataManager.acceossryList;
        float valueFinal = 0;
        for(int i =0; i<data_VarLoad.Count; i++){
            AccessoryData data_Staic = accessoryData[i];
            int nowUpgradeLevel = data_VarLoad[i].count_Level;
            float ownedValue = data_Staic.GetOwnedLUK(nowUpgradeLevel);
            valueFinal = ownedValue;
        }
        return valueFinal;
    }

    /**
    리턴 함수 종료
    **/


    /**
    InfoUI의 강화, 레벨업 탭 전환 관련 로직
    **/
    public Image upgradeTap;
    public Image levelUpTap;

    public void SetUpgradeMode()
    {
        upgradeType = UpgradeType.upgrade;
        AccessoryInfo_UISetting(saveNowId);

        // 색상 설정 (활성화: #CF7200, 비활성화: #4B4B4B)
        Color activeColor = new Color32(0xCF, 0x72, 0x00, 0xFF);
        Color inactiveColor = new Color32(0x4B, 0x4B, 0x4B, 0xFF);

        upgradeTap.color = activeColor;
        levelUpTap.color = inactiveColor;
    }

    public void SetLevelMode()
    {
        upgradeType = UpgradeType.levelup;
        AccessoryInfo_UISetting(saveNowId);

        // 색상 설정 (활성화: #CF7200, 비활성화: #4B4B4B)
        Color activeColor = new Color32(0xCF, 0x72, 0x00, 0xFF);
        Color inactiveColor = new Color32(0x4B, 0x4B, 0x4B, 0xFF);

        upgradeTap.color = inactiveColor;
        levelUpTap.color = activeColor;
    }
}



























