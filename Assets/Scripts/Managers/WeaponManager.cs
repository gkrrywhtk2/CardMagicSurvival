using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using RANK;
using System.Linq.Expressions;

public class WeaponManager : MonoBehaviour
{
    public Image[] waeaponIcons;
    public TMP_Text[] upgradeText;//강화 수치
    public TMP_Text[] stackCountText;//중첩 수치
    public Image[] fills;//게이지 수치 fill 오브젝트
    public TMP_Text[] weaponCountText;//0/5 아이템 보유 수치 텍스트
    public GameObject[] getBackGround;//아이템 미획득 백그라운드
    //
    //weapon Icon UI
    public GameObject weaponUI;
    public TMP_Text rankText_WeaponUI;
    public TMP_Text nameText_WeaponUI;
    public TMP_Text upgradeText_WeaponUI;
    public TMP_Text stackCount_WeaponUI;
    public Image fills_WeaponUI;//게이지 수치 fill 오브젝트
    public TMP_Text weaponCountText_WeaponUI;//0/5 아이템 보유 수치 텍스트
    public GameObject getBackGround_WeaponUI;//아이템 미획득 백그라운드
    public TMP_Text equipEffectVar_Text;//변하는 장착 효과
    public TMP_Text ownedEffectVar_Text;//변하는 보유 효과
    public Image weaponUI_MainSprite;//메인 스프라이트


    //
    public WeaponData[] weaponData;//프론트 엔드에서 관리하는 무기 데이터 관리
    
    public void PrintWeaponList()
    {
        
    foreach (Weapon weapon in GameManager.instance.dataManager.weaponList)
    {
        Debug.Log("Weapon ID: " + weapon.weaponId + 
                  ", Upgrade Level: " + weapon.upgradeLevel + 
                  ", Grade: " + weapon.grade + 
                  ", Stack Count: " + weapon.stackCount + 
                  ", Is Equipped: " + weapon.isEquipped);
    }
    }

    public void WeaponImageSetting(){
        List<Weapon> weapons = GameManager.instance.dataManager.weaponList;
           Color blackColor = new Color(0f, 0f, 0f, 200f / 255f); // 검은색, 알파 200
            Color whiteColor = new Color(1f, 1f, 1f, 1f); // 흰색, 알파 255
        //현재 장비 리스트를 읽어 인게임 이미지를 보여준다
        for(int i = 0; i < weapons.Count; i++){
            //강화 수치 세팅
            upgradeText[i].text = "+" + weapons[i].upgradeLevel.ToString();
             //중첩 수치 세팅
            stackCountText[i].text = weapons[i].stackCount.ToString() + "중첩";
            //게이지 FILL 수치 세팅
            int weaponCount = weapons[i].weaponCount;
            fills[i].fillAmount = Mathf.Clamp01((float)weaponCount / 5);
            weaponCountText[i].text = weaponCount.ToString() + " / 5";
            //미획득 아이템 어둡게 세팅
            getBackGround[i].gameObject.SetActive(true);//초기화
              waeaponIcons[i].color = blackColor; // 기본값: 검은색 + 알파 200 초기화
            if(weapons[i].isAcquired == true){
                getBackGround[i].gameObject.SetActive(false);
                  waeaponIcons[i].color = whiteColor; // 흰색 + 알파 255
            }
        }

    }

