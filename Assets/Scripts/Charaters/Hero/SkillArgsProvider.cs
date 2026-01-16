using System.Globalization;
using Game.RankSystem;
using UnityEngine.Localization.Settings;
using UnityEngine;

public static class SkillArgsProvider
{
    private const string ORANGE = "#FFA500";

    public static object[] GetArgs(int heroId, RankType rank, ServerDataManager.HeroAccount acc)
    {
        return heroId switch
        {
            0 => Hero0Args(rank, acc),
            1 => Hero1Args(rank, acc),
            _ => System.Array.Empty<object>()
        };
        
    }

    // ✅ {0}에 들어갈 값만 주황색
    private static string Orange(object v)
        => $"<color={ORANGE}>{ToCleanString(v)}</color>";

    // ✅ float/int 어떤 타입이 와도 보기 좋게
    private static string ToCleanString(object v)
    {
        if (v == null) return "0";

        return v switch
        {
            float f  => f.ToString("0.#", CultureInfo.InvariantCulture),
            double d => d.ToString("0.#", CultureInfo.InvariantCulture),
            int i    => i.ToString(CultureInfo.InvariantCulture),
            long l   => l.ToString(CultureInfo.InvariantCulture),
            _        => v.ToString()
        };
    }

    private static Hero0AutoAttack GetHero0ValueData()
    {
        return GameManager.instance?.heroManager?.autoAttackManager?.heroAutoAttacks?[0] as Hero0AutoAttack;
    }

    private static Hero1AutoAttack GetHero1ValueData()
    {
        return GameManager.instance?.heroManager?.autoAttackManager?.heroAutoAttacks?[1] as Hero1AutoAttack;
    }

    private static object[] Hero0Args(RankType rank, ServerDataManager.HeroAccount acc)
    {
        var valueData = GetHero0ValueData();
        if (valueData == null || acc == null) return System.Array.Empty<object>();

        int cLv = acc.cSkillLevel;
        int rLv = acc.rSkillLevel;
        int eLv = acc.eSkillLevel;
        int lLv = acc.lSkillLevel;

        return rank switch
        {
            RankType.Uncommon  => new object[] { Orange(valueData.GetCSkillValue(cLv)) },
            RankType.Rare      => new object[] { Orange(valueData.GetRSkillValue(rLv)) },
            RankType.Epic      => new object[] { Orange(valueData.GetESkillValue(eLv)) },
            RankType.Legendary => new object[] { Orange(valueData.GetLSkillValue(lLv)) },
            _ => System.Array.Empty<object>()
        };
    }

    private static object[] Hero1Args(RankType rank, ServerDataManager.HeroAccount acc)
    {
        var valueData = GetHero1ValueData();
        if (valueData == null || acc == null) return System.Array.Empty<object>();

        int cLv = acc.cSkillLevel;
        int rLv = acc.rSkillLevel;
        int eLv = acc.eSkillLevel;
        int lLv = acc.lSkillLevel;


        return rank switch
        {
            RankType.Uncommon  => new object[] { Orange(valueData.GetSkillPower(cLv)) },
            RankType.Rare      => new object[] { Orange(valueData.GetPoisonPower(rLv)) },
            RankType.Epic      => new object[] { Orange(valueData.GetEskillValue(eLv)) },
            RankType.Legendary => new object[] { Orange(valueData.GetRepeatCount(lLv)) },
            _ => System.Array.Empty<object>()
        };
    }

}
