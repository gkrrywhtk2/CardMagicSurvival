using UnityEngine;
using Game.RankSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.TextCore.Text;

public class HeroCard : MonoBehaviour
{
    public RankType rank;      // 등급
    public int heroID;         // 영웅ID (씬에서 미리 세팅해둔다)
    public int heroLevel;      // 영웅레벨
    public int nowExp;            // 경험치
    public int maxExp;            // 최대경험치
    public bool ownedBool;     // 보유여부
    public bool isselected;    // 선택여부

    [Header("UI Refs")]
    public Image frameImage;
    public Image innerImage;
    public Image heroImage;
    public TMP_Text levelText;
    public TMP_Text nameText;
    public TMP_Text sliderText;//4/5 형식
    public Slider expSlider;//경험치 슬라이더
    public Image selectedImage;//선택 표시 이미지


    // ✅ 매니저가 파싱한 값들을 여기에 넣어줌
    public void Init(int level, int exp, int rankInt, bool isUnlocked, bool isSelected)
    {
        heroLevel = level;
        nowExp = exp;
        rank = (RankType)rankInt;
        ownedBool = isUnlocked;
        isselected = isSelected;

        ColorSetting();
        HeroManager.Instance.MaxExpSetting(heroLevel);
        SliderSetting();

        heroImage.sprite = GameManager.instance.heroManager.heroes[heroID].heroSprite_ForHeroCard;
        TextSetting();
        SelectedImageSetting(isselected);
    }
    public void SliderSetting()
    {
        maxExp = HeroManager.Instance.MaxExpSetting(heroLevel);
        float ratio = (maxExp <= 0) ? 0f : nowExp / (float)maxExp;
        expSlider.minValue = 0f;
        expSlider.maxValue = 1f;
        expSlider.value = ratio;           // 0.0 ~ 1.0
        sliderText.text = $"{nowExp}/{maxExp}";
    }
    public void SelectedImageSetting(bool isSelected)
    {
        if (selectedImage != null)
            selectedImage.gameObject.SetActive(isSelected);
    }
    public void TextSetting()
    {
        levelText.text = "Lv. " + heroLevel;
        nameText.text = GameManager.instance.heroManager.heroes[heroID].nameKor;
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
