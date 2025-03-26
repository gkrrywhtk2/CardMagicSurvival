using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public TMP_Text text_EquipEffect; //변하는 장착 효과
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
    public TMP_Text text_RequireLevelUpCount;//레벨업에 필요한 아이템의 수



}
