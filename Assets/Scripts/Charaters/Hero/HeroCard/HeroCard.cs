using UnityEngine;
using Game.RankSystem;
using UnityEngine.UI;
using TMPro;

public class HeroCard : MonoBehaviour
{
    public RankType rank;
    public int heroID;
    public int heroLevel;
    public int nowExp;
    public int maxExp;
    public bool ownedBool;
    public bool isselected;

    [Header("UI Refs")]
    public Image frameImage;
    public Image innerImage;
    public Image heroImage;
    public TMP_Text levelText;
    public TMP_Text nameText;
    public TMP_Text sliderText;
    public Slider expSlider;
    public Image selectedImage;

    public void Init(int level, int exp, int rankInt, bool isUnlocked, bool isSelected)
    {
        heroLevel = level;
        nowExp = exp;
        rank = (RankType)rankInt;
        ownedBool = isUnlocked;
        isselected = isSelected;

        ColorSetting();
        SliderSetting();
        TextSetting();
        SelectedImageSetting(isselected);
    }

    public void SliderSetting()
    {
        maxExp = HeroManager.Instance.MaxExpSetting(heroLevel);
        float ratio = (maxExp <= 0) ? 0f : nowExp / (float)maxExp;

        if (expSlider != null)
        {
            expSlider.minValue = 0f;
            expSlider.maxValue = 1f;
            expSlider.value = ratio;
        }

        if (sliderText != null)
            sliderText.text = (maxExp <= 0) ? "0/0" : $"{nowExp}/{maxExp}";
    }

    public void SelectedImageSetting(bool isSelected)
    {
        if (selectedImage != null)
            selectedImage.gameObject.SetActive(isSelected);
    }

    public void TextSetting()
    {
        if (levelText != null)
            levelText.text = $"Lv. {heroLevel}";

        // ✅ 안전 접근 추천 (가능하면)
        var heroSO = HeroManager.Instance.GetHeroSO(heroID);
        if (heroSO != null)
        {
            if (heroImage != null) heroImage.sprite = heroSO.heroSprite_ForHeroCard;
            if (nameText != null) nameText.text = heroSO.nameKor;
        }
        else
        {
            // 기존 방식 fallback (정말 필요할 때만)
            if (GameManager.instance != null && GameManager.instance.heroManager != null)
            {
                var list = GameManager.instance.heroManager.heroes;
                if (list != null && heroID >= 0 && heroID < list.Length && list[heroID] != null)
                {
                    if (heroImage != null) heroImage.sprite = list[heroID].heroSprite_ForHeroCard;
                    if (nameText != null) nameText.text = list[heroID].nameKor;
                }
            }
        }
    }

    public void ColorSetting()
    {
        Color32 frameColor;
        Color32 innerColor;

        switch (rank)
        {
            case RankType.Uncommon:
                frameColor = new Color32(0x8A, 0x8D, 0x93, 0xFF);
                innerColor = new Color32(0xA5, 0xA7, 0xAB, 0xFF);
                break;
            case RankType.Rare:
                frameColor = new Color32(0x50, 0x9A, 0xFF, 0xFF);
                innerColor = new Color32(0x85, 0xBC, 0xFF, 0xFF);
                break;
            case RankType.Epic:
                frameColor = new Color32(0xB3, 0x63, 0xDF, 0xFF);
                innerColor = new Color32(0xCC, 0x96, 0xEA, 0xFF);
                break;
            case RankType.Legendary:
                frameColor = new Color32(0xEB, 0xB6, 0x2D, 0xFF);
                innerColor = new Color32(0xF2, 0xCE, 0x72, 0xFF);
                break;
            case RankType.Mythic:
                frameColor = new Color32(0xD2, 0x41, 0x69, 0xFF);
                innerColor = new Color32(0xE1, 0x80, 0x9A, 0xFF);
                break;
            default:
                frameColor = new Color32(0x8A, 0x8D, 0x93, 0xFF);
                innerColor = new Color32(0xA5, 0xA7, 0xAB, 0xFF);
                break;
        }

        if (frameImage != null) frameImage.color = frameColor;
        if (innerImage != null) innerImage.color = innerColor;
    }

    public void CallHeroInfo()
    {
        HeroManager.Instance.ApplyHeroToHeroInfo(heroID);
    }
}
