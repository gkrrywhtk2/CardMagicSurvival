using System.Collections;
using UnityEngine;
using Game.RankSystem;

public class Hero1AutoAttack : HeroAutoAttackBase
{
    protected override IEnumerator AutoAttackRoutine(RankType rank)
    {
        yield return null;

        while (true)
        {
            int repeatCount = GetRepeatCountByRank(rank);
            float damage = GetDamageByRank(rank);
            Vector3 scale = GetScaleByRank(rank);
            int burstObjectNum = 8;

            bool enablePoison = (rank >= RankType.Rare);
            float poisonDuration = 3f;     // 원하는 값으로 조절
            float poisonDpsRate = 1.0f;    // poisonDps = finalDamage * rate

            for (int i = 0; i < repeatCount; i++)
            {
                float finalDamage = GameManager.instance.player.playerStatus
                    .DamageReturn(damage, out bool isCritical);

                GameObject burst = GameManager.instance.effectPoolManager.Get(burstObjectNum);

                Vector2 skillPosition = GameManager.instance.player.dirFront.skillPosition;
                float angle = GameManager.instance.player.dirFront.angle;

                burst.transform.position = skillPosition;
                burst.transform.rotation = Quaternion.Euler(0, 0, angle);

                // Melee
                Melee melee = burst.GetComponent<Melee>();
                if (melee != null)
                {
                    melee.Init(finalDamage, isCritical);
                    melee.ScaleSetting(scale);
                }

                // ✅ Rare+ Poison
                PoisonOnHit poison = burst.GetComponent<PoisonOnHit>();
                if (poison != null)
                {
                    poison.enabled = enablePoison;
                    if (enablePoison)
                    {
                        poison.poisonDps = finalDamage * poisonDpsRate;
                        poison.duration = poisonDuration;
                    }
                }

                yield return new WaitForSeconds(0.25f);
            }

            yield return new WaitForSeconds(5f);
        }
    }

    private int GetRepeatCountByRank(RankType rank)
    {
        switch (rank)
        {
            case RankType.Uncommon: return 4;
            case RankType.Rare: return 4;
            case RankType.Epic: return 6;
            case RankType.Legendary: return 6;
            default: return 6;
        }
    }

    private float GetDamageByRank(RankType rank)
    {
        switch (rank)
        {
            case RankType.Uncommon: return 2;
            case RankType.Rare: return 2;
            case RankType.Epic: return 2;
            case RankType.Legendary: return 2;
            default: return 5;
        }
    }

    private Vector3 GetScaleByRank(RankType rank)
    {
        switch (rank)
        {
            case RankType.Legendary: return new Vector3(2f, 2f, 1);
            default: return new Vector3(1.5f, 1.5f, 1);
        }
    }
}
