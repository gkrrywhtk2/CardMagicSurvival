using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreSet_Deck : MonoBehaviour
{

   public Image mainPanel;//선택 시 색상 변경
   public TMP_Text numberText;//덱 1
   public TMP_Text plusText;//+텍스트
   public Image lockImage;//자물쇠 이미지

   public enum state{unLock, canUnlock, nowLock};
   public state buttonState;
   

   public void UnLock(){
    //해금 되었을때 호출 함수
    numberText.gameObject.SetActive(true);
    plusText.gameObject.SetActive(false);
    lockImage.gameObject.SetActive(false);
    buttonState = state.unLock;
   }

   public void CanUnlock(){
    //해금 할 수 있는 프리셋 버튼 일때 호출
    numberText.gameObject.SetActive(false);
    plusText.gameObject.SetActive(true);
    lockImage.gameObject.SetActive(false);
    buttonState = state.canUnlock;
   }

   public void NowLock(){
    //해금 할 수 없는 프리셋 버튼 일때 호출
     numberText.gameObject.SetActive(false);
    plusText.gameObject.SetActive(false);
    lockImage.gameObject.SetActive(true);
     buttonState = state.nowLock;
   }

   



}
