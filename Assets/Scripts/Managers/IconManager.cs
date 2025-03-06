using System.Collections.Generic;
using UnityEngine;

public class IconManager : MonoBehaviour
{
    public GameObject iconPrefab; // 아이콘 프리팹
    public Transform iconParent; // 아이콘을 담을 부모 오브젝트

    private Dictionary<int, Icon_MagicCard> activeIcons = new Dictionary<int, Icon_MagicCard>(); // 현재 활성화된 아이콘 목록

    public void AddOrUpdateIcon(int iconID, float duration)
    {
        // 같은 ID의 아이콘이 이미 존재하면 제거
        if (activeIcons.ContainsKey(iconID))
        {
            Destroy(activeIcons[iconID].gameObject);
            activeIcons.Remove(iconID);
        }

        // 새로운 아이콘 생성
        GameObject newIcon = Instantiate(iconPrefab, iconParent);
        Icon_MagicCard iconScript = newIcon.GetComponent<Icon_MagicCard>();

        // 아이콘 초기화
        iconScript.Init(iconID, duration);

        // 딕셔너리에 추가
        activeIcons[iconID] = iconScript;
    }

    public void RemoveIcon(int iconID)
    {
        if (activeIcons.ContainsKey(iconID))
        {
            Destroy(activeIcons[iconID].gameObject);
            activeIcons.Remove(iconID);
        }
    }
}
