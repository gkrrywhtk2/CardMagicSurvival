using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    // 스테이지 정보
    public int currentStageLevel;   // 현재 플레이어가 도전 중인 스테이지 레벨
    public float stageProgress;     // 스테이지 진행률 (100%일 경우 보스 도전 가능)
    public bool isBossReady;        // 보스 도전 가능 여부
    private readonly object singleThread = new object();//스테이지 진행률 접근 싱글 스레드 방식으로 처리

    // UI 요소
    public Slider stageProgressSlider; // 스테이지 진행률 슬라이더
    public TMP_Text stageProgressText; // 스테이지 진행률 텍스트
    public Slider bossHpSlider;        // 보스 체력 슬라이더
    public Slider bossTimeSlider;      // 보스 제한 시간 슬라이더
    public GameObject bossButton;       //보스 도전 버튼

    // 보스전 관련 변수
    public float bossTime = 60f; // 보스 제한 시간 (기본값 1분)
    public Animator cameraAnim;

    private void Update() 
    {

    }

    /// <summary>
    /// 스테이지 시작 설정
    /// </summary>
    public void StartStage(int stageLevel)
    {
        currentStageLevel = stageLevel;
        stageProgress = 0f;
        isBossReady = false;
        UpdateUI();
    }

    /// <summary>
    /// 스테이지 진행률 체크 및 UI 업데이트
    /// </summary>
    public void CheckStageProgress()
{
    //몬스터가 사망시 해당 함수에 접근하여 스테이지 진행률 증가
    lock (singleThread) // 동시 접근 방지
    {
        if (isBossReady) return; // 이미 보스 도전 가능하면 추가 증가 방지

        stageProgress = Mathf.Min(stageProgress + 2, 100f); // 100% 초과 방지
        stageProgressSlider.value = stageProgress / 100f;
        stageProgressText.text = stageProgress + "%";

        if (stageProgress >= 100f)
        {
            isBossReady = true;
            BossButtonSetting();
        }
    }
}
    /// <summary>
    /// UI 업데이트 (필요시 호출)
    /// </summary>
    private void UpdateUI()
    {
        CheckStageProgress();
    }
    public void BossButtonSetting(){
        bossButton.gameObject.SetActive(true);
    }

    public void BossButton(){
        bossButton.gameObject.SetActive(false);
        cameraAnim.SetBool("Boss", true);
        GameManager.instance.spawnManager.MonsterSpawn(2);

    }
   
}

