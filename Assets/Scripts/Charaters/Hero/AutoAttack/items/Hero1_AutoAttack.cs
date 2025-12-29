using System.Collections;
using UnityEngine;
using Game.RankSystem;

public class Hero1AutoAttack : HeroAutoAttackBase
{
    [Header("Cycle")]
    [SerializeField] private float bombardInterval = 0.2f; // 펑펑 간격

    [Header("Pool / Visual")]
    [SerializeField] private int poisonBurstPoolId = 8;       // effectPoolManager.Get(8)
    [SerializeField] private Vector3 baseScale = Vector3.one; // Uncommon 기준 크기

    [Header("Poison (Rare+)")]
    [SerializeField] private float poisonDpsRate = 1.0f;
    [SerializeField] private float poisonDuration = 3f;

    [Header("Damage")]
    [SerializeField] private float damageBase = 1f; // DamageReturn에 넣는 베이스 값

    private Player_Main player;
    private Coroutine bombardCo;

    private void Awake()
    {
        player = GetComponentInParent<Player_Main>();
        if (player == null) Debug.LogError("[Hero1AutoAttack] Player_Main not found in parent.", this);
    }

    protected override IEnumerator AutoAttackRoutine(RankType rank)
    {
        yield return null;

        while (true)
        {
            if (CanAutoAttack())
            {
                // 이미 돌고 있으면 끊고 새로(카드 Use 방식과 동일)
                if (bombardCo != null) StopCoroutine(bombardCo);
                bombardCo = StartCoroutine(BombardRoutine(rank));
            }

            // ✅ “횟수 4면 4초마다 / 6이면 6초마다” 감각
            yield return new WaitForSeconds(GetRepeatCount(rank));
        }
    }

    private bool CanAutoAttack()
    {
        if (player == null) return false;

        if (player.playerStatus.playerHP.isLive != true) return false;
        if (GameManager.instance.GamePlayState != true) return false;
        if (GameManager.instance.ArtefactSelectState == true) return false;

        // ✅ 적 없어도 발동 가능
        return true;
    }

    private IEnumerator BombardRoutine(RankType rank)
    {
        int repeatCount = GetRepeatCount(rank);
        Vector3 scale = GetScale(rank);

        for (int i = 0; i < repeatCount; i++)
        {
            if (!CanAutoAttack()) break;

            // ✅ FlameBurstRoutine처럼 "매 스폰마다" 정면 위치/각도 갱신
            Vector2 skillPosition = GameManager.instance.player.dirFront.skillPosition;
            float angle = GameManager.instance.player.dirFront.angle;

            SpawnPoisonBurst(rank, skillPosition, angle, scale);

            yield return new WaitForSeconds(bombardInterval);
        }

        bombardCo = null;
    }

    private void SpawnPoisonBurst(RankType rank, Vector2 pos, float angle, Vector3 scale)
    {
        float dmg = GameManager.instance.player.playerStatus.DamageReturn(damageBase, out bool isCritical);

        GameObject burstRoot = GameManager.instance.effectPoolManager.Get(poisonBurstPoolId);

        burstRoot.transform.position = pos;
        burstRoot.transform.rotation = Quaternion.Euler(0, 0, angle);
        burstRoot.transform.localScale = scale;

        // ✅ 자식 데미지박스에 Melee가 있는 구조
        Melee melee = burstRoot.GetComponentInChildren<Melee>(true);
        if (melee != null)
        {
            melee.Init(dmg, isCritical);
            melee.ScaleSetting(scale); // FlameBurst처럼 Melee가 스케일을 직접 먹는 구조면 유지
        }

        // ✅ Rare+ 독 부여
        PoisonOnHit poison = burstRoot.GetComponentInChildren<PoisonOnHit>(true);
        if (poison != null)
        {
            bool enablePoison = (rank >= RankType.Rare);
            poison.enabled = enablePoison;

            if (enablePoison)
            {
                poison.poisonDps = dmg * poisonDpsRate;
                poison.duration = poisonDuration;
            }
        }
    }

    // ===== Rank Effects =====
    private int GetRepeatCount(RankType rank)
    {
        // Uncommon/ Rare = 4회
        // Epic+ = 6회
        return rank switch
        {
            RankType.Uncommon => 4,
            RankType.Rare => 4,
            RankType.Epic => 6,
            RankType.Legendary => 6,
            RankType.Mythic => 6,
            _ => 4
        };
    }

    private Vector3 GetScale(RankType rank)
    {
        // Legendary+ = 1.5배
        float mul = (rank >= RankType.Legendary) ? 1.5f : 1f;
        return new Vector3(baseScale.x * mul, baseScale.y * mul, baseScale.z);
    }
}
