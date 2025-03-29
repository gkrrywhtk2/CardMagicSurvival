using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class Icon_Acc : MonoBehaviour
{
     public int id;
    public Image mainIcon;
    public TMP_Text text_LevelUp;//강화 수치
    public Image fill;//게이지 수치 fill
    public TMP_Text text_UnderFill;//0/5 아이템 보유 수치 텍스트
    public GameObject ownedBackGround;//아이템 미획득 백그라운드
    public GameObject Eicons;//E 장착표시
    public Image upArrow;//레벨업 가능 화살표 오브젝트
    public Outline frame;
    private List<Accessory> accessories;

    private AccessoryManager accessoryManager;
    private DataManager dataManager;

     void Awake()
    {
        accessoryManager = GameManager.instance.accessoryManager;
        dataManager = GameManager.instance.dataManager;
    }
    void OnEnable()
    {
        accessories = GameManager.instance.dataManager.acceossryList;
    }
     public void Init(int id){
    
        //id 세팅
        this.id = id;

        //아이콘 스프라이트 세팅 
        mainIcon.sprite = accessoryManager.accessoryData[id].MainSprite;

        //강화 수치 TEXT 세팅
        text_LevelUp.text = "Lv." + accessories[id].level.ToString();

        // ✅ 중첩 요구량 계산 (현재 레벨 + 1)
        int levelUp_Require = accessoryManager.ReturnLevelUpRequire(id);

        //게이지 FILL 수치 세팅
        int weaponCount = accessories[id].count_Owned;
        fill.fillAmount = Mathf.Clamp01((float)weaponCount / levelUp_Require);
        text_UnderFill.text = weaponCount.ToString() + " / " + levelUp_Require;

        //화살표 오브젝트 연출
        upArrow.gameObject.SetActive(weaponCount>=levelUp_Require);

        //장착한 장비라면 밝게 연출
        bool isOwned = accessories[id].isAcquired;
        ownedBackGround.SetActive(!isOwned);
        mainIcon.color = isOwned ? accessoryManager.whiteColor : accessoryManager.blackColor;
        Eicons.SetActive(isOwned && accessories[id].isEquipped);

        //랭크별 프레임 색상 변경
        frame.effectColor = accessoryManager.accessoryData[id].rank switch
    {
        AccessoryData.AccessoryGrade.Common => accessoryManager.commonColor_W,
         AccessoryData.AccessoryGrade.Rare => accessoryManager.rareColor_W,
        AccessoryData.AccessoryGrade.Epic => accessoryManager.epicColor_W,
        AccessoryData.AccessoryGrade.Legendary=> accessoryManager.legendColor_W,
         AccessoryData.AccessoryGrade.Mythic => accessoryManager.mythicColor_W,
        AccessoryData.AccessoryGrade.Primordial => accessoryManager.primordialColor_W,
        _ => accessoryManager.commonColor,
    };
    }

     public void IconTouch(){
        accessoryManager.AccessoryInfo_UISetting(id);
    }
}
