using Game.RankSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SkillSlotType { C, R, E, L, M }

public class HeroSkillFrame : MonoBehaviour
{
    [Header("REF")]
    public SkillFrame_Image skillFrame_Image;
    public HeroInfo heroInfo;

    [Header("Type")]
    public SkillSlotType slotType;   // 인스펙터에서 C/R/E/L/M 지정

    [Header("Data")]
    public ServerDataManager.HeroAccount data;
    public HeroScriptableObject heroScriptableObject;

    public void Init(ServerDataManager.HeroAccount heroAccount)
    {
        data = heroAccount;
        heroScriptableObject = GameManager.instance.heroManager.heroes[data.heroId];
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (data == null || skillFrame_Image == null) return;

        int lv = GetLevelByType(data, slotType);
        int exp = ServerDataManager.instance.GetCurrentUpgradeStone();

        int maxExp = HeroManager.Instance.GetRequirementsUpgradeStone(lv); // TODO: 규칙으로 교체

        Sprite icon = GetSkillIconBySlotType();
        Color frameColor = RankDatas.GetColor(GetRankBySlotType());

        bool isLocked = IsLocked();

        skillFrame_Image.Render(
            level: lv,
            exp: exp,
            maxExp: maxExp,
            skillIcon: icon,
            frameColor: frameColor,
            isLocked: isLocked
        );
    }

    public void ShowSkillPanel()
    {
        RankType rank = GetRankBySlotType();
        heroInfo.nowTouchSkillFrame = rank;
        heroInfo.RefreshSkillPanel(rank);

    }

    private bool IsLocked()
    {
        RankType slotRank = GetRankBySlotType();
        RankType heroOpenRank = (RankType)data.rank; // heroAccount.rank가 int 라면 캐스팅
        return heroOpenRank < slotRank;
    }


    private Sprite GetSkillIconBySlotType()
    {
        if (heroScriptableObject == null || heroScriptableObject.rankUnlocks == null)
            return null;

        RankType targetRank = slotType switch
        {
            SkillSlotType.C => RankType.Uncommon,
            SkillSlotType.R => RankType.Rare,
            SkillSlotType.E => RankType.Epic,
            SkillSlotType.L => RankType.Legendary,
            SkillSlotType.M => RankType.Mythic,
            _ => RankType.Uncommon
        };

        var unlock = heroScriptableObject.rankUnlocks.Find(u => u.rank == targetRank);
        return unlock != null ? unlock.skillSpite : null;
    }

    private int GetLevelByType(ServerDataManager.HeroAccount h, SkillSlotType t) => t switch
    {
        SkillSlotType.C => h.cSkillLevel,
        SkillSlotType.R => h.rSkillLevel,
        SkillSlotType.E => h.eSkillLevel,
        SkillSlotType.L => h.lSkillLevel,
        SkillSlotType.M => h.mSkillLevel,
        _ => 1
    };

    private int GetExpByType(ServerDataManager.HeroAccount h, SkillSlotType t) => t switch
    {
        SkillSlotType.C => h.cSkillExp,
        SkillSlotType.R => h.rSkillExp,
        SkillSlotType.E => h.eSkillExp,
        SkillSlotType.L => h.lSkillExp,
        SkillSlotType.M => h.mSkillExp,
        _ => 0
    };

    private RankType GetRankBySlotType() => slotType switch
    {
        SkillSlotType.C => RankType.Uncommon,
        SkillSlotType.R => RankType.Rare,
        SkillSlotType.E => RankType.Epic,
        SkillSlotType.L => RankType.Legendary,
        SkillSlotType.M => RankType.Mythic,
        _ => RankType.Uncommon
    };
}
