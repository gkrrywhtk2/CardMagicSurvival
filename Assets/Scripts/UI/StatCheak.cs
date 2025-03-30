using UnityEngine;
using TMPro;
using System.Collections;

public class StatCheak : MonoBehaviour
{
    public TMP_Text ATK;//공격력
    public TMP_Text Hp;//체력 Text 
    public TMP_Text criChance;//치명타 확율
    public TMP_Text criDamage;//치명타 데미지
    public TMP_Text VIT;//초당 체력 회복
    public TMP_Text LUK;//골드 추가 획득량
    Player_Status player_Status;
    private Coroutine Coroutine; // 실행 중인 체력 회복 코루틴
    void Awake()
    {
        player_Status = GameManager.instance.player.playerStatus;
    }
    void Start()
    {
        StartInit();
    }
    void OnEnable()
    {
        StopInit();
        StartInit();
    }

  public void TextInit(){
                //텍스트 세팅
                ATK.text = "공격력 : " + player_Status.totalATK;
                Hp.text = "체력 : " + player_Status.health + " / " + player_Status.maxHealth;
                criChance.text = "치명타 확률 : " + player_Status.totalCriChance + "%";
                criDamage.text = "치명타 데미지 : " + player_Status.totalCriDamage + "%";
                VIT.text = "초당 체력 회복 : " + player_Status.VIT;
                LUK.text = "골드 추가 획득량 : " + player_Status.LUK + "%";
                        }

        public void StartInit()
    {
        if (Coroutine == null) // 중복 실행 방지
        {
            Coroutine = StartCoroutine(InitCorutine());
        }
    }
        private IEnumerator InitCorutine()
    {
        while (true)
        {
            // 일시정지 상태일 경우, 대기 (코루틴이 종료되지 않도록 함)
            yield return new WaitUntil(() => GameManager.instance.GamePlayState == true);

            TextInit();
            yield return new WaitForSeconds(0.5f); // 0.5초 대기 후 반복
        }
    }
    public void StopInit()
    {
        if (Coroutine != null)
        {
            StopCoroutine(Coroutine);
            Coroutine = null;
        }
    }
    }
