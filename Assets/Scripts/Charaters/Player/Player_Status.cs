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
        GameManager.instance.levelUpState = true;//바꿔야함 
        GameManager.instance.spawnManager.spawnAllow = false;
        GameManager.instance.player.GetComponent<Collider2D>().isTrigger = true;
        GameManager.instance.player.playerEffect.levelUpCircleTimeStop.gameObject.SetActive(true);
        GameManager.instance.player.playerEffect.levelUpCircleTimeStop.transform.position = GameManager.instance.player.dirFront.playerTransform.position;
        GameManager.instance.nextWaveButton.gameObject.SetActive(true);
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
