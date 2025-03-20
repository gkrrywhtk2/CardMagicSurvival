using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeButtonHandler : MonoBehaviour
{
    public int upgradeType; // Inspector에서 설정 가능
    DataManager mainData;
    UpgradeUI upgradeUI;
    void Awake()
    {
        mainData = GameManager.instance.dataManager;
        upgradeUI = GameManager.instance.boardUI.upgradeUI;

        
    }

    public void OnPointerDown()
    {
       InvokeRepeating(nameof(UpgradeTo), 0.5f, 0.1f); // 0.3초마다 반복 실행
    }

    public void OnPointerUp()
    {
       CancelInvoke(nameof(UpgradeTo)); // 업그레이드 중단
    }
    private void UpgradeTo(){
        if(upgradeType >= 0){
            GameManager.instance.boardUI.upgradeUI.UpgradeButton(upgradeType);
        }else{
            PlayerLevelUp();
        }
    }
    public void PlayerLevelUp()
    {
        
        int nowPlayerLevel = mainData.playerLevel;
        int maxEXP = nowPlayerLevel * 1000; // 임시, 필요 경험치 함수
        int nowEXP = mainData.expPoint;

        if(nowEXP >= maxEXP){
            mainData.playerLevel++;
            mainData.expPoint -= maxEXP;
            mainData.cur_statPoint += 1;//스탯 포인트 1추가
            upgradeUI.EXPUpdate();
            upgradeUI.ShowLevelUpAnimation();//반짝 애니메이션
        }else{
            Debug.Log("경험치가 부족합니다");
        }
    }

}
