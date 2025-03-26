using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;


public class WeaponManager : MonoBehaviour
{
    public WeaponsData[] weaponsData;
    public Image[] waeaponIcons;
    public TMP_Text[] upgradeText;//강화 수치
    public TMP_Text[] stackCountText;//중첩 수치
    public Image[] fills;//게이지 수치 fill 오브젝트
    public TMP_Text[] weaponCountText;//0/5 아이템 보유 수치 텍스트
    public GameObject[] getBackGround;//아이템 미획득 백그라운드
    public GameObject[] Eicons;//E 장착표시


   // TextColor
    private Color commonColor_W, commonColor;
    private Color rareColor_W, rareColor;
    private Color epicColor_W, epicColor;
    private Color legendColor_W, legendColor;
    private Color mythicColor_W, mythicColor;         // 신화 등급 색상
    private Color primordialColor_W, primordialColor; // 태초 등급 색상


    

    //weapon Icon UI
    public GameObject weaponUI;
    public TMP_Text rankText_WeaponUI;
    public TMP_Text nameText_WeaponUI;
    public TMP_Text upgradeText_WeaponUI;
    public TMP_Text stackCount_WeaponUI;
    public Image fills_WeaponUI;//게이지 수치 fill 오브젝트
    public Image frame_weaponUI;//등급에 따른 프레임 색상 변경
    public TMP_Text weaponCountText_WeaponUI;//0/5 아이템 보유 수치 텍스트
    public GameObject getBackGround_WeaponUI;//아이템 미획득 백그라운드
    public TMP_Text equipEffectVar_Text;//변하는 장착 효과
    public TMP_Text[] ownedEffectName_Text;//변하는 보유 효과
    public TMP_Text[] ownedEffectVar_Text;//변하는 보유 효과
    public Image weaponUI_MainSprite;//메인 스프라이트
    public TMP_Text upgradePostionCountText;//강화 포션 보유량 텍스트
    public int saveNowWeaponId;//현재 켜져있는 아이템 UI가 무엇인지
    public TMP_Text requiredUpgradePotionText;//강화 포션 요구량 텍스트
    public Image EquipButton;//장착 버튼
    public Image EuipIcon;//메인 스프라이트 위에 떠있는 E 표시
    public TMP_Text equipText;//장착 or 장착중
    public Image stackButton_WeaponSrptie;//스택 버튼에 있는 무기이미지 변경
    public GameObject warningCost;//재료가 부족합니다 알림창
    public Animator warningCost_Anim;//재료가 부족합니다 알림창 애니메이션 연출
    public enum UpgradeType{upgrade, levelup};
    public UpgradeType upgradeType = UpgradeType.upgrade;
    
