using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player_Status : MonoBehaviour
{
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool isLive;
    public float health;
    public float maxHealth = 100;
    public float mana;
    public float maxMana = 10;
    public float manaRecovery;
    public float manaRecoveryPlus;
    public float maxexp;
    public float nowexp;
    public int playLevel;
    [Header("Bar")]
    public Slider attackBar;
    public Slider hpBar;
    public Slider manaBar;
    public Slider expBar;
    public TMP_Text manaText;

    private void Awake() {
        isLive = true;
        health = maxHealth;
      
    }

    private void HpBarUpdate()
    {
        hpBar.value = health / maxHealth;

        if (health < maxHealth)
            return;

        if (health > maxHealth)
        {
            health = maxHealth;
            return;
        }

    }

     private void ExpBarUpdate()
    {
        expBar.value = nowexp / maxexp;

        if(nowexp >= maxexp){
            nowexp = 0;
            playLevel += 1;
            LevelUpEvent();
        }

    }
    public void LevelUpEvent(){
        GameManager.instance.GamePlayState = false; //일시 정지 
        //GameManager.instance.spawnManager.spawnAllow = false; //소환 중지를 gameplayerstae에 종속시켰음.
        //GameManager.instance.itemManager.SpawnItems_(); 아이템 스폰 기능 짜쳐서 버림
        GameManager.instance.deckManager.StartUpgradeEvent();//카드 랜덤 선택 이벤트

    }
    private void AttackBarUpdate(){
        float automaxpoint = GameManager.instance.player.autoAttack.autoAttackMaxPoint;
        float autocurpoint = GameManager.instance.player.autoAttack.autoAttackCurrentPoint;
        attackBar.value = autocurpoint/automaxpoint;

    }

    private void ManaBarUpdate()
    {
        int currentMana = Mathf.FloorToInt(mana); // 정수로 변환
        manaText.text = currentMana.ToString();
        manaBar.value = mana / maxMana;

        if (mana < maxMana)
            return;

        if (mana > maxMana)
        {
            mana = maxMana;
            return;
        }

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isLive != true)
            return;
        HpBarUpdate();
        AttackBarUpdate();
        ManaBarUpdate();
        ManaRecovery();
        ExpBarUpdate();
    }

    public void ManaRecovery(){
        if(GameManager.instance.GamePlayState != true)
        return;
        if(GameManager.instance.levelUpState == true)
        return;

        mana += (manaRecovery+ manaRecoveryPlus) * Time.deltaTime;
    }
}
