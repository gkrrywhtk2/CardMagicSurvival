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
    [SerializeField] private int baseRepeatCount = 3;         // ✅ 기본 반복 회수: 3회
    [SerializeField] private Vector3 baseScale = new(1.5f, 1.5f, 1f);

    [Header("Poison (Rare+)")]
    [SerializeField] private float poisonBaseDuration = 3f;   // ✅ 기본 3초
    [SerializeField] private int poisonBasePower = 10;        // ✅ 공격력의 10% (skillPower=10)
    [SerializeField] private int poisonPowerPerLv = 2;        // ✅ 레벨당 +2%p

    [Header("Spawn Offset")]
    [SerializeField] private float forwardOffset = 0.5f; // 캐릭터 앞쪽으로 밀 거리(유니티 월드 단위)
    [SerializeField] private Vector2 extraOffset = Vector2.zero; // 필요하면 x/y 미세조정

    private AutoAttackManager aam;

    private void Awake()
    {
        // 우선 같은 오브젝트에서 찾고, 없으면 GameManager에서 가져오기
        aam = GetComponent<AutoAttackManager>();
    
        if (aam == null)
            Debug.LogWarning("[Hero1AutoAttack] AutoAttackManager not found. Skill scaling will default to lv=0.", this);
    }

    protected override IEnumerator AutoAttackRoutine(RankType rank)
    {
        yield return null;

        while (true)
        {
            yield return new WaitForSeconds(cycleInterval);//쿨타임

            if (!CanAutoAttack())
            {
                yield return null;
                continue;
            }

            int repeatCount = GetRepeatCount(rank);
            float skillPower = GetSkillPower(rank); // DamageReturn에 넣을 값(공격력 %)
            Vector3 scale = GetScale(rank);

            bool enablePoison = (rank >= RankType.Rare);
            int poisonLv = enablePoison ? GetRareSkillLv() : 0;
            int poisonPower = enablePoison ? GetPoisonPower(poisonLv) : 0;

            for (int i = 0; i < repeatCount; i++)
            {
                // 메인 타격 데미지(크리 포함)
                float finalDamage = GameManager.instance.player.playerStatus
                    .DamageReturn(skillPower, out bool isCritical);

                GameObject burst = GameManager.instance.effectPoolManager.Get(burstObjectNum);

                Vector2 skillPosition = GameManager.instance.player.dirFront.skillPosition;
                float angle = GameManager.instance.player.dirFront.angle;

                // angle 방향(앞방향) 벡터 만들기
                Vector2 forward = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

                // ✅ 캐릭터 앞쪽으로 이동 + 추가 오프셋
                Vector2 spawnPos = skillPosition + forward * forwardOffset + extraOffset;

                burst.transform.position = spawnPos;
                burst.transform.rotation = Quaternion.Euler(0, 0, angle);

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
                        // ✅ 독은 크리 적용 안 되게 "공격력 기반"으로 계산(권장)
                        // totalATK의 (poisonPower/100) * (±5% 랜덤은 빼고 싶으면 아래 식으로)
                        float atk = GameManager.instance.player.playerStatus.totalATK;
                        float poisonDps = atk * (poisonPower / 100f);

                        poison.poisonDps = poisonDps;
                        poison.duration = poisonBaseDuration;
                    }
                }

                yield return new WaitForSeconds(betweenHitDelay);
            }

            //yield return new WaitForSeconds(cycleInterval);
        }
    }

    private bool CanAutoAttack()
    {
        var player = GameManager.instance.player;
        if (player == null) return false;

        if (player.playerStatus.playerHP.isLive != true) return false;
        if (GameManager.instance.GamePlayState != true) return false;
        if (GameManager.instance.ArtefactSelectState == true) return false;

        // 방향/위치 참조에 필요한 값들이 있는지
        if (player.dirFront == null) return false;

        return true;
    }

    // =========================
    // Rank Rules
    // =========================

    // ✅ 전설: 반복 횟수 증가 (L레벨만큼)
    private int GetRepeatCount(RankType rank)
    {
        int count = baseRepeatCount;

        if (rank >= RankType.Legendary)
            count += GetLegendarySkillLv(); // L레벨 1이면 +1, 2이면 +2...

        return count;
    }

    // ✅ 일반: 데미지 증가 (C레벨 * 10%p)
    // - cLv=0이면 100%
    // - cLv=1이면 110%
    private float GetSkillPower(RankType rank)
    {
        // 여기선 "기본 공격(일반) 강화"로만 사용. 랭크 제한을 걸고 싶으면 if(rank>=Uncommon) 같은 조건 추가.
        int cLv = GetCommonSkillLv();
        return 100f + (cLv * 10f);
    }

    // ✅ 에픽: 크기 증가 (E레벨 1=>+10%, 2=>+20%…)
    private Vector3 GetScale(RankType rank)
    {
        if (rank < RankType.Epic)
            return baseScale;

        int eLv = GetEpicSkillLv();
        float mul = 1f + (eLv * 0.10f);

        return new Vector3(baseScale.x * mul, baseScale.y * mul, baseScale.z);
    }

    // ✅ Rare 독 파워: 10% + 레벨당 2%p (lv=1=>10, lv=2=>12 ...)
    private int GetPoisonPower(int lv)
    {
        lv = Mathf.Max(1, lv);
        return poisonBasePower + (lv - 1) * poisonPowerPerLv;
    }

    // =========================
    // Skill Lv Getters (Null-safe)
    // =========================
    private int GetCommonSkillLv()    => aam != null ? Mathf.Max(0, aam.CskillLv) : 0;
    private int GetRareSkillLv()      => aam != null ? Mathf.Max(1, aam.RskillLv) : 1;
    private int GetEpicSkillLv()      => aam != null ? Mathf.Max(0, aam.EskillLv) : 0;
    private int GetLegendarySkillLv() => aam != null ? Mathf.Max(0, aam.LskillLv) : 0;
}
