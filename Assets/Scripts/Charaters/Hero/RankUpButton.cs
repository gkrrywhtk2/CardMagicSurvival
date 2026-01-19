using Game.RankSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankUpButton : MonoBehaviour
{
    public TMP_Text text_value;
    public TMP_Text text_rankUp;
    public TMP_Text text_MaxRank;
    public Image value_BG;
    public Image lockBG;
    public HeroInfo heroInfo;
    public SkillPanel skillPanel;
    public Button button; // ✅ 추가

    private int value;

    private void Reset()
    {
        if (button == null) button = GetComponent<Button>(); // ✅ 자동 연결
    }

    public void Init(int value)
    {
        this.value = value;

        int heroId = heroInfo.id;
        bool canBeMythic = ServerDataManager.instance.GetIsMythicHero(heroId);

        RankType maxRank = canBeMythic ? RankType.Mythic : RankType.Legendary;
        RankType nowRank = ServerDataManager.instance.GetHeroRank(heroId);

        bool isMaxRank = nowRank >= maxRank;
        bool hasEnoughStone = ServerDataManager.instance.GetCurrentUpgradeStone() >= value;
        bool canRankUp = hasEnoughStone && !isMaxRank;

        text_value.text = value.ToString();
        text_value.color = canRankUp ? Color.white : Color.red;

        text_MaxRank.gameObject.SetActive(isMaxRank);
        text_rankUp.gameObject.SetActive(!isMaxRank);

        value_BG.gameObject.SetActive(canRankUp);
        lockBG.gameObject.SetActive(!canRankUp);

        // ✅ 여기 한 줄로 “터치/깜빡임” 자체를 차단
        //if (button != null) button.interactable = canRankUp;
    }

    public void RankUpButton_EXE()
    {
        // ✅ 혹시라도 이벤트가 호출되면(키보드/코드 등) 방어
        if (button != null && !button.interactable) return;

        int heroId = heroInfo.id;

        if (ServerDataManager.instance.GetCurrentUpgradeStone() < value)
        {
            Debug.LogWarning("[RankUpButton] Not enough upgrade stones.");
            return;
        }

        RankType nowRank = ServerDataManager.instance.GetHeroRank(heroId);

        bool canBeMythic = ServerDataManager.instance.GetIsMythicHero(heroId);
        RankType maxRank = canBeMythic ? RankType.Mythic : RankType.Legendary;

        if (nowRank >= maxRank)
        {
            Debug.LogWarning("[RankUpButton] Already at max rank for this hero.");
            return;
        }

        // ✅ 차감은 “성공이 확정된 뒤”에 하는 게 안전
        ServerDataManager.instance.AddUpgradeStone(-value);

        RankType nextRank = (RankType)((int)nowRank + 1);
        ServerDataManager.instance.HeroRankUp(heroId, nextRank);

        heroInfo.RefreshUI();
        skillPanel.RefreshUI();

        // (선택) 버튼 UI도 즉시 갱신하고 싶으면:
        // Init(value);
    }
}