    public void WeaponIconButton_UISetting(int weaponId){
         Color blackColor = new Color(0f, 0f, 0f, 200f / 255f); // 검은색, 알파 200
            Color whiteColor = new Color(1f, 1f, 1f, 1f); // 흰색, 알파 255
        WeaponData data_Staic = weaponData[weaponId];
        List<Weapon> data_VarLoad = GameManager.instance.dataManager.weaponList;
        Weapon data_Var = data_VarLoad[weaponId];


        weaponUI.gameObject.SetActive(true);

        //등급Text 세팅
        switch (data_Staic.weaponGrade)
        {
            case WeaponGrade.Common :
            rankText_WeaponUI.text = "일반";
            break;
            case WeaponGrade.Rare :
            rankText_WeaponUI.text = "희귀";
            break;
            case WeaponGrade.Epic :
            rankText_WeaponUI.text = "영웅";
            break;
            case WeaponGrade.Legendary :
            rankText_WeaponUI.text = "전설";
            break;
        }

        //이름 세팅
        nameText_WeaponUI.text = data_Staic.weaponName_KOR;

        //강화 수치 세팅
        upgradeText_WeaponUI.text = "+" + data_Var.upgradeLevel.ToString(); // +1

        //중첩 수치 세팅
        stackCount_WeaponUI.text = data_Var.stackCount.ToString() + "중첩";

        //게이지 FILL 수치 세팅
        int weaponCount = data_Var.weaponCount;
        fills_WeaponUI.fillAmount = Mathf.Clamp01((float)weaponCount / 5);
        weaponCountText_WeaponUI.text = weaponCount + "/ 5";

        //미획득 어둡게 세팅 + 메인 스프라이트 세팅
        getBackGround_WeaponUI.gameObject.SetActive(true);//어둡게 초기화
        weaponUI_MainSprite.sprite = data_Staic.weaponMainSprite;//스프라이트 세팅
            if(data_Var.isAcquired == true){//아이템 보유중이라면 
                getBackGround_WeaponUI.gameObject.SetActive(false);// 밝게 하고
                  weaponUI_MainSprite.color = whiteColor; // 흰색 + 알파 255
            }

        //장착 효과 텍스트 세팅
        int nowUpgradeLevel = data_Var.upgradeLevel;
        int nextUpgradeLevel = data_Var.upgradeLevel + 1;
        float stackCountUpgradeOffset = (data_Var.stackCount * 0.1f) + 1;//1중첩 이면 1.1
        
        //장착 효과 공식 : (해당 장비 공격력 수치 + (해당 장비 공격력 수치 * 강화 수치)) * 1중첩 당 10% 추가 
        float equipValue_Now = (data_Staic.equippedEffect_ATK + (data_Staic.equippedEffect_ATK * nowUpgradeLevel))
                                * stackCountUpgradeOffset;
        float equipValue_Next = (data_Staic.equippedEffect_ATK + (data_Staic.equippedEffect_ATK * nextUpgradeLevel))
                                * stackCountUpgradeOffset;
        
        //보유 효과
        float ownedValue_Now = (data_Staic.ownedEffect_ATK + (data_Staic.ownedEffect_ATK * nowUpgradeLevel))
                                * stackCountUpgradeOffset;
        float ownedValue_Next = (data_Staic.ownedEffect_ATK + (data_Staic.ownedEffect_ATK * nextUpgradeLevel))
                                * stackCountUpgradeOffset;
        equipEffectVar_Text.text = equipValue_Now + "%" + "<color=#00FF00>-> </color>" + "<color=#00FF00>" + 
                                    equipValue_Next + "</color>" + "<color=#00FF00>%</color>";
        ownedEffectVar_Text.text = ownedValue_Now + "%" + "<color=#00FF00>-> </color>" + "<color=#00FF00>" + 
                                    ownedValue_Next + "</color>" + "<color=#00FF00>%</color>";

    }

    public void WeaponUI_XButton(){
        weaponUI.gameObject.SetActive(false);
    }


}

 [System.Serializable]
public class WeaponData
{
    //프론트엔드 웨폰 데이터
    public int weaponId;//무기 고유번호
    public float equippedEffect_ATK;//장착 추가 공격력% * level
    public float ownedEffect_ATK;//보유 효과 * level
    public string weaponName_KOR;//한글 이름
    public WeaponGrade weaponGrade;
    public Sprite weaponMainSprite;//메인 스프라이트
}
