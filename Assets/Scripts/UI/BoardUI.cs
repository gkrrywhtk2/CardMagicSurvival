using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class BoardUI : MonoBehaviour
{
   public GameObject[] boards;
   //0 : upgrade, 1 : item  2: card, 3: spellbook, 4: fairy 5: shop
   public GameObject mainBoard_Card;//카드 보드
   public Image[] taps_Image;
   public RectTransform[] taps_Rect;
   public Image[] taps_Icons;
   public TMP_Text[] taps_text;//탭 이름 텍스트
   //
   public GameObject deckSettingUI;//덱 관리 UI 창

   public UpgradeUI upgradeUI;//
   public GameObject EqipUI;//장비 UI
   public GameObject buttomTapUI;//아래 위치하는 탭 UI창, 카드 세팅UI연출시 활성화.
   //
   public DeckCard[] deckCardUI;//덱 관리 UI에서 현재 플레이어의 카드 8장
   public CardInfoUI cardInfoUI;
   
   public GameObject deckCardButtons;//카드 누르면 정보, 추가, 제거 버튼

   //모두 레벨업 버튼
    public GameObject allLevelUp_Weapon;
    public GameObject allLevelUp_Acc;
  
   private void Awake() {     //Debug.Log(boards[0].GetComponent<RectTransform>().anchoredPosition);
   // HideAllBoards();
    // ShowBoard(2);//일단 카드 ui 호출
   }

   public void HideAllBoards(){
    Vector3 hidePostion = new Vector3(-1500, 0, 0);
    for(int i =0; i<boards.Length;i++){
        boards[i].GetComponent<RectTransform>().anchoredPosition = hidePostion;
    }
   }
  public void HideAllTaps()
{
    // 모든 탭 비활성화 (y 좌표를 -20으로 내려서 숨기고 색상 변경)
    for (int i = 0; i < taps_Rect.Length; i++)
    {
        Vector2 hidePosition = taps_Rect[i].anchoredPosition;
        //hidePosition.y = -40; // 비활성화 위치
        taps_Rect[i].anchoredPosition = hidePosition;

        // 비활성화 색상 변경 (BCBCBC)
        taps_Image[i].color = new Color32(188, 188, 188, 255);
        taps_Icons[i].color = new Color32(188, 188, 188, 255);
        taps_text[i].color = new Color32(188, 188, 188, 255);
    }
}

public void ShowSeletedTap(int tapNum)
{
    // 선택한 탭만 활성화 (y 좌표를 0으로 올려서 표시)
    Vector2 showPosition = taps_Rect[tapNum].anchoredPosition;
    //showPosition.y = -30; // 활성화 위치
    taps_Rect[tapNum].anchoredPosition = showPosition;

    // 활성화 색상 변경 (FFFFFF)
    taps_Image[tapNum].color = new Color32(255, 255, 255, 255);
     taps_Icons[tapNum].color = new Color32(255, 255, 255, 255);
     taps_text[tapNum].color = new Color32(255, 255, 255, 255);
}


   public void ShowBoard(int boardId){
    Vector3 showPos = new Vector3(0,0,0);
   HideAllBoards();
    boards[boardId].GetComponent<RectTransform>().anchoredPosition = showPos;
   }
   public void Show_UpgradeBoard(){
    /**
   Vector3 showPos = new Vector3(0,0,0);
    HideAllBoards();
    UpgradeUI upgradeUI = boards[0].GetComponent<UpgradeUI>();
    upgradeUI.GetComponent<RectTransform>().anchoredPosition = showPos;
    upgradeUI.AllUpgradeSetting();
    **/

      upgradeUI.gameObject.SetActive(true);
      upgradeUI.AllUpgradeSetting();
      buttomTapUI.gameObject.SetActive(true);
      
   }
   public void Show_WeaponBoard(){
     Vector3 showPos = new Vector3(0,0,0);
     GameManager.instance.weaponManager.WeaponImageSetting();
     HideAllBoards();
     boards[1].GetComponent<RectTransform>().anchoredPosition = showPos;
     
   }
    public void Show_DeckSettingUI(){
      deckSettingUI.gameObject.SetActive(true);
      buttomTapUI.gameObject.SetActive(true);
       deckCardButtons.gameObject.SetActive(false);//일단 비활성화
     }
     public void Show_EqipBoardUI(){
        EqipUI.gameObject.SetActive(true);
        GameManager.instance.weaponManager.WeaponImageSetting();
        buttomTapUI.gameObject.SetActive(true);
     }

     public void Hide_DeckSettingUI(){
       deckSettingUI.gameObject.SetActive(false);
      buttomTapUI.gameObject.SetActive(false);
     }

     public void DeckCardButtonOff(){
       Debug.Log("빈곳 터치");
        deckCardButtons.gameObject.SetActive(false);
    }
    public void OffButtonUI(){
      buttomTapUI.gameObject.SetActive(false);
    }

  public void EquipAllLevelUpButton(int index){
    switch (index){
      case 0 :
      allLevelUp_Weapon.gameObject.SetActive(true);
      allLevelUp_Weapon.gameObject.SetActive(false);
      break;

      case 1:
      allLevelUp_Weapon.gameObject.SetActive(false);
      allLevelUp_Weapon.gameObject.SetActive(true);
      break;
    }
  }
    
}

