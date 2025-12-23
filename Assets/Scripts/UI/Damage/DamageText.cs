using System.Collections;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public float value;

    [Header("Refs")]
    [SerializeField] public TMP_Text damageText;

    [Header("Colors (Inspector)")]
    public Color normalColor = Color.white;
    public Color criticalColor = new Color(1f, 0.3f, 0.3f, 1f);
    public Color poisonColor = Color.green;

    [Header("Policy")]
    public bool poisonCanCrit = false;          // 보통 독은 크리 안 터지게
    public bool forceColorAfterFrame = true;    // 애니메이션 색 덮어쓰기 방어

    private Animator anim;
    private Coroutine colorFixCo;

    private void Awake()
    {
        if (damageText == null)
            damageText = GetComponentInChildren<TMP_Text>(true);

        anim = GetComponent<Animator>();
        if (anim == null && damageText != null)
            anim = damageText.GetComponent<Animator>();
    }

    public void Init(DamageType type, bool isCritical)
    {
        if (damageText == null) return;

        damageText.text = Mathf.RoundToInt(value).ToString();

        // ✅ 크리 파라미터 (독은 기본적으로 크리 false 처리)
        bool critForAnim = (type == DamageType.Poison && !poisonCanCrit) ? false : isCritical;
        if (anim != null) anim.SetBool("IsCritical", critForAnim);

        // ✅ 최종 색 결정
        Color target = GetColor(type, isCritical);
        ApplyColor(target);
    }

    private Color GetColor(DamageType type, bool isCritical)
    {
        if (type == DamageType.Poison)
        {
            if (poisonCanCrit && isCritical) return criticalColor; // 원하면 독 크리 지원
            return poisonColor;
        }

        // Normal
        return isCritical ? criticalColor : normalColor;
    }

    private void ApplyColor(Color c)
    {
        damageText.faceColor = c;

        if (forceColorAfterFrame)
        {
            if (colorFixCo != null) StopCoroutine(colorFixCo);
            colorFixCo = StartCoroutine(ApplyColorNextFrame(c));
        }
    }

    private IEnumerator ApplyColorNextFrame(Color c)
    {
        yield return null;
        if (damageText != null) damageText.color = c;
        colorFixCo = null;
    }

    private void OnDisable()
    {
        if (colorFixCo != null)
        {
            StopCoroutine(colorFixCo);
            colorFixCo = null;
        }

        // 풀링 대비: 기본값으로 복구
        if (damageText != null) damageText.color = normalColor;

        if (anim != null) anim.SetBool("IsCritical", false);
    }
}
