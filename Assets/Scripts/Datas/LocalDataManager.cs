using Game.CardData;
using UnityEngine;

public class LocalDataManager : MonoBehaviour
{
    public static LocalDataManager Instance { get; private set; }

    private void Awake()
    {
        // 이미 인스턴스가 존재하면 자기 자신 삭제 (중복 방지)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 인스턴스 등록
        Instance = this;

        // 씬이 바뀌어도 파괴되지 않게 설정
        //DontDestroyOnLoad(gameObject);
    }

    // 🔹 예시: 로컬 리소스(예: 카드 데이터, 사운드, 언어 설정 등)
    [Header("Card Resources")]
    public CardData cardData;
    

    // 🔹 예시 함수
    public void PrintDebug()
    {
        Debug.Log("[LocalDataManager] 로컬 데이터 접근 성공!");
    }
}
