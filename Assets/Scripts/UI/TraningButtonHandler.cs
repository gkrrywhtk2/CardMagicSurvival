using UnityEngine;

public class TraningButtonHandler : MonoBehaviour
{
    public int type; // Inspector에서 설정 가능
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
        GameManager.instance.boardUI.upgradeUI.Traning_UpgradeButton(type);
    }
   
}
