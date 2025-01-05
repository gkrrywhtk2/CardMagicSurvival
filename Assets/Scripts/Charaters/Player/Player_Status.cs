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
    [Header("Bar")]
    public Slider attackBar;
    public Slider hpBar;
    public Slider manaBar;
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
    }

    public void ManaRecovery(){
        mana += (manaRecovery+ manaRecoveryPlus) * Time.deltaTime;
    }
}
