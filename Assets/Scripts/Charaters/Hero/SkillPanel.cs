using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Game.RankSystem;

public class SkillPanel : MonoBehaviour
{
    [SerializeField] private string tableName = "HeroSkill";

    public TMP_Text text_Name;
    public TMP_Text text_SubDesc;
    public TMP_Text text_MainDesc;
    public SkillFrame_Image skillFrame_Image;
    public RankLabelUI rankLabelUI;
    public GameObject lockBG;
    public TMP_Text lockText;

    private ServerDataManager.HeroAccount data;
    private RankType rank;
    public HeroScriptableObject heroScriptableObject;
    public RankLocalization rankLocalization;
    

    public void Init(ServerDataManager.HeroAccount heroAccount, RankType rank)
    {
        data = heroAccount;
        this.rank = rank;
        heroScriptableObject = HeroManager.Instance.heroes[heroAccount.heroId];
        RefreshUI();
    }

    private string Key(string field) => $"HeroSkill.{data.heroId}.{rank}.{field}";

    public async void RefreshUI()
    {
        if (data == null) return;

        text_Name.text = await LocalizationSettings.StringDatabase
            .GetLocalizedStringAsync(tableName, Key("name")).Task;

        // text_SubDesc.text = await LocalizationSettings.StringDatabase
        //     .GetLocalizedStringAsync(tableName, Key("sub")).Task;

        // ✅ 핵심: 스킬별 args(object[])만 받아서 주입
        object[] args = SkillArgsProvider.GetArgs(data.heroId, rank, data);

        var localized = new LocalizedString(tableName, Key("desc"))
        {
            Arguments = args
        };

        try
    {
        text_MainDesc.text = await localized.GetLocalizedStringAsync().Task;
    }
    catch (System.Exception ex)
    {
        Debug.LogError($"[LOC FAIL] key={Key("desc")}, rank={rank}, heroId={data.heroId}");
        Debug.LogError(ex); // OperationException
        if (ex.InnerException != null)
            Debug.LogError("INNER => " + ex.InnerException); // ✅ 여기 진짜 원인이 뜸

        // args도 같이 찍어봐
        if (args != null)
        {
            for (int i = 0; i < args.Length; i++)
                Debug.LogError($"arg[{i}] = {args[i]} (type={args[i]?.GetType().Name})");
        }
    }



        int lv = GetLevelByType(data, rank);
        int exp = GetExpByType(data, rank);

        int maxExp = 10; // TODO: 규칙으로 교체

        Sprite icon = GetSkillIconBySlotType();
        Color frameColor = RankDatas.GetColor(rank);

        bool isLocked = IsLocked();

        skillFrame_Image.Render(
            level: lv,
            exp: exp,
            maxExp: maxExp,
            skillIcon: icon,
            frameColor: frameColor,
            isLocked: isLocked
        );
        rankLabelUI.SetRank(rank);
        rankLocalization.BindRank(rank);

        await UpdateLockUIAsync(isLocked);
    }

     private Sprite GetSkillIconBySlotType()
    {
        
        if (heroScriptableObject == null || heroScriptableObject.rankUnlocks == null)
            return null;

        RankType targetRank = rank;

        var unlock = heroScriptableObject.rankUnlocks.Find(u => u.rank == targetRank);
        return unlock != null ? unlock.skillSpite : null;
    }

    private int GetLevelByType(ServerDataManager.HeroAccount h, RankType t) => t switch
    {
        RankType.Uncommon => h.cSkillLevel,
        RankType.Rare => h.rSkillLevel,
        RankType.Epic => h.eSkillLevel,
        RankType.Legendary => h.lSkillLevel,
        RankType.Mythic => h.mSkillLevel,
        _ => 1
    };

    private int GetExpByType(ServerDataManager.HeroAccount h, RankType t) => t switch
    {
        RankType.Uncommon => h.cSkillExp,
        RankType.Rare => h.rSkillExp,
        RankType.Epic => h.eSkillExp,
        RankType.Legendary => h.lSkillExp,
        RankType.Mythic => h.mSkillExp,
        _ => 0
    };


    private bool IsLocked()
    {
        RankType slotRank = rank;
        RankType heroOpenRank = (RankType)data.rank; // heroAccount.rank가 int 라면 캐스팅
        return heroOpenRank < slotRank;
    }

        private const string CommonTable = "UI_Common";
    private const string UnlockKey = "UnlockAtRank";

    private async System.Threading.Tasks.Task UpdateLockUIAsync(bool isLocked)
    {
        if (lockBG != null)
            lockBG.SetActive(isLocked);

        if (lockText == null) return;

        if (!isLocked)
        {
            lockText.text = string.Empty;
            return;
        }

        // ✅ 색상은 해당 등급 색
        lockText.color = RankDatas.GetColor(rank);

        // ✅ 등급명(다국어)
        string rankName = await RankLocalization.GetRankNameAsync(rank);

        // ✅ "해금 문장"(다국어 템플릿에 {0} 주입)
        var ls = new LocalizedString(CommonTable, UnlockKey)
        {
            Arguments = new object[] { rankName }
        };

        lockText.text = await ls.GetLocalizedStringAsync().Task;
    }
}



