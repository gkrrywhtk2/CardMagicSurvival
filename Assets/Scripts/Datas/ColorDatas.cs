using UnityEngine;

public enum RankType
{
    Normal,   // 일반
    Rare,     // 고급
    Epic,     // 특급
    Legendary,// 전설
    Mythic    // 신화
}

[System.Serializable]
public struct RankColorData
{
    public RankType rank;
    public Color color;
}

public class ColorDatas : MonoBehaviour
{
    public RankColorData[] rankColors = new RankColorData[]
    {
        new RankColorData { rank = RankType.Normal,    color = new Color32(0x85, 0x87, 0x86, 0xFF) }, // 일반
        new RankColorData { rank = RankType.Rare,      color = new Color32(0x65, 0xDF, 0x13, 0xFF) }, // 고급
        new RankColorData { rank = RankType.Epic,      color = new Color32(0xC8, 0x55, 0xFF, 0xFF) }, // 특급
        new RankColorData { rank = RankType.Legendary, color = new Color32(0xFF, 0xC9, 0x00, 0xFF) }, // 전설
        new RankColorData { rank = RankType.Mythic,    color = new Color32(0xFF, 0x08, 0x00, 0xFF) }  // 신화
    };

    public Color GetColor(RankType rank)
    {
        foreach (var data in rankColors)
        {
            if (data.rank == rank)
                return data.color;
        }
        return Color.white;
    }
}