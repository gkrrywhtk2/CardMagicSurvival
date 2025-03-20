using UnityEngine;
using UnityEngine.UI;

public class RedDotController : MonoBehaviour
{
    public Button targetButton;  // 빨간 동그라미를 추가할 버튼
    public GameObject redDotPrefab;  // 빨간 동그라미 이미지 프리팹
    private GameObject redDotInstance;  // 생성된 빨간 동그라미 인스턴스

    void Start()
    {
        // 빨간 동그라미를 초기화 시 추가
        //AddRedDot();
    }

    // 버튼이 활성화되거나 비활성화되는 조건에 따라 빨간 동그라미를 추가하거나 제거
    public void UpdateRedDot(bool isActive)
    {
        if (isActive)
        {
            if (redDotInstance == null)
            {
                AddRedDot();
            }
        }
        else
        {
            if (redDotInstance != null)
            {
                Destroy(redDotInstance);
            }
        }
    }

    private void AddRedDot()
    {
        // 빨간 동그라미 이미지를 버튼 위에 자식으로 추가합니다.
            redDotInstance = Instantiate(redDotPrefab, targetButton.transform);
        //redDotInstance.transform.localPosition = new Vector3(30, 30, 0);  // 버튼의 오른쪽 상단에 위치시킵니다. 필요시 위치 조정 가능
        //redDotInstance.GetComponent<Image>().color = Color.red;  // 빨간색으로 설정 (이 부분은 프리팹에서 처리할 수도 있음)
    }
}
