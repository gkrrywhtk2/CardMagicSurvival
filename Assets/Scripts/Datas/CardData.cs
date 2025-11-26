using UnityEngine;

namespace Game.CardData
{
    public class CardData : MonoBehaviour
    {
        // ✅ Singleton 패턴
        public static CardData Instance { get; private set; }
        
        public CardScritableData[] cardScritableData; // 카드 스크립터블 오브젝트
        
        private void Awake()
        {
            // ✅ Singleton 초기화
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // (선택사항) 씬 전환 시에도 유지
            }
            else
            {
                Debug.LogWarning("[CardData] 중복된 인스턴스가 감지되어 파괴합니다.");
                Destroy(gameObject);
            }
        }
    }
}