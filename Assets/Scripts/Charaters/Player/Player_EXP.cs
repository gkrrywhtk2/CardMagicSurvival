using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player_EXP : MonoBehaviour
{
    [Header("Level Settings")]
    public int level = 1;           // 현재 레벨
    public int maxLevel = 20;       // 최대 레벨
    
    [Header("Experience Settings")]
    public float currentExp = 0;    // 현재 경험치
    public float maxExp = 100;      // 다음 레벨업에 필요한 경험치 (초기값)
    public float expGrowthFactor = 1.2f; // 레벨업 시 필요 경험치 증가율 (20%씩 증가)

    [Header("슬라이더 연동")]
    public Slider expSlider;
    public TMP_Text currentLevelText;

    void Start()
    {
        // 초기화 로직이 필요하다면 여기에 작성
        CalculateNextMaxExp();
    }

    // 경험치를 획득하는 함수
    public void AddExp(float amount)
    {
        // 이미 만렙이라면 경험치를 얻지 않음
        if (level >= maxLevel) return;

        currentExp += amount;

        //상승된 경험치 UI에 반영
        SliderUpdate();

        // 획득한 경험치가 필요 경험치보다 많으면 레벨업 (while문은 한 번에 여러 레벨업을 처리하기 위함)
        while (currentExp >= maxExp && level < maxLevel)
        {
            currentExp -= maxExp; // 남은 경험치는 다음 레벨로 이월
            LevelUp();
        }
        
        // 만렙 도달 시 경험치 바 처리를 위한 예외 처리 (선택사항)
        if (level >= maxLevel)
        {
            currentExp = maxExp;
        }
    }

    // 레벨업 처리 함수
    void LevelUp()
    {
        level++;
        
        // 필요 경험치량 증가 계산
        CalculateNextMaxExp();

        // 뱀서라이크의 핵심: 카드 선택(스킬 획득) 이벤트 트리거
        ShowCardSelectEvent();

    }

    // 다음 레벨업에 필요한 경험치를 계산하는 함수 (곡선형 성장)
    void CalculateNextMaxExp()
    {
        // 단순 증가 방식 예시: 현재 필요 경험치 * 1.2
        // 기획에 따라 공식을 변경하세요. (예: level * 100 등)
        maxExp = maxExp * expGrowthFactor;
        SliderUpdate();//초기화
    }

    // 카드 선택 이벤트 (현재는 로그만 출력)
    void ShowCardSelectEvent()
    {
        Debug.Log($"[Level Up!] 현재 레벨: {level} >> 카드 선택 창이 열립니다.");
        // 나중에 여기에 Time.timeScale = 0 (게임 일시정지) 및 UI 팝업 로직을 추가하면 됩니다.
    }

    void SliderUpdate()
    {
        //슬라이드 업데이트 함수
        expSlider.value = currentExp / maxExp;
        currentLevelText.text = level.ToString();
    }
}