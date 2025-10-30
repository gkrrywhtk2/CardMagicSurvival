using UnityEngine;
using UnityEngine.UI;

public class ScrollBoard : MonoBehaviour
{
    public RectTransform[] scrolls;
     // 🔹 미리 세팅해둔 위치 값들 (스크롤별 기본 위치)
    private Vector2[] presetPositions;
    public GameObject[] titleLine_Objects; //0은 프리셋 버튼, 1은 스크롤 내리기
    public Image[] taps;

    private void Awake()
    {
        // 📌 현재 Scroll들의 초기 위치 저장
        presetPositions = new Vector2[scrolls.Length];
        for (int i = 0; i < scrolls.Length; i++)
        {
            presetPositions[i] = scrolls[i].anchoredPosition;
        }
    }

     public void SetScroll(int index)
    {
        for (int i = 0; i < scrolls.Length; i++)
        {
            RectTransform rt = scrolls[i];

            if (i == index)
            {
                // 🎯 미리 저장한 위치 값으로 설정
                rt.anchoredPosition = presetPositions[i];
            }
            else
            {
                // ❌ 화면 밖으로 이동 (좌우 이동만 적용)
                rt.anchoredPosition = new Vector2(3000, presetPositions[i].y);
            }
        }

        //titleLine_Objects[0].SetActive(index == 0); // 덱 관리 UI라면 타이틀 라인 활성화
        SetTaps(index); // 탭 색상 변경
    }

    public void SetTaps(int index)
{
    Color selectedColor = new Color(0.6f, 0.6f, 0.6f);  // 선택된 탭 색상 (밝은 회색)
    Color defaultColor = new Color(0.4f, 0.4f, 0.4f);   // 기본 탭 색상 (회색)

    for (int i = 0; i < taps.Length; i++)
    {
        taps[i].color = (i == index) ? selectedColor : defaultColor;
    }
}


    public void OffUI(){
        //GameManager.instance.boardUI.buttomTapUI.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

}
