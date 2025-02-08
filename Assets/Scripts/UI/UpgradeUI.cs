using UnityEngine;
using TMPro;
using System;

public class UpgradeUI : MonoBehaviour
{
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


  public void ATK_Setting(){
    DataManager data = GameManager.instance.dataManager;
    int nowLevel = data.level_ATK;
    float desc_Now = nowLevel * 10;
    float desc_After = (nowLevel + 1) * 10;

    //text setting
    ATK_Text_level.text = "Lv." + nowLevel;
    ATK_Text_Desc.text = desc_Now + "->" + desc_After;
    ATK_Text_Gold.text = GetGoldForLevel_0(nowLevel).ToString();
  }
    public static int GetGoldForLevel_0(int level)
    {
        //등차가 2씩 증가하는 골드값 리턴
        if (level < 1) throw new ArgumentException("레벨은 1 이상이어야 합니다.");

        int firstTerm = 10; // 첫 번째 레벨의 필요 골드
        int commonDifference = 1; // 초기 등차
        int requiredGold = firstTerm; // 첫 번째 레벨 필요 골드

        for (int i = 1; i < level; i++)
        {
            requiredGold += commonDifference;
            commonDifference += 2; // 등차 2씩 증가
        }

        return requiredGold;
    }

    public void Button_ATK(){
        GameManager.instance.dataManager.level_ATK += 1;
        ATK_Setting();

    }
}
