using UnityEngine;
using System.Collections.Generic;
using System;


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
         { RankType.Uncommon,  new Color32(0x8A, 0x8D, 0x93, 0xFF) },
        { RankType.Rare,      new Color32(0x3D, 0x75, 0xB3, 0xFF) },
        { RankType.Epic,      new Color32(0xB3, 0x63, 0xDF, 0xFF) },
        { RankType.Legendary, new Color32(0xEB, 0xB6, 0x2D, 0xFF) },
        { RankType.Mythic,    new Color32(0xD2, 0x41, 0x69, 0xFF) }
    };

        public static Color GetColor(RankType rank)
        {
            return colorMap.TryGetValue(rank, out var color) ? color : Color.white;
        }

        public static string GetRankString(RankType rank)
        {
            switch (rank)
            {
                case RankType.Uncommon:
                    return "언커먼";
                
                case RankType.Rare:
                    return "레어";
                
                case RankType.Epic:
                    return "에픽";
                
                case RankType.Legendary:
                    return "레전더리";
                
                case RankType.Mythic:
                    return "미식";
                
                default:
                    return "알 수 없음";
            }
        }
    }
}
