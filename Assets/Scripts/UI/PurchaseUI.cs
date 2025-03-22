using TMPro;
using UnityEngine;

public class PurchaseUI : MonoBehaviour
{
    public enum PurchaseList
{
    ResetStatPoint,
    Deckpreset,
    preset2,
    preset3,
    preset4
}
   public PurchaseList purchase;
   public TMP_Text descText;//설명 TEXT
   public TMP_Text priceText;//가격 Text
   public int price;//가격

   public void Init(PurchaseList index){
      switch (index)
        {
            case PurchaseList.ResetStatPoint:
                descText.text = "모든 스탯포인트를 돌려받습니다.\n 스탯을 초기화 하시겠습니까?";
                price = 3000;
                priceText.text = price.ToString();
                purchase = index;
                break;
            case PurchaseList.Deckpreset:
                descText.text = "덱 프리셋을 추가하시겠습니까?";
                price = 3000;
                priceText.text = price.ToString();
                purchase = index;
                break;
            case PurchaseList.preset2:
               
                break;
            case PurchaseList.preset3:
                
                break;
            case PurchaseList.preset4:
               
                break;
        }
   }

    public void ButtonPurchase(){

        if(GameManager.instance.dataManager.rubyPoint >= price){
            GameManager.instance.dataManager.rubyPoint -= price;
            HandlePurchase(purchase);
            gameObject.SetActive(false);
        }else{
            GameManager.instance.warningUI.gameObject.SetActive(true);
            GameManager.instance.warningUI.text.text = "재화가 부족합니다";
        }
       
    }
     public void HandlePurchase(PurchaseList type)
    {
        switch (type)
        {
            case PurchaseList.ResetStatPoint:
                ResetStatPoint();
                break;
            case PurchaseList.Deckpreset:
                DeckPreSet();
                break;
            case PurchaseList.preset2:
               
                break;
            case PurchaseList.preset3:
                
                break;
            case PurchaseList.preset4:
               
                break;
        }
    }
    public void ResetStatPoint(){
        //스탯 초기화 로직
        GameManager.instance.boardUI.upgradeUI.ResetTraningPoint();
    }
    public void DeckPreSet(){
        //스탯 초기화 로직
        GameManager.instance.deckManager.cardSetting_UI.BuyButton();
    }

    public void ButtonOff(){
        gameObject.SetActive(false);
    }
   
}
