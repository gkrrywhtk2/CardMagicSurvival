using UnityEngine;
using UnityEngine.UI;

public class SortButton : MonoBehaviour
{
    public Image upArrow;
    public Image downArrow;

    public SortKey key; // Inspector: Rank / Level

    // ✅ 이 버튼이 마지막으로 표시하던 방향(유지용)
    [SerializeField] private SortDirection lastDirection = SortDirection.Asc;

    private void Awake()
    {
        // ✅ 초기에도 둘 중 하나는 켜져 있도록(기본 Asc)
        ApplyDirection(lastDirection);
    }

    public void ApplyState(SortKey currentKey, SortDirection direction)
    {
        // ✅ 현재 선택된 버튼만 갱신
        if (key != currentKey) return;

        lastDirection = direction;
        ApplyDirection(lastDirection);
    }

    private void ApplyDirection(SortDirection direction)
    {
        if (upArrow != null) upArrow.gameObject.SetActive(direction == SortDirection.Asc);
        if (downArrow != null) downArrow.gameObject.SetActive(direction == SortDirection.Desc);
    }

    // 필요하면 외부에서 초기화용으로 사용
    public void SetDefault(SortDirection direction)
    {
        lastDirection = direction;
        ApplyDirection(lastDirection);
    }
}
