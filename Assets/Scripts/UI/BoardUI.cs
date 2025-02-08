using UnityEngine;

public class BoardUI : MonoBehaviour
{
   public GameObject[] boards;
   //0 : upgrade, 1 : item ...
   private void Awake() {
     //Debug.Log(boards[0].GetComponent<RectTransform>().anchoredPosition);
     HideAllBoards();
     ShowBoard(2);//일단 카드 ui 호출
   }

   public void HideAllBoards(){
    Vector3 hidePostion = new Vector3(-1500, 0, 0);
    for(int i =0; i<boards.Length;i++){
        boards[i].GetComponent<RectTransform>().anchoredPosition = hidePostion;
    }
   }

   public void ShowBoard(int boardId){
    Vector3 showPos = new Vector3(0,0,0);
    HideAllBoards();
    boards[boardId].GetComponent<RectTransform>().anchoredPosition = showPos;
   }
   public void Show_UpgradeBoard(){
   Vector3 showPos = new Vector3(0,0,0);
    HideAllBoards();
    UpgradeUI upgradeUI = boards[0].GetComponent<UpgradeUI>();
    upgradeUI.GetComponent<RectTransform>().anchoredPosition = showPos;
    upgradeUI.ATK_Setting();

   }
   }

