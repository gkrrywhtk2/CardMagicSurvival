using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using Game.RankSystem;

public class RankLocalization : MonoBehaviour
{
    [SerializeField] private LocalizeStringEvent localizeEvent;

    // ✅ 너 프로젝트 String Table 이름으로 변경
    private const string Table = "UI_Rank";
    private const string RankKeyPrefix = "Rank.";

    private void Reset()
    {
        if (localizeEvent == null)
            localizeEvent = GetComponentInChildren<LocalizeStringEvent>();
    }

    /// <summary>
    /// RankType을 받아서 "Rank.Uncommon" 같은 키로 로컬라이즈 표시
    /// </summary>
    public void BindRank(RankType rank)
    {
        if (localizeEvent == null)
        {
            Debug.LogWarning($"{nameof(RankLabelUI)}: LocalizeStringEvent가 비어있습니다.");
            return;
        }

        string key = GetRankKey(rank);
        var ls = new LocalizedString(Table, key);

        localizeEvent.StringReference = ls;
        localizeEvent.RefreshString();
    }
    // ✅ 추가: RankType -> 현재 로케일의 등급명 문자열 반환
    public static async Task<string> GetRankNameAsync(RankType rank)
    {
        string key = $"{RankKeyPrefix}{rank}";
        string text = await LocalizationSettings.StringDatabase
            .GetLocalizedStringAsync(Table, key).Task;

        // 번역 누락 대비(키 그대로 나오면 enum으로)
        if (string.IsNullOrEmpty(text) || text == key)
            text = rank.ToString();

        return text;
    }


    private string GetRankKey(RankType rank)
    {
        // enum 이름 그대로 쓰면 Rank.Uncommon 형태로 깔끔
        // (주의: enum 이름 바꾸면 키도 같이 바뀜)
        return $"{RankKeyPrefix}{rank}";
    }
}
