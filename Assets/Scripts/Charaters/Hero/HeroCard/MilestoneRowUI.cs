using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class MilestoneRowUI : MonoBehaviour
{
    [SerializeField] private LocalizeStringEvent localizeEvent;
    public Image lockIcon;

    private const string Table = "UI_Milestones";

    private void Reset()
    {
        if (localizeEvent == null) localizeEvent = GetComponentInChildren<LocalizeStringEvent>();
    }

    public void Bind(StatMod mod)
    {
        // 1) (stat, op) 조합으로 키 선택
        string key = GetKey(mod.stat, mod.op);

        // 2) 표시 값 만들기 (PercentAdd는 *100)
        float raw = (mod.op == ModOp.PercentAdd) ? mod.value * 100f : mod.value;

        // 3) 포맷(원하는 자리수로)
        string formatted = (mod.op == ModOp.PercentAdd)
            ? raw.ToString("F0")     // 10%
            : raw.ToString("F1");    // +0.5 같은 값

        // 4) LocalizedString 만들고 {value} 주입
        var ls = new LocalizedString(Table, key);
        ls.Arguments = new object[] { new { value = formatted } };

        // 5) LocalizeStringEvent에 적용
        localizeEvent.StringReference = ls;
        localizeEvent.RefreshString();
    }

    private string GetKey(StatType stat, ModOp op)
    {
        return stat switch
        {
            StatType.Attack when op == ModOp.PercentAdd => "milestone.attack.percentAdd",
            StatType.Attack when op == ModOp.Add => "milestone.attack.add",
            StatType.MoveSpeed when op == ModOp.PercentAdd => "milestone.moveSpeed.percentAdd",
            StatType.MoveSpeed when op == ModOp.Add     => "milestone.moveSpeed.add",
            StatType.HP when op == ModOp.PercentAdd         => "milestone.maxHP.percentAdd",
            StatType.HP when op == ModOp.Add         => "milestone.maxHP.add",
            StatType.CritChance when op == ModOp.PercentAdd         => "milestone.critChance.percentAdd",
            StatType.CritChance when op == ModOp.Add         => "milestone.critChance.add",
            StatType.CritDamage when op == ModOp.PercentAdd         => "milestone.critDamage.percentAdd",
            StatType.CritDamage when op == ModOp.Add         => "milestone.critDamage.add",
            _ => "milestone.unknown"
        };
    }

        public void SetLocked(bool locked)
    {
        if (lockIcon != null)
            lockIcon.gameObject.SetActive(locked);
    }
}
