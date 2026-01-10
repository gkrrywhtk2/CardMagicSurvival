using TMPro;
using UnityEngine;

public class TopBar : MonoBehaviour
{
[SerializeField] private TMP_Text goldText;

    private void OnEnable()
    {
        // 이벤트 구독
        ServerDataManager.OnGoldChanged += UpdateGoldDisplay;
        
        // ServerDataManager가 준비될 때까지 대기
        if (ServerDataManager.instance != null)
        {
            UpdateGoldDisplay(ServerDataManager.instance.GetCurrentGold());
        }
    }

    private void OnDisable()
    {
        // 이벤트 구독 해제
        ServerDataManager.OnGoldChanged -= UpdateGoldDisplay;
    }

    private void UpdateGoldDisplay(int newGold)
    {
        goldText.text = newGold.ToString();
        // 또는 애니메이션 효과
        // goldText.text = $"{newGold:N0}"; // 천 단위 콤마
    
    }   
}
