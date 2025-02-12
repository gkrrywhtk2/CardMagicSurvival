using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    // 스테이지 정보
    public int currentStageLevel;   // 현재 플레이어가 도전 중인 스테이지 레벨
    public float stageProgress;     // 스테이지 진행률 (100%일 경우 보스 도전 가능)
    private readonly object singleThread = new object();//스테이지 진행률 접근 싱글 스레드 방식으로 처리

    // UI 요소
    public Slider stageProgressSlider; // 스테이지 진행률 슬라이더
    public TMP_Text stageProgressText; // 스테이지 진행률 텍스트
    public Slider bossHpSlider;        // 보스 체력 슬라이더
    public Slider bossTimeSlider;      // 보스 제한 시간 슬라이더
    public GameObject bossButton;       //보스 도전 버튼
    public TMP_Text stageNameText;     //스테이지 레벨텍스트
    public GameObject nextStageButton;//다음 스테이지 진행 버튼


    // 보스전 관련 변수
    public float bossTime = 60f; // 보스 제한 시간 (기본값 1분)
    public Animator cameraAnim;
    public Monster bossObject;//보스 오브젝트 링크
    public bool isBossReady;        // 보스 도전 가능 여부
    public bool nowBossBattle;  //보스 도전 중
    public TMP_Text bossHpText;//보스체력 텍스트

    

    /// <summary>
    /// 스테이지 시작 설정
    /// </summary>
    public void StartStage(int stageLevel)
    {
        currentStageLevel = stageLevel;
        stageProgress = 0f;
        isBossReady = false;
        nowBossBattle = false;
        stageNameText.text = "스테이지 " + currentStageLevel;
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
        nowBossBattle = true;//보스 배틀 시작
        stageNameText.text = "";
        bossButton.gameObject.SetActive(false);
        cameraAnim.SetBool("Boss", true);
        GameManager.instance.spawnManager.MonsterSpawn(2);
        bossHpSlider.gameObject.SetActive(true);

        //보스 슬라이드 세팅
        bossTime = 60;//보스 제한시간 1분
    }
    private void FixedUpdate() {
        BossHpUpdate();
    }

    public void BossHpUpdate()
{
    if (!nowBossBattle) // 보스가 나오지 않았다면 리턴
        return;

    float hp = bossObject.health;
    float maxHealth = bossObject.maxHealth;
    bossHpSlider.value = hp / maxHealth;

    // HP를 소수점 첫째 자리까지 표시
    bossHpText.text = hp.ToString("F1") +  "%";

    if (!GameManager.instance.GamePlayState) // 게임이 일시정지라면 시간 감소X
        return;

    bossTime -= Time.deltaTime;
    bossTimeSlider.value = bossTime / 60;
        if(bossTime < 0){
            nowBossBattle = false;//보스 배틀 종료
            GameManager.instance.player.playerCol.PlayerDeathSetting();
            return;
        }
}

public void BossDeadEvent(){
    nowBossBattle = false;//보스배틀 종료
    bossHpSlider.gameObject.SetActive(false);//보스 슬라이드 감추기
    nextStageButton.gameObject.SetActive(true);//다음 스테이지 진행 버튼 활성화
}

    public void NextStageButton(){
        GameManager.instance.player.playerEffect.NextStagePadeOut.gameObject.SetActive(true); //화염 어두워짐 효과
        currentStageLevel++;//스테이지 레벨업
        StartStage(currentStageLevel);//스테이지 세팅
        StartCoroutine(NextStageStepAnim());//스테이지 스텝 애니 연출
        nextStageButton.gameObject.SetActive(false);
    }

    IEnumerator NextStageStepAnim(){
        yield return new WaitForSeconds(0.5f);
        GameManager.instance.player.playerEffect.NextStagePadeOut.SetTrigger("FadeIn");//다시 밝아지는 연출
        JoyStick_P joy =  GameManager.instance.player.joystickP;
        joy.nextStageSetting = true;
       // GameManager.instance.pixelPerfectCamera.assetsPPU = 25;
        joy.inputVec.x = 1;
        joy.inputVec.y = 0;
        yield return new WaitForSeconds(2);
        joy.inputVec.x = 0;
        joy.inputVec.y = 0;
        joy.nextStageSetting = false;
    }

   

}

