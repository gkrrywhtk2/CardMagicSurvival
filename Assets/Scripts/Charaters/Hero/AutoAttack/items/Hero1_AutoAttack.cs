using System.Collections;
using UnityEngine;
using Game.RankSystem;

public class Hero1AutoAttack : HeroAutoAttackBase
{
    [Header("Timings")]
    [SerializeField] private float cycleInterval = 5f;        // 한 사이클(연타 묶음) 간격
    [SerializeField] private float betweenHitDelay = 0.25f;   // 연타 사이 딜레이

    [Header("Pool")]
    [SerializeField] private int burstObjectNum = 8;

    [Header("Base")]
    [SerializeField] private int baseRepeatCount = 3;         // 기본 반복 회수
    [SerializeField] private Vector3 baseScale = new(1.5f, 1.5f, 1f);

    [Header("Poison (Rare+)")]
    [SerializeField] private float poisonBaseDuration = 3f;   // 기본 3초
    [SerializeField] private int poisonBasePower = 10;        // 공격력의 10% (skillPower=10)
    [SerializeField] private int poisonPowerPerLv = 2;        // 레벨당 +2%p

    [Header("Spawn Offset")]
    [SerializeField] private float forwardOffset = 0.5f;      // 캐릭터 앞쪽으로 밀 거리
    [SerializeField] private Vector2 extraOffset = Vector2.zero;

    private AutoAttackManager aam;

    private void Awake()
    {
        aam = GetComponent<AutoAttackManager>();
        if (aam == null)
            Debug.LogWarning("[Hero1AutoAttack] AutoAttackManager not found. Skill scaling will default to lv=0.", this);
    }

    protected override IEnumerator AutoAttackRoutine(RankType rank)
    {
        // (선택) 한 프레임 뒤 시작
        yield return null;

        while (true)
        {
            // ✅ 쿨타임은 항상 유지
            yield return new WaitForSeconds(cycleInterval);

            // ✅ 공격 불가면 짧게만 쉬고 다음 사이클
            if (!CanAutoAttack())
            {
                // 너무 빡빡하게 돌지 않게(상황에 따라 0.1~0.3 추천)
                yield return new WaitForSeconds(0.2f);
                continue;
            }

            // ===== 캐싱(가독성 + 성능) =====
            var gm = GameManager.instance;
            var player = gm.player;
            var status = player.playerStatus;

            int cLv = (aam != null) ? aam.CskillLv : 0;
            int rLv = (aam != null) ? aam.RskillLv : 0;
            int eLv = (aam != null) ? aam.EskillLv : 0;
            int lLv = (aam != null) ? aam.LskillLv : 0;

            // ===== 반복 횟수 =====
            int repeatCount = (rank < RankType.Legendary)
                ? baseRepeatCount
                : baseRepeatCount + GetRepeatCount(lLv);

            // ===== 메인 데미지 % (공격력 기반) =====
            float skillPower = GetSkillPower(cLv);

            // ===== 스케일 =====
            Vector3 scale = baseScale;
            if (rank >= RankType.Epic)
            {
                int plusScalePercent = GetEskillValue(eLv);   // 10, 20, 30...
                scale = GetScale(plusScalePercent);
            }

            // ===== 독 =====
            bool enablePoison = (rank >= RankType.Rare);
            int poisonPower = enablePoison ? GetPoisonPower(rLv) : 0;

            // ===== 연타 =====
            for (int i = 0; i < repeatCount; i++)
            {
                // (중간에 죽거나 상태 바뀌면 즉시 중단)
                if (!CanAutoAttack()) break;

                float finalDamage = status.DamageReturn(skillPower, out bool isCritical);

                GameObject burst = gm.effectPoolManager.Get(burstObjectNum);
                if (burst == null)
                {
                    Debug.LogWarning("[Hero1AutoAttack] EffectPool returned null.", this);
                    yield return new WaitForSeconds(betweenHitDelay);
                    continue;
                }

                Vector2 skillPosition = player.dirFront.skillPosition;
                float angle = player.dirFront.angle;

                // angle 방향(앞방향) 벡터
                Vector2 forward = new Vector2(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    Mathf.Sin(angle * Mathf.Deg2Rad)
                );

                Vector2 spawnPos = skillPosition + forward * forwardOffset + extraOffset;

                burst.transform.SetPositionAndRotation(
                    spawnPos,
                    Quaternion.Euler(0, 0, angle)
                );

                // Melee
                if (burst.TryGetComponent<Melee>(out var melee))
                {
                    melee.Init(finalDamage, isCritical);
                    melee.ScaleSetting(scale);
                }

                // Poison (Rare+)
                if (burst.TryGetComponent<PoisonOnHit>(out var poison))
                {
                    poison.enabled = enablePoison;

                    if (enablePoison)
                    {
                        // ✅ 독은 크리 제외, 공격력 기반 DPS
                        float atk = status.totalATK;
                        poison.poisonDps = atk * (poisonPower / 100f);
                        poison.duration = poisonBaseDuration;
                    }
                }

                yield return new WaitForSeconds(betweenHitDelay);
            }
        }
    }

    private bool CanAutoAttack()
    {
        var gm = GameManager.instance;
        if (gm == null) return false;

        var player = gm.player;
        if (player == null) return false;

        if (player.playerStatus?.playerHP == null) return false;
        if (!player.playerStatus.playerHP.isLive) return false;
        if (!gm.GamePlayState) return false;
        if (gm.ArtefactSelectState) return false;
        if (player.dirFront == null) return false;

        return true;
    }

    // =========================
    // Rank Rules
    // =========================

    // 전설: 반복 횟수 증가 (L레벨만큼)
    public int GetRepeatCount(int level)
    {
        return level; // L레벨 1이면 +1, 2이면 +2...
    }

    // 에픽: 크기 증가량(%)
    public int GetEskillValue(int level)
    {
        level = Mathf.Max(0, level);
        return level * 10; // 10, 20, 30...
    }

    // 일반: 데미지 증가 (C레벨 * 10%p)
    public float GetSkillPower(int level)
    {
        level = Mathf.Max(0, level);
        return 100f + (level * 10f);
    }

    // 에픽: 크기 증가 적용
    public Vector3 GetScale(int valuePercent)
    {
        float mul = 1f + (valuePercent / 100f);
        return new Vector3(baseScale.x * mul, baseScale.y * mul, baseScale.z);
    }

    // Rare 독 파워: 10% + (레벨-1)*2%p
    // - rLv=0이면 "독 없음"으로 처리하고 싶으면 여기서 0 리턴하게 바꾸는 게 더 자연스러움
    public int GetPoisonPower(int lv)
    {
        lv = Mathf.Max(1, lv);
        return poisonBasePower + (lv - 1) * poisonPowerPerLv;
    }
}
