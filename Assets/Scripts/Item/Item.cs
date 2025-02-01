using TMPro;
using UnityEngine;
using RANK;

public class Item : MonoBehaviour
{
    [Header("#Main Info")]
    public int ItemID;
    public int originItemID;
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
            GameManager.instance.itemManager.ItemSelected(this.ItemID,originItemID);
        }
    }
}
