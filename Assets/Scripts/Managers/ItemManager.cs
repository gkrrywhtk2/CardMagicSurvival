using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{

    public ItemData[] itemDatas;         // 모든 아이템 데이터
    public List<int> deck = new List<int>(); // 현재 회득 아이템
    public RectTransform[] Points; // 아이템 생성 위치
    public Item[] items;  //아이템 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
}
