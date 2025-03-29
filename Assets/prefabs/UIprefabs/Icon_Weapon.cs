using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class Icon_Weapon : MonoBehaviour
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
    private List<Weapon> weapons;

    private WeaponManager weaponManager;
    private DataManager dataManager;

    void Awake()
    {
        weaponManager = GameManager.instance.weaponManager;
        dataManager = GameManager.instance.dataManager;
    }
    void OnEnable()
    {
        weapons = GameManager.instance.dataManager.weaponList;
    }
    public void Init(int id){
    
        //id 세팅
        this.id = id;

        //아이콘 스프라이트 세팅 
        mainIcon.sprite = weaponManager.weaponsData[id].weaponMainSprite;

        //강화 수치 TEXT 세팅
        text_LevelUp.text = "Lv." + weapons[id].level.ToString();

        // ✅ 중첩 요구량 계산 (현재 레벨 + 1)
        int levelUp_Require = weaponManager.ReturnLevelUpRequire(id);

        //게이지 FILL 수치 세팅
        int weaponCount = weapons[id].weaponCount;
        fill.fillAmount = Mathf.Clamp01((float)weaponCount / levelUp_Require);
        text_UnderFill.text = weaponCount.ToString() + " / " + levelUp_Require;

        //화살표 오브젝트 연출
        upArrow.gameObject.SetActive(weaponCount>=levelUp_Require);

        //장착한 장비라면 밝게 연출
        bool isOwned = weapons[id].isAcquired;
        ownedBackGround.SetActive(!isOwned);
        mainIcon.color = isOwned ? weaponManager.whiteColor : weaponManager.blackColor;
        Eicons.SetActive(isOwned && weapons[id].isEquipped);

        //랭크별 프레임 색상 변경
        frame.effectColor = weaponManager.weaponsData[id].weaponGrade switch
    {
        WeaponGrade.Common => weaponManager.commonColor_W,
        WeaponGrade.Rare => weaponManager.rareColor_W,
        WeaponGrade.Epic => weaponManager.epicColor_W,
        WeaponGrade.Legendary => weaponManager.legendColor_W,
        WeaponGrade.Mythic => weaponManager.mythicColor_W,
        WeaponGrade.Primordial => weaponManager.primordialColor_W,
        _ => weaponManager.commonColor_W,
    };
    }
    public void IconTouch(){
        weaponManager.Setting_WeaponInfoUI(id);
    }
    

}