    // 레벨업 버튼 관련 모음
        public GameObject levelUpButton;//레벨업 버튼
        public GameObject upgradeButton;//강화 버튼
        public TMP_Text requireLevelUpCount;//레벨업에 필요한 아이템의 수

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
}

    public void WeaponImageSetting(){
        List<Weapon> weapons = GameManager.instance.dataManager.weaponList;
           Color blackColor = new Color(0f, 0f, 0f, 200f / 255f); // 검은색, 알파 200
            Color whiteColor = new Color(1f, 1f, 1f, 1f); // 흰색, 알파 255
        //현재 장비 리스트를 읽어 인게임 이미지를 보여준다
            for (int i = 0; i < weapons.Count; i++)
        {
            // 강화 수치 세팅
            upgradeText[i].text = "+" + weapons[i].upgrade.ToString();

            // 중첩 수치 세팅
            stackCountText[i].text = "Lv." + weapons[i].level.ToString();

            // ✅ 중첩 요구량 계산 (기본값 2 + 현재 스택 수)
            int stackRequire = 2 + weapons[i].level;

            // 게이지 FILL 수치 세팅
            int weaponCount = weapons[i].weaponCount;
            fills[i].fillAmount = Mathf.Clamp01((float)weaponCount / stackRequire);
            weaponCountText[i].text = weaponCount.ToString() + " / " + stackRequire;

            // 미획득 아이템 어둡게 세팅
            getBackGround[i].gameObject.SetActive(true); // 초기화
            waeaponIcons[i].color = blackColor;

            if (weapons[i].isAcquired)
            {
                getBackGround[i].gameObject.SetActive(false);
                waeaponIcons[i].color = whiteColor;

                // 장착한 무기에 E 표시
                Eicons[i].gameObject.SetActive(weapons[i].isEquipped);
            }
        }

    }

    public void WeaponIconButton_UISetting(int weaponId){
        //weapon 상세보기 UI 세팅
        Color blackColor = new Color(0f, 0f, 0f, 200f / 255f); // 검은색, 알파 200
        Color whiteColor = new Color(1f, 1f, 1f, 1f); // 흰색, 알파 255
        Color alphaColor = new Color(1f, 1f, 1f, 0.3f); // 흰색, 알파 255

        WeaponsData data_Staic = weaponsData[weaponId];
        List<Weapon> data_VarLoad = GameManager.instance.dataManager.weaponList;
        Weapon data_Var = data_VarLoad[weaponId];
        saveNowWeaponId = weaponId;
        weaponUI.gameObject.SetActive(true);


        // 텍스트 색상 설정, 프레임 색상 설정, 보유, 미보유에 따른 색상 톤 변경
    Dictionary<WeaponGrade, (Color acquired, Color defaultColor)> gradeColors = new Dictionary<WeaponGrade, (Color, Color)>
    {
        { WeaponGrade.Common, (commonColor_W, commonColor) },
        { WeaponGrade.Rare, (rareColor_W, rareColor) },
        { WeaponGrade.Epic, (epicColor_W, epicColor) },
        { WeaponGrade.Legendary, (legendColor_W, legendColor) },
        { WeaponGrade.Mythic, (mythicColor_W, mythicColor) },
        { WeaponGrade.Primordial, (primordialColor_W, primordialColor) }
    };

    // 등급 설정
    rankText_WeaponUI.text = data_Staic.weaponGrade switch
    {
        WeaponGrade.Common => "일반",
        WeaponGrade.Rare => "희귀",
        WeaponGrade.Epic => "영웅",
        WeaponGrade.Legendary => "전설",
        WeaponGrade.Mythic => "신화",
        WeaponGrade.Primordial => "태초",
        _ => "알 수 없음"
    };

    // 색상 적용
    (Color acquiredColor, Color defaultColor) = gradeColors[data_Staic.weaponGrade];
    Color selectedColor = data_Var.isAcquired ? acquiredColor : defaultColor;

    rankText_WeaponUI.color = selectedColor;
    frame_weaponUI.color = selectedColor;

        //이름 세팅
        nameText_WeaponUI.text = data_Staic.weaponName_KOR;

        //강화 수치 세팅
        upgradeText_WeaponUI.text = "+" + data_Var.upgrade.ToString(); // +1

        //중첩 수치 세팅
        stackCount_WeaponUI.text = "Lv." + data_Var.level.ToString();

        //게이지 FILL 수치 세팅
       // 중첩에 필요한 무기 수 = 2 + 현재 중첩 수치
        int stackRequire = 2 + data_Var.level;
        // 게이지 FILL 수치 세팅
        int weaponCount = data_Var.weaponCount;
        fills_WeaponUI.fillAmount = Mathf.Clamp01((float)weaponCount / stackRequire);
        weaponCountText_WeaponUI.text = weaponCount + " / " + stackRequire;
        //미획득 어둡게 세팅 + 메인 스프라이트 세팅
        
        getBackGround_WeaponUI.gameObject.SetActive(true);//어둡게 초기화
        weaponUI_MainSprite.sprite = data_Staic.weaponMainSprite;//스프라이트 세팅
            if(data_Var.isAcquired == true){//아이템 보유중이라면 
                getBackGround_WeaponUI.gameObject.SetActive(false);// 밝게 하고
                  weaponUI_MainSprite.color = whiteColor; // 흰색 + 알파 255
            }else{
                getBackGround_WeaponUI.gameObject.SetActive(true);// 밝게 하고
                   weaponUI_MainSprite.color = blackColor; // 흰색 + 알파 255
            }

        //장착 효과 텍스트 세팅 , 분기
       switch (upgradeType)
{
    case UpgradeType.upgrade:
        {
            int nowUpgradeLevel = data_Var.upgrade;
            int nextUpgradeLevel = data_Var.upgrade + 1;

            float equipValue_Now = data_Staic.GetEuipedATK(nowUpgradeLevel);
            float equipValue_Next = data_Staic.GetEuipedATK(nextUpgradeLevel);

            equipEffectVar_Text.text = $"{equipValue_Now}%<color=#00FF00>-> </color><color=#00FF00>{equipValue_Next}</color><color=#00FF00>%</color>";

            SetOwnedEffectTexts(data_Staic, data_Var.level, data_Var.level + 1, false);

            upgradeButton.gameObject.SetActive(true);
            levelUpButton.gameObject.SetActive(false);
        }
        break;

    case UpgradeType.levelup:
        {
            int nowLevel = data_Var.level;
            int nextLevel = data_Var.level + 1;

            int nowUpgradeLevel = data_Var.upgrade;
            float equipValue_Now = data_Staic.GetEuipedATK(nowUpgradeLevel);
            equipEffectVar_Text.text = $"{equipValue_Now}%";

            SetOwnedEffectTexts(data_Staic, nowLevel, nextLevel, true);

            upgradeButton.gameObject.SetActive(false);
            levelUpButton.gameObject.SetActive(true);
            requireLevelUpCount.text = stackRequire.ToString();
        }
        break;
}

        //보유 포션량
        upgradePostionCountText.text = GameManager.instance.dataManager.upgradePostionCount.ToString();

       // 요구 포션량 공식 = 재료 요구량 + (재료 요구량 * 강화 레벨) * 1.1 (10% 추가)
        int baseCost = data_Staic.materialCost_Upgrade * data_Var.upgrade;
        int postionCost = data_Staic.materialCost_Upgrade + (int)(baseCost * 1.1f);
        requiredUpgradePotionText.text = postionCost.ToString();//적용

        //장착 여부 
        if(data_Var.isEquipped == true){
             EquipButton.gameObject.SetActive(true);
            equipText.text = "장착중";
            EquipButton.color = alphaColor;
            EuipIcon.gameObject.SetActive(true);
        }
        else if(data_Var.isEquipped != true && data_Var.isAcquired == true){
             EquipButton.gameObject.SetActive(true);
             equipText.text = "장착";
              EquipButton.color = whiteColor;
                EuipIcon.gameObject.SetActive(false);
        }
        else if(data_Var.isEquipped != true && data_Var.isAcquired != true){
            equipText.text = "미획득";
            EquipButton.color = alphaColor;
            EuipIcon.gameObject.SetActive(false);
            upgradeButton.SetActive(false);
            levelUpButton.SetActive(false);
            EquipButton.gameObject.SetActive(false);
        }

        //선택한 무기에 따른 중첩 버튼 이미지 세팅
        stackButton_WeaponSrptie.sprite = data_Staic.weaponMainSprite;



        WeaponImageSetting();//웨펀 스크롤에 값 적용
        
    }
    
    

    private void SetOwnedEffectTexts(WeaponsData data_Static, int nowLevel, int nextLevel, bool showNext)
{
    int lineCount = data_Static.tags.Length;

    for (int i = 0; i < ownedEffectName_Text.Length; i++)
    {
        ownedEffectName_Text[i].text = "";
        ownedEffectVar_Text[i].text = "";
    }

    for (int i = 0; i < lineCount; i++)
    {
        ownedEffectName_Text[i].text = data_Static.tags[i] switch
        {
            WeaponsData.EffectTag.ATK => "공격력",
            WeaponsData.EffectTag.CRI => "치명타 공격력",
            _ => "알 수 없음"
        };

        ownedEffectVar_Text[i].text = data_Static.tags[i] switch
        {
            WeaponsData.EffectTag.ATK => showNext
                ? $"{data_Static.GetOwnedATK(nowLevel)}%<color=#00FF00>-> </color><color=#00FF00>{data_Static.GetOwnedATK(nextLevel)}</color><color=#00FF00>%</color>"
                : $"{data_Static.GetOwnedATK(nowLevel)}%",

            WeaponsData.EffectTag.CRI => showNext
                ? $"{data_Static.GetOwnedCRI(nowLevel)}%<color=#00FF00>-> </color><color=#00FF00>{data_Static.GetOwnedCRI(nextLevel)}</color><color=#00FF00>%</color>"
                : $"{data_Static.GetOwnedCRI(nowLevel)}%",

            _ => "알 수 없음"
        };
    }
}


    public void UpgradePostionButton(){
        int weaponId = saveNowWeaponId;
        //데이터 세팅
        WeaponsData data_Staic = weaponsData[weaponId];
        List<Weapon> data_VarLoad = GameManager.instance.dataManager.weaponList;
        Weapon data_Var = data_VarLoad[weaponId];
        DataManager data = GameManager.instance.dataManager;//데이터 메니저 호출
        int nowUpgradeLevel = data_Var.upgrade;
        int baseCost = data_Staic.materialCost_Upgrade * nowUpgradeLevel;
        int postionCost = data_Staic.materialCost_Upgrade + (int)(baseCost * 1.1f);

        if(data_Var.isAcquired != true)
            return;

        //강화 포션 버튼
        if(data.upgradePostionCount >= postionCost){
            data.upgradePostionCount -= postionCost;//재료 감소
            data_Var.upgrade += 1;//강화 수치 1 상승
        }else{
            warningCost.gameObject.SetActive(true);
            warningCost_Anim.SetTrigger("Replay");
        }

         GameManager.instance.dataManager.weaponList = data_VarLoad;//변경된 값 데이터 메니저에 적용
        WeaponIconButton_UISetting(weaponId);//적용된 값 UI 변경
       // GameManager.instance.dataManager.ChageToRealValue();//캐릭터 최신화
        
    }

    public void EquipedButton(){
        //데이터 세팅
        int weaponId = saveNowWeaponId;
        //데이터 세팅
       // WeaponData data_Staic = weaponData[weaponId];
        List<Weapon> data_VarLoad = GameManager.instance.dataManager.weaponList;
        Weapon data_Var = data_VarLoad[weaponId];
       // DataManager data = GameManager.instance.dataManager;//데이터 메니저 호출
         //장착 여부 
        if(data_Var.isEquipped == true){
            //아이템을 장착한 경우 아무일도 일어나지 않음
        }
        else if(data_Var.isEquipped != true && data_Var.isAcquired == true){
            
            for(int i = 0; i < data_VarLoad.Count; i++){
                //모든 아이템을 장착해제한 후
                data_VarLoad[i].isEquipped = false;
            }
                //선택한 아이템을 장착
            data_VarLoad[weaponId].isEquipped = true;
           // EquipedImageSetting_Character(weaponId);
        }
        else if(data_Var.isEquipped != true && data_Var.isAcquired != true){
            //아이템을 미보유한 경우 아무일도 일어나지 않음
        }
        GameManager.instance.dataManager.weaponList = data_VarLoad;//변경된 값 리스트에 적용
        WeaponIconButton_UISetting(weaponId);//적용된 값 UI 변경
        
        GameManager.instance.player.playerStatus.GetTotalATK();//ATK 적용

    }
   
     public void EquipedImageSetting_Character(int weaponId){
        //장착한 무기를 캐릭터에게 이미지 적용
        WeaponsData data_Staic = weaponsData[weaponId];
        GameManager.instance.player.joystickP.weaponR.sprite = data_Staic.weaponMainSprite;//스프라이트 세팅
        GameManager.instance.player.joystickP.weaponL.sprite = data_Staic.weaponMainSprite;//스프라이트 세팅
     }
     

    public void LevelUpButton()
    {
        int weaponId = saveNowWeaponId;
        List<Weapon> data_VarLoad = GameManager.instance.dataManager.weaponList;
        Weapon data_Var = data_VarLoad[weaponId];

        // 현재 중첩 수에 따라 필요한 무기량 계산
        int stackRequire = 2 + data_Var.level;

        if (data_Var.weaponCount >= stackRequire)
        {
            data_Var.weaponCount -= stackRequire; // 무기 수 감소
            data_Var.level += 1;             // 중첩 1 증가
        }
        else
        {
            // 재료가 부족할 경우 경고 UI 표시
            warningCost.gameObject.SetActive(true);
            warningCost_Anim.SetTrigger("Replay");
        }

        GameManager.instance.dataManager.weaponList = data_VarLoad; // 변경 사항 저장
        WeaponIconButton_UISetting(weaponId); // UI 갱신
        // GameManager.instance.dataManager.ChageToRealValue(); // 필요 시 적용
    }

    public void StackAllButton()
    {
        List<Weapon> weaponList = GameManager.instance.dataManager.weaponList;
        bool didStack = false; // 중첩이 하나라도 일어났는지 체크용

        for (int i = 0; i < weaponList.Count; i++)
        {
            Weapon weapon = weaponList[i];

            // 미획득 무기는 건너뛰기
            if (!weapon.isAcquired)
                continue;

            // 가능한 만큼 중첩
            while (true)
            {
                int stackRequire = 2 + weapon.level;

                if (weapon.weaponCount >= stackRequire)
                {
                    weapon.weaponCount -= stackRequire;
                    weapon.level += 1;
                    didStack = true;
                }
                else
                {
                    break;
                }
            }
        }

        // 변경 사항 저장
        GameManager.instance.dataManager.weaponList = weaponList;

        // UI 갱신
        WeaponImageSetting();

        // 하나도 중첩되지 않았을 경우 경고 출력
        if (!didStack)
        {
            warningCost.gameObject.SetActive(true);
            warningCost_Anim.SetTrigger("Replay");
        }
    }
        

    public void WeaponUI_XButton(){
        weaponUI.gameObject.SetActive(false);
    }

    public float ReturnOwnedATKEffect(){
        //무기 보유 효과 총량 계산해서 리턴해주는 함수
        List<Weapon> data_VarLoad = GameManager.instance.dataManager.weaponList;
        
        float valueFinal = 0;
        for(int i =0; i<data_VarLoad.Count; i++){
            WeaponsData data_Staic = weaponsData[i];
            int nowUpgradeLevel = data_VarLoad[i].level;
            float ownedValue = data_Staic.GetOwnedATK(nowUpgradeLevel);
            valueFinal += ownedValue;
        }
        return valueFinal;
    }
    public float ReturnOwnedCRIEffect(){
        //무기 보유 효과 총량 계산해서 리턴해주는 함수
        List<Weapon> data_VarLoad = GameManager.instance.dataManager.weaponList;
        
        float valueFinal = 0;
        for(int i =0; i<data_VarLoad.Count; i++){
            WeaponsData data_Staic = weaponsData[i];
            int nowUpgradeLevel = data_VarLoad[i].level;
            float ownedValue = data_Staic.GetOwnedCRI(nowUpgradeLevel);
            valueFinal += ownedValue;
        }
        return valueFinal;
    }

     public float ReturnEquipEffect(){
        //무기 장착 효과 계산해서 리턴해주는 함수 
        //+++무기 이미지 적용까지 이 함수에서 작동
        List<Weapon> data_VarLoad = GameManager.instance.dataManager.weaponList;
        
        float valueFinal = 0;
        for(int i =0; i<data_VarLoad.Count; i++){
            WeaponsData data_Staic = weaponsData[i];
            int nowUpgradeLevel = data_VarLoad[i].upgrade;
    
            if(data_VarLoad[i].isEquipped == true){
                float equopValue = data_Staic.GetEuipedATK(nowUpgradeLevel);
                valueFinal = equopValue;

                //무기 이미지 세팅
                EquipedImageSetting_Character(data_VarLoad[i].weaponId);
                break;
            }
        }
        return valueFinal;
    }

    public void DebugLoadedWeapons()
{
    List<Weapon> loadedWeapons = GameManager.instance.dataManager.weaponList;

    foreach (Weapon weapon in loadedWeapons)
    {
        Debug.Log($"ID: {weapon.weaponId}, " +
                  $"강화: {weapon.upgrade}, " +
                //  $"등급: {weapon.grade}, " +
                  $"중첩: {weapon.level}, " +
                  $"장착 여부: {weapon.isEquipped}, " +
                  $"보유 개수: {weapon.weaponCount}, " +
                  $"획득 여부: {weapon.isAcquired}");
    }
}

    public Image upgradeTap;
    public Image levelUpTap;

    public void SetUpgradeMode()
    {
        upgradeType = UpgradeType.upgrade;
        WeaponIconButton_UISetting(saveNowWeaponId);

        // 색상 설정 (활성화: #CF7200, 비활성화: #4B4B4B)
        Color activeColor = new Color32(0xCF, 0x72, 0x00, 0xFF);
        Color inactiveColor = new Color32(0x4B, 0x4B, 0x4B, 0xFF);

        upgradeTap.color = activeColor;
        levelUpTap.color = inactiveColor;
    }

    public void SetLevelMode()
    {
        upgradeType = UpgradeType.levelup;
        WeaponIconButton_UISetting(saveNowWeaponId);

        // 색상 설정 (활성화: #CF7200, 비활성화: #4B4B4B)
        Color activeColor = new Color32(0xCF, 0x72, 0x00, 0xFF);
        Color inactiveColor = new Color32(0x4B, 0x4B, 0x4B, 0xFF);

        upgradeTap.color = inactiveColor;
        levelUpTap.color = activeColor;
    }


}

