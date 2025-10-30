using UnityEngine;
using System.Collections.Generic;


namespace Game.RankSystem
{

    public enum RankType
    {
        Uncommon,   // 일반
        Rare,       // 고급
        Epic,       // 특급
        Legendary,  // 전설
        Mythic      // 신화
    }

    [System.Serializable]
    public struct RankData
    {
        public RankType rank;
        public Color color;
    }

    public class RankDatas : MonoBehaviour
    {

        private static readonly Dictionary<RankType, Color> colorMap = new()
        {
            { RankType.Uncommon,  new Color32(0x85,0x87,0x86,0xFF) },
            { RankType.Rare,      new Color32(0x65,0xDF,0x13,0xFF) },
            { RankType.Epic,      new Color32(0xC8,0x55,0xFF,0xFF) },
            { RankType.Legendary, new Color32(0xFF,0xC9,0x00,0xFF) },
            { RankType.Mythic,    new Color32(0xFF,0x08,0x00,0xFF) }
        };

        public static Color GetColor(RankType rank)
        {
            return colorMap.TryGetValue(rank, out var color) ? color : Color.white;
        }
    }
}
