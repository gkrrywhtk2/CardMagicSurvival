using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHP : MonoBehaviour
{
    [Header("State")]
    public bool isLive = true;

    [Header("Health")]
    public float health;
    public float maxHealth = 100f;

    [Header("Regen")]
    public bool enableRegen = true;
    public float regenPerSecond = 1f; // 초당 회복량 (기존 GetVIT()가 1이었으니 기본 1)
    private Coroutine regenCoroutine;

    [Header("UI")]
    public Slider hpBar;

    private void Update()
    {
        if (!isLive) return;
        if(GameManager.instance.GamePlayState == false) return;
        if(GameManager.instance.ArtefactSelectState == true) return;
        UpdateHpBar();
    }

    public void InitHealth(float maxHp)
    {
        isLive = true;
        maxHealth = maxHp;
        health = maxHealth;

        if (enableRegen)
            StartRegen();
    }

    public void SetLive(bool live)
    {
        isLive = live;
        if (!isLive) StopRegen();
    }

    public void TakeDamage(float dmg)
    {
        if (!isLive) return;

        health = Mathf.Max(0f, health - dmg);
        if (health <= 0f)
        {
            isLive = false;
            StopRegen();
            // 필요하면 여기서 사망 처리 이벤트 호출
        }
        UpdateHpBar();
    }

    public void Heal(float amount)
    {
        if (!isLive) return;
        health = Mathf.Min(maxHealth, health + amount);
        UpdateHpBar();
    }

    public void StartRegen()
    {
        if (!enableRegen) return;
        if (regenCoroutine != null) return;
        regenCoroutine = StartCoroutine(RegenRoutine());
    }

    public void StopRegen()
    {
        if (regenCoroutine == null) return;
        StopCoroutine(regenCoroutine);
        regenCoroutine = null;
    }

    private IEnumerator RegenRoutine()
    {
        while (true)
        {
            // 게임이 진행중일 때만 회복
            yield return new WaitUntil(() => GameManager.instance.GamePlayState == true);

            Heal(regenPerSecond); // 1초마다 regenPerSecond 회복
            yield return new WaitForSeconds(1f);
        }
    }

    private void UpdateHpBar()
    {
        if (hpBar == null) return;
        if (maxHealth <= 0f) return;
        hpBar.value = health / maxHealth;
    }
}
