using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillFrame_Image : MonoBehaviour
{
    private const int MAX_LEVEL = 10;

    [Header("REF")]
    public Image frame;
    public Image icon;
    public TMP_Text text_Lv;
    public Slider expSlider;
    public Image expFill;
    public TMP_Text text_exp;
    public Image upArrow;
    public Image lockImage;     // 좌물쇠
    public Image lockBgImage;   // 좌물쇠 배경

    [Header("Colors")]
    public Color blue_Color;
    public Color green_Color;

    /// <summary>
    /// 비주얼 갱신 (계산은 외부에서 끝내고 값만 전달)
    /// </summary>
    public void Render(
        int level,
        int exp,
        int maxExp,
        Sprite skillIcon,
        Color frameColor,
        bool isLocked
    )
    {
        bool isMax = level >= MAX_LEVEL;

        // Lv 텍스트
        if (text_Lv != null)
            text_Lv.text = $"Lv.{level}";

        // 아이콘
        if (icon != null)
            icon.sprite = skillIcon;

        // 프레임 색
        if (frame != null)
            frame.color = frameColor;

        // 잠금 표시
        if (lockImage != null) lockImage.gameObject.SetActive(isLocked);
        if (lockBgImage != null) lockBgImage.gameObject.SetActive(isLocked);

        // exp 관련 안전 처리
        int safeMaxExp = Mathf.Max(1, maxExp);
        int clampedExp = Mathf.Clamp(exp, 0, safeMaxExp);

        // 만렙이면 exp UI 고정
        if (isMax)
        {
            if (text_exp != null) text_exp.text = "MAX";

            if (expSlider != null)
            {
                expSlider.maxValue = safeMaxExp;
                expSlider.value = expSlider.maxValue;
            }

            if (expFill != null) expFill.color = blue_Color;
            if (upArrow != null) upArrow.gameObject.SetActive(false);
            return;
        }

        // 만렙 아니면 정상 exp 표시
        if (expSlider != null)
        {
            expSlider.maxValue = safeMaxExp;
            expSlider.value = clampedExp;
        }

        if (text_exp != null)
            text_exp.text = $"{clampedExp} / {safeMaxExp}";

        // 레벨업 가능 여부: maxExp가 0인 데이터는 레벨업 불가로 취급
        bool canLevelUp = (maxExp > 0) && (exp >= maxExp);

        if (upArrow != null)
            upArrow.gameObject.SetActive(!isLocked && canLevelUp);

        if (expFill != null)
            expFill.color = canLevelUp ? green_Color : blue_Color;
    }
}
