using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Game.RankSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings; // 추가

public class RankLabel : MonoBehaviour
{
    public Image background;
    public Image inner;
    public TMP_Text text;
    public LocalizedString localizedRanklabel;

    public void SetRank(RankType rank)
    {
        var (_, frameColor, innerColor) = GetRankColors(rank);

        background.color = frameColor;
        inner.color = innerColor;

        // 랭크 텍스트 반영
        text.text = GetRankLabel(rank);
    }

    private (string name, Color32 frame, Color32 inner) GetRankColors(RankType rank)
    {
        return rank switch
        {
            RankType.Uncommon => ("Uncommon", new Color32(0x8A, 0x8D, 0x93, 0xFF), new Color32(0xA5, 0xA7, 0xAB, 0xFF)),
            RankType.Rare => ("Rare", new Color32(0x50, 0x9A, 0xFF, 0xFF), new Color32(0x85, 0xBC, 0xFF, 0xFF)),
            RankType.Epic => ("Epic", new Color32(0xB3, 0x63, 0xDF, 0xFF), new Color32(0xCC, 0x96, 0xEA, 0xFF)),
            RankType.Legendary => ("Legendary", new Color32(0xEB, 0xB6, 0x2D, 0xFF), new Color32(0xF2, 0xCE, 0x72, 0xFF)),
            RankType.Mythic => ("Mythic", new Color32(0xFF, 0x5C, 0x7C, 0xFF), new Color32(0xE1, 0x80, 0x9A, 0xFF)),
            _ => ("Unknown", Color.white, Color.white)
        };
    }

    private string GetRankKey(RankType rank)
    {
        return rank switch
        {
            RankType.Uncommon => "Rank.Uncommon",
            RankType.Rare => "Rank.Rare",
            RankType.Epic => "Rank.Epic",
            RankType.Legendary => "Rank.Legendary",
            RankType.Mythic => "Rank.Mythic",
            _ => "Rank.Unknown"
        };
    }

    public string GetRankLabel(RankType rank)
    {
        if (localizedRanklabel == null) return "";

        localizedRanklabel.TableEntryReference = GetRankKey(rank);
        return localizedRanklabel.GetLocalizedString();
    }
}
