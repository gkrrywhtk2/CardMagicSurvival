using System.Collections;
using UnityEngine;
using Game.RankSystem; // RankType

public class Hero0AutoAttack : HeroAutoAttackBase
{
    [Header("Attack Interval")]
    [SerializeField] private float attackInterval = 1.0f;

    [Header("Shotgun")]
    [SerializeField] private float shotgunSpreadAngle = 15f; // 좌/우 벌어지는 각도

    private Player_Main player;
    private Animator anim;
    private Scaner scaner;
    private SpriteRenderer sprite;

    private void Awake()
    {
        player = GetComponentInParent<Player_Main>();
        anim = GetComponentInParent<Animator>();
        scaner = GetComponentInParent<Scaner>();
        sprite = GetComponentInParent<SpriteRenderer>();

        if (player == null) Debug.LogError("[Hero1AutoAttack] Player_Main not found in parent.", this);
        if (scaner == null) Debug.LogError("[Hero1AutoAttack] Scaner not found in parent.", this);
    }

    protected override IEnumerator AutoAttackRoutine(RankType rank)
    {
        yield return null;

        while (true)
        {
            if (CanAutoAttack())
            {
                AutoAttack_Exe(rank);
            }

            yield return new WaitForSeconds(attackInterval);
        }
    }

    private bool CanAutoAttack()
    {
        if (player == null || scaner == null) return false;

        if (player.playerStatus.playerHP.isLive != true) return false;
        if (GameManager.instance.GamePlayState != true) return false;
        if (GameManager.instance.ArtefactSelectState == true) return false;

        if (scaner.nearestTarget == null) return false;

        return true;
    }

    private void AutoAttack_Exe(RankType rank)
    {
        if (scaner.nearestTarget == null) return;

        Vector3 spawnPos = (player.playerCenterPivot != null)
            ? player.playerCenterPivot.transform.position
            : transform.position;

        Vector3 targetPos = scaner.nearestTarget.position;
        Vector3 dir = (targetPos - spawnPos).normalized;

        float damage = GameManager.instance.player.playerStatus.DamageReturn(1, out bool isCritical);

        int bulletNumber = 0;
        int effectNumber = 1;
        float bulletSpeed = 12f;

        // ===== 랭크별 옵션 =====
        int per = 0;                         // Uncommon 기본: 1히트(현재 bullet 로직 기준)
        Vector3 scale = Vector3.one;         // 기본 크기
        bool shotgun = false;

        // Rare 이상: 관통 추가 (총 2히트 가능)
        if (rank >= RankType.Rare)
            per = -1;

        // Epic 이상: 크기 증가
        if (rank >= RankType.Epic)
            scale = new Vector3(1.5f, 1.5f, 1f);

        // Legendary 이상: 산탄
        if (rank >= RankType.Legendary)
            shotgun = true;

        // ===== 발사 =====
        if (!shotgun)
        {
            SpawnBullet(bulletNumber, effectNumber, damage, per, bulletSpeed, spawnPos, dir, scale, isCritical);
        }
        else
        {
            Vector3 dirCenter = dir;
            Vector3 dirLeft = Quaternion.Euler(0, 0, +shotgunSpreadAngle) * dir;
            Vector3 dirRight = Quaternion.Euler(0, 0, -shotgunSpreadAngle) * dir;

            SpawnBullet(bulletNumber, effectNumber, damage, per, bulletSpeed, spawnPos, dirCenter, scale, isCritical);
            SpawnBullet(bulletNumber, effectNumber, damage, per, bulletSpeed, spawnPos, dirLeft, scale, isCritical);
            SpawnBullet(bulletNumber, effectNumber, damage, per, bulletSpeed, spawnPos, dirRight, scale, isCritical);
        }
    }

    private void SpawnBullet(
        int bulletNumber,
        int effectNumber,
        float damage,
        int per,
        float bulletSpeed,
        Vector3 spawnPos,
        Vector3 dir,
        Vector3 scale,
        bool isCritical)
    {
        Transform bulletTr = GameManager.instance.effectPoolManager.Get(bulletNumber).transform;

        bulletTr.position = spawnPos;
        bulletTr.rotation = Quaternion.FromToRotation(Vector3.right, dir);
        bulletTr.localScale = scale; // ✅ 풀링 재사용 대비: 반드시 매번 세팅

        global::bullet.bulletType type = global::bullet.bulletType.bullet;
        bulletTr.GetComponent<bullet>().Init(damage, per, bulletSpeed, dir, effectNumber, type, isCritical);
    }
}
