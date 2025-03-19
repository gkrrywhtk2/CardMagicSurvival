using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeButtonHandler : MonoBehaviour
{
    public int upgradeType; // Inspector에서 설정 가능

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
        int nowPlayerLevel = GameManager.instance.dataManager.playerLevel;
        int maxEXP = nowPlayerLevel * 1000; // 임시, 필요 경험치 함수
        int nowEXP = GameManager.instance.dataManager.expPoint;

        if(nowEXP >= maxEXP){
            GameManager.instance.dataManager.playerLevel++;
            GameManager.instance.dataManager.expPoint -= maxEXP;
        }else{
            Debug.Log("경험치가 부족합니다");
        }
    }

}
