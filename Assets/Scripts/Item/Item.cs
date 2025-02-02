using TMPro;
using UnityEngine;
using RANK;

public class Item : MonoBehaviour
{
    [Header("#Main Info")]
    public int ItemID;
    public int originItemNum;//012 3가지의 미리 생성된 아이템을 번호만 초기화하여 구현,
    public Rank itemrank;
    public SpriteRenderer itemSprite;
    

    private void Awake() {
        
    }

    public void Init(ItemData data){
        this.ItemID = data.ItemID;
        itemSprite.sprite = data.ItemSprite;
        this.itemrank = data.rank;
    } 

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player"){
            GameManager.instance.itemManager.ItemSelected(this.ItemID);
        }
    }
    public void Init_UI(){
        //아이템 설명 UI창 초기화
        GameManager.instance.itemManager.ItemUI.gameObject.SetActive(true);

        //위치 설정
        GameManager.instance.itemManager.ItemUI.transform.position = GameManager.instance.itemManager.Uipos[originItemNum].position;
        ItemData[] data = GameManager.instance.itemManager.itemDatas_normal;
        GameManager.instance.itemManager.Ui_Name.text = data[ItemID].ItemName;
        GameManager.instance.itemManager.Ui_Desc.text = data[ItemID].ItemDesc;
    }

    
}
