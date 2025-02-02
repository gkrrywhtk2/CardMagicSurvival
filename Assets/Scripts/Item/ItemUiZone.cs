using UnityEngine;

public class ItemUiZone : MonoBehaviour
{
    public Item Parent;

    private void OnTriggerEnter2D(Collider2D collison) {
        if(collison.CompareTag("Player")){
           Parent.Init_UI();
        }
    }

    private void OnTriggerExit2D(Collider2D collison) {
         if(collison.CompareTag("Player")){
            //UI 비활성화
           GameManager.instance.itemManager.ItemUI.gameObject.SetActive(false);
        }
    }
}
