using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeButtonHandler : MonoBehaviour
{
    public int upgradeType; // Inspector에서 설정 가능
    private bool isPressing = false; // 버튼이 눌려있는지 확인
     public enum UpgradeType{ATK, MaxHp, HpRecovery, CriticalDamage, CriticalPer};
    UpgradeType upgradeType_;

    private void Start()
    {
        isPressing = false;
    }

    public void OnPointerDown()
    {
        isPressing = true;
       InvokeRepeating(nameof(UpgradeTo), 0.5f, 0.1f); // 0.3초마다 반복 실행
    }

    public void OnPointerUp()
    {
        isPressing = false;
       CancelInvoke(nameof(UpgradeTo)); // 업그레이드 중단
    }
    private void UpgradeTo(){
        GameManager.instance.boardUI.upgradeUI.UpgradeButton(upgradeType);
    }

    private void Upgrade()
    {
        if (!isPressing) return;

        // 기존의 UpgradeButton 로직을 그대로 가져옴
        DataManager data = GameManager.instance.dataManager;
        int requiredGold = GetGoldForLevel((UpgradeType)upgradeType);

        if (data.goldPoint < requiredGold) return;

        data.goldPoint -= requiredGold;

        switch ((UpgradeType)upgradeType)
        {
            case UpgradeType.ATK:
                data.level_ATK += 1;
                break;
            case UpgradeType.MaxHp:
                data.level_Hp += 1;
                GameManager.instance.player.playerStatus.health += 10;
                break;
            case UpgradeType.HpRecovery:
                data.level_HpRecovery += 1;
                break;
            case UpgradeType.CriticalDamage:
                data.level_CriticalDamage += 1;
                break;
            case UpgradeType.CriticalPer:
                data.level_CriticalPer += 1;
                break;
        }

        GameManager.instance.boardUI.upgradeUI.AllUpgradeSetting();
        data.ChageToRealValue();
    }

     public static int GetGoldForLevel(UpgradeType type)
    {
      //업그레이드 항목마다 등차 설정
        DataManager data = GameManager.instance.dataManager;
        int level;//업그레이드 레벨
        int d;//등차

        switch(type){
            case UpgradeType.ATK:
            level = data.level_ATK;
            d = 2;
            break;
            case UpgradeType.MaxHp:
            level = data.level_Hp;
            d = 2;
            break;
            case UpgradeType.HpRecovery:
            level = data.level_HpRecovery;
            d = 2;
            break;
            case UpgradeType.CriticalDamage:
            level = data.level_CriticalDamage;
            d = 2;
            break;
            case UpgradeType.CriticalPer:
            level = data.level_CriticalPer;
            d = 2;
            break;
            default:
            level = 0;
            d = 2;
            break;
        }

        int firstTerm = 10; // 첫 번째 레벨의 필요 골드
        int commonDifference = 1; // 초기 등차
        int requiredGold = firstTerm; // 첫 번째 레벨 필요 골드

        for (int i = 1; i < level; i++)
        {
            requiredGold += commonDifference;
            commonDifference += d; // 등차 2씩 증가
        }

        return requiredGold;
    }

}
