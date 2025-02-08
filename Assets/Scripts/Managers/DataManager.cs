using UnityEngine;

public class DataManager : MonoBehaviour
{
    //백엔드 서버와 이어주는 중간 연결다리
    public float goldPoint;//현재 플레이어의 골드량

    [Header("#Player Info_UpgradeLevel")]
    public int level_ATK;//공격력 레벨
    public int level_Hp;//최대 체력 레벨
    public int level_HpRecovery;//최력 회복량 레벨
    public int level_CriticalDamage;//치명타 배율 레벨
    public int level_CriticalPer;//치명타 확률 레벨

    private void Awake() {
        //임시로 레벨 세팅
        UpgradeLevelSetting();
    }

    public void UpgradeLevelSetting(){
        //게임이 시작될때 저장되어있던 param값을 받아서 UpgradeLevel을 세팅한다.
        level_ATK = 1;
        level_Hp = 1;
        level_HpRecovery = 1;
        level_CriticalDamage = 1;
        level_CriticalPer = 1;
    }

    public void ChageToRealValue(){
        //Upgrade 레벨을 받아서 실제 플레이어에게 적용되는 값으로 바꿔주는 함수.
        
        float real_ATK = level_ATK * 2;//1레벨당 증가량 2
        float real_HP = 100 + (level_Hp * 10);//1레벨당 증가량 10
        float real_HPRecovery = level_HpRecovery * 0.1f;//1레벨당 증가량 0.1%
        float real_CriticalDamage = level_CriticalDamage;//1레벨당 증가량 1%
        float real_criticalPer = level_CriticalPer * 0.1f;//1레벨당 증가량 0.1%

        Player_Status player =  GameManager.instance.player.playerStatus;
        player.ATK = real_ATK;
        player.maxHealth = real_HP;
        player.health += 10;//최대체력 증가량만큼 현재체력 회복
        player.healthRecoveryPer = real_HPRecovery;
        player.CriticalDamagePer = real_CriticalDamage;
        player.CriticalPer = real_criticalPer;

    }
}
