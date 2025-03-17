using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreSet_Deck : MonoBehaviour
{

   public Image mainPanel;//선택 시 색상 변경
   public TMP_Text numberText;//덱 1
   public TMP_Text plusText;//+텍스트
   public Image lockImage;//자물쇠 이미지
   

   public void UnLock(){
    //해금 되었을때 호출 함수
    numberText.gameObject.SetActive(true);
    plusText.gameObject.SetActive(false);
    lockImage.gameObject.SetActive(false);
   }

   public void CanUnlock(){
    //해금 할 수 있는 프리셋 버튼 일때 호출
      mainPanel.color = Color.white;
    numberText.gameObject.SetActive(false);
    plusText.gameObject.SetActive(true);
    lockImage.gameObject.SetActive(false);
   }

   public void Lock(){
    //해금 할 수 없는 프리셋 버튼 일때 호출
   }

}
