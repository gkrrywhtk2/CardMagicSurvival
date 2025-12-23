using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Player_Status : MonoBehaviour
{
    public DataManager dataManager;

    //AccessoryManager accessoryManager;
    [Header("#플레이어의 상태값")]
    public float health;//현재 체력
    public bool gameStop;//게임이 일시정지 중이면 true 입니다.
    public bool artefactEvent;//아티팩트 이벤트 중이면 true 입니다.
    [Header("#스크립트 연동")]
    public Player_EXP playerEXP;//플레이어 경험치 스크립트 연동
    public PlayerMana playerMana;//플레이어 마나 스크립트 연동
    public PlayerCritical playerCritical; //플레이어 치명타 스크립트 연동
    public PlayerHP playerHP;//플레이어 HP 스크립트 연동
    public PlayerMoveSpeed playerMoveSpeed;//플레이어 이동속도 스크립트 연동
    private Player_col playerCol;//플레이어 충돌 스크립트 연동

     [Header("#상태 저장 ")]
     private Vector3 StonePointPos;//스톤포인트 위치 저장용
    [Header("#능력치 ")]
    
    public float totalATK;//공격력
    public int LUK;//골드 추가 획득량
    public float VIT;

    void Awake()
    {
        playerCol = GetComponent<Player_col>();
    }
  public void PlayerInit()
    {
        //게임 시작시 플레이어 변수 초기화
        playerHP.InitHealth(100);
        artefactEvent = false;
    
    }
    public void LevelUpEvent(){
        GameManager.instance.Pause();
        //GameManager.instance.spawnManager.spawnAllow = false; //소환 중지를 gameplayerstae에 종속시켰음.
        //GameManager.instance.itemManager.SpawnItems_(); 아이템 스폰 기능 짜쳐서 버림
      //  GameManager.instance.deckManager.StartUpgradeEvent();//카드 랜덤 선택 이벤트
    }

    public float DamageReturn(float skillPower, out bool isCritical)
    {
        float randomOffset = Random.Range(totalATK * -0.05f, totalATK * 0.05f);
        float basicDamage = (totalATK + randomOffset) * (skillPower / 100f);

        isCritical = playerCritical != null && playerCritical.RollCritical();

        float mult = (playerCritical != null) ? playerCritical.critMultiplier : 2f;
        return isCritical ? basicDamage * mult : basicDamage;
    }
    public void StartArtefactEvent()
    {
        GameManager.instance.ArtefactSelectState = true;
        StonePointPos = transform.position;
        playerCol.GetComponent<Collider2D>().isTrigger = true;
    }

    public void EndArtefactEvent()
    {
        GameManager.instance.ArtefactSelectState = false;
        transform.position = StonePointPos;
        playerCol.GetComponent<Collider2D>().isTrigger = false;
    }

    /**
    Get 함수 모음
    **/
    

    public int GetLUK(){
        float traningValue = 1;
        // float accValue = accessoryManager.ReturnEquipEffect_LUK() + 
        //     accessoryManager.ReturnOwnedLUKEffect();

        LUK = (int)traningValue;
        return LUK;
    }
}
