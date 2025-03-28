using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;


public class WeaponManager : MonoBehaviour
{
    [Header("# DATA")]
    public WeaponsData[] weaponsData;//장비 데이터
    public Icon_Weapon[] icon_Weapons;//장비 아이콘 UI
    public Icon_Weapon info_Weapon;//상세 보기 아이콘
    private int saveNowWeaponId;//현재 켜져있는 아이템 UI가 무엇인지

    [Header("# COLOR")]
    public Color commonColor_W, commonColor;
    public Color rareColor_W, rareColor;
    public Color epicColor_W, epicColor;
    public Color legendColor_W, legendColor;
    public Color mythicColor_W, mythicColor;         // 신화 등급 색상
    public Color primordialColor_W, primordialColor; // 태초 등급 색상
    public Color blackColor;
    public Color whiteColor;
    public Color alphaColor;
    
    [Header("# INFO UI")]
    public GameObject info_UI;//인포UI 오브젝트 본체
    public TMP_Text text_Rank;//등급
    public TMP_Text text_Name;//이름
    public TMP_Text text_EquipEffect;//장착 효과 텍스트
    public TMP_Text[] text_OwnedEffectTagName;//변하는 보유 효과
    public TMP_Text[] test_OwnedEffect;//변하는 보유 효과
    public Image EquipButton;//장착 버튼
    public TMP_Text equipText;//장착 or 장착중
    public Image stackButton_WeaponSrptie;//스택 버튼에 있는 무기이미지 변경
    public GameObject levelUpButton;//레벨업 버튼

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

    public void WeaponImageSetting(){

        List<Weapon> weapons = GameManager.instance.dataManager.weaponList;
        //아이콘 세팅
            for (int i = 0; i < weapons.Count; i++)
        {
            icon_Weapons[i].Init(i);
        }

    }

    public void Setting_WeaponInfoUI(int weaponId){
        //weapon 상세보기 UI 세팅
    
        //변하지 않는 데이터 캐싱
        WeaponsData data_Staic = weaponsData[weaponId];

        //변수형 데이터 캐싱
        List<Weapon> data_VarLoad = GameManager.instance.dataManager.weaponList;
        Weapon data_Var = data_VarLoad[weaponId];
        saveNowWeaponId = weaponId;

        //UI 활성화
        info_UI.gameObject.SetActive(true);


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
        text_Rank.text = data_Staic.weaponGrade switch
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
    text_Rank.color = selectedColor;

     //이름 세팅
    text_Name.text = data_Staic.weaponName_KOR;

    //아이콘 세팅
    info_Weapon.Init(saveNowWeaponId);

    //장착 효과 텍스트 세팅
    int nowLevel = data_Var.level;
    int nextLevel = data_Var.level + 1;

    float equipValue_Now = data_Staic.GetEuipedATK(nowLevel);
    float equipValue_Next = data_Staic.GetEuipedATK(nextLevel);

    //장착 효과 텍스트
    text_EquipEffect.text = $"{equipValue_Now}%<color=#00FF00>-> </color><color=#00FF00>{equipValue_Next}</color><color=#00FF00>%</color>";

    //보유 효과 텍스트
    SetOwnedEffectTexts(data_Staic, data_Var.level, data_Var.level + 1, true);

    //장착 여부 
        if(data_Var.isEquipped == true){
            EquipButton.gameObject.SetActive(true);
            equipText.text = "장착중";
            EquipButton.color = alphaColor;
        }
        else if(data_Var.isEquipped != true && data_Var.isAcquired == true){
            EquipButton.gameObject.SetActive(true);
            equipText.text = "장착";
            EquipButton.color = whiteColor;
        }
        else if(data_Var.isEquipped != true && data_Var.isAcquired != true){
            equipText.text = "미획득";
            EquipButton.color = alphaColor;
            levelUpButton.SetActive(false);
            EquipButton.gameObject.SetActive(false);
        }

        //선택한 무기에 따른 중첩 버튼 이미지 세팅
        stackButton_WeaponSrptie.sprite = data_Staic.weaponMainSprite;

        //웨펀 스크롤 초기화
        WeaponImageSetting();
    }
    

    private void SetOwnedEffectTexts(WeaponsData data_Static, int nowLevel, int nextLevel, bool showNext)
{
    int lineCount = data_Static.tags.Length;

    for (int i = 0; i < text_OwnedEffectTagName.Length; i++)
    {
        text_OwnedEffectTagName[i].text = "";
        test_OwnedEffect[i].text = "";
    }

    for (int i = 0; i < lineCount; i++)
    {
        text_OwnedEffectTagName[i].text = data_Static.tags[i] switch
        {
            WeaponsData.EffectTag.ATK => "공격력",
            WeaponsData.EffectTag.CRI => "치명타 공격력",
            _ => "알 수 없음"
        };

        test_OwnedEffect[i].text = data_Static.tags[i] switch
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
        Setting_WeaponInfoUI(weaponId);//적용된 값 UI 변경
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
        int stackRequire = 1 + data_Var.level;

        if (data_Var.weaponCount >= stackRequire)
        {
            data_Var.weaponCount -= stackRequire; // 무기 수 감소
            data_Var.level += 1;             // 중첩 1 증가
        }
        else
        {
            // 재료가 부족할 경우 경고 UI 표시
            GameManager.instance.WarningText("재료가 부족합니다!");
        }

        GameManager.instance.dataManager.weaponList = data_VarLoad; // 변경 사항 저장
        Setting_WeaponInfoUI(weaponId); // UI 갱신
    }

    public void AllLevelUpButton()
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
                int stackRequire = 1 + weapon.level;

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
            GameManager.instance.WarningText("레벨업 가능한 장비가 없습니다!");
        }
    }
        

    public void WeaponUI_XButton(){
        info_UI.gameObject.SetActive(false);
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
            int nowUpgradeLevel = data_VarLoad[i].level;
    
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
                $"강화: {weapon.level}, " +
            //  $"등급: {weapon.grade}, " +
                $"중첩: {weapon.level}, " +
                $"장착 여부: {weapon.isEquipped}, " +
                $"보유 개수: {weapon.weaponCount}, " +
                $"획득 여부: {weapon.isAcquired}");
    }
}

}

