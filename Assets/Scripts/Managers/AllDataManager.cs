using Game.CardData;
using UnityEngine;

public class AllDataManager : MonoBehaviour
{
    // ✅ 전역 접근을 위한 static 인스턴스
    //이 데이터메니져는 서버와의 연동을 위한 스크립트입니다.
    public static AllDataManager Instance { get; private set; }


    [Header("Card")]
    public AccountCardManager accountCardManager;



    private void Awake()
    {
        // 이미 존재하는 인스턴스가 있다면, 자기 자신 파괴
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 인스턴스 등록
        Instance = this;

        // 씬이 바뀌어도 파괴되지 않음
        DontDestroyOnLoad(gameObject);
    }

}
