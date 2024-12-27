using UnityEngine;
using UnityEngine.UI;

public class Player_Status : MonoBehaviour
{
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool isLive;
    public float health;
    public float maxHealth = 100;
    [Header("Bar")]
    public Slider attackBar;
    public Slider hpBar;

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
    }
}
