using TMPro;
using UnityEngine;
using RANK;

public class Item : MonoBehaviour
{
    [Header("#Main Info")]
    public int ItemID;
    public Rank itemrank;
    SpriteRenderer spr;
    

    private void Awake() {
        spr = GetComponent<SpriteRenderer>();
    }

    public void Init(ItemData data){
        this.ItemID = data.ItemID;
        spr.sprite = data.ItemSprite;
        this.itemrank = data.rank;
    } 
}
