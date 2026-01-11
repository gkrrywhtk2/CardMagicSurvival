using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class HeroNameUI : MonoBehaviour
{
    [SerializeField] private LocalizeStringEvent localizeEvent;

    // ✅ 여기만 너 프로젝트 테이블 이름으로 바꿔주면 됨
    private const string Table = "UI_HeroInfo";

    private const string HeroNameKeyPrefix = "Hero.Name.";

    private void Reset()
    {
        if (localizeEvent == null) localizeEvent = GetComponentInChildren<LocalizeStringEvent>();
    }

    /// <summary>
    /// heroId가 0이면 "Hero.Name.0" 키로 로컬라이즈해서 표시
    /// </summary>
    public void BindHeroName(int heroId)
    {
        if (localizeEvent == null)
        {
            Debug.LogWarning($"{nameof(HeroNameUI)}: LocalizeStringEvent가 비어있습니다.");
            return;
        }

        string key = GetHeroNameKey(heroId);

        var ls = new LocalizedString(Table, key);

        // 값 주입 필요 없으면 Arguments 생략
        localizeEvent.StringReference = ls;
        localizeEvent.RefreshString();
    }

    private string GetHeroNameKey(int heroId)
    {
        // 안전장치: 음수면 unknown으로
        if (heroId < 0) return "Hero.Name.Unknown";
        return $"{HeroNameKeyPrefix}{heroId}";
    }
}
