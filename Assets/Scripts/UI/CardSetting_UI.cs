using UnityEngine;
using UnityEngine.UI;

public class CardSetting_UI : MonoBehaviour
{
    public RectTransform[] scrolls;
    public GameObject titleLine_Preset;
    public Image[] taps;

/**
    public void SetScroll(int index)
    {
        Debug.Log("Touch");
        for (int i = 0; i < scrolls.Length; i++)
        {
            if (i == index)
            {
                // 선택된 스크롤은 Left, Right를 0으로 설정
                scrolls[i].offsetMin = new Vector2(0, scrolls[i].offsetMin.y);
                scrolls[i].offsetMax = new Vector2(0, scrolls[i].offsetMax.y);
            }
            else
            {
                 // 나머지 스크롤들은 화면 밖으로 이동 (Left = 1500, Right = -1500)
                scrolls[i].offsetMin = new Vector2(1500, scrolls[i].offsetMin.y);
                scrolls[i].offsetMax = new Vector2(-1500, scrolls[i].offsetMax.y);
            }
        }

        titleLine_Preset.SetActive(index == 0); // 덱 관리 UI라면 타이틀 라인 활성화
        SetTaps(index); // 탭 색상 변경
    }

    **/
    public void SetScroll(int index)
{
    Debug.Log("Touch");

    for (int i = 0; i < scrolls.Length; i++)
    {
        if (i == index)
        {
            // 선택된 스크롤은 제자리로 이동
            scrolls[i].anchoredPosition = Vector2.zero;
        }
        else
        {
            // 선택되지 않은 스크롤들은 화면 밖으로 이동 (오른쪽으로 보내기)
            scrolls[i].anchoredPosition = new Vector2(1500, 0);
        }
    }

    titleLine_Preset.SetActive(index == 0); // 덱 관리 UI라면 타이틀 라인 활성화
    SetTaps(index); // 탭 색상 변경
}

    public void SetTaps(int index)
    {
        Color selectedColor = Color.white;         // 선택된 탭 색상
        Color defaultColor = new Color(0.6f, 0.6f, 0.6f); // #9A9A9A (RGB: 154,154,154)

        for (int i = 0; i < taps.Length; i++)
        {
            taps[i].color = (i == index) ? selectedColor : defaultColor;
        }
    }
}
