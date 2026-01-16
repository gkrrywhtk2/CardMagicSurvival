using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillFrame_Image : MonoBehaviour
{
    [Header("REF")]
    public Image frame;
    public Image icon;
    public TMP_Text text_Lv;
    public Slider expSlider;
    public TMP_Text text_exp;
    public Image upArrow;
    public Image lockImage; // 좌물쇠
    public Image lockBgImage; // 좌물쇠 배경

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
        if (text_Lv != null) text_Lv.text = $"Lv.{level}";

        // exp
        if (expSlider != null)
        {
            expSlider.maxValue = maxExp;
            expSlider.value = Mathf.Clamp(exp, 0, maxExp);
        }

        if (text_exp != null) text_exp.text = $"{exp} / {maxExp}";

        // icon
        if (icon != null) icon.sprite = skillIcon;

        // frame color
        if (frame != null) frame.color = frameColor;

        // lock
        if (lockImage != null) lockImage.gameObject.SetActive(isLocked);
        if (lockBgImage != null) lockBgImage.gameObject.SetActive(isLocked);

        // upArrow (잠기면 무조건 false)
        bool canLevelUp = exp >= maxExp;
        if (upArrow != null) upArrow.gameObject.SetActive(!isLocked && canLevelUp);
    }
}
