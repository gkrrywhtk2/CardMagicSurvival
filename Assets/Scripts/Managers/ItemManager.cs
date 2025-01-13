using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ItemManager : MonoBehaviour
{

    public ItemData[] itemDatas;         // 모든 아이템 데이터
    public List<int> deck = new List<int>(); // 현재 회득 아이템
    public Transform[] Points; // 아이템 생성 위치
    public Item[] items;  //아이템 
    
     public void SpawnItems_()
    {
        StartCoroutine(spawnItem());
    }
    IEnumerator spawnItem(){
         List<ItemData> availableItems = GetAvailableItems(); // 사용 가능한 아이템 필터링

        if (availableItems.Count < Points.Length)
        {
            Debug.LogWarning("생성할 수 있는 아이템의 개수가 부족합니다.");
            yield break;
        }

        // 아이템 위치에 생성
        for (int i = 0; i < Points.Length; i++)
        {
            // 랜덤 아이템 선택
            int randomIndex = Random.Range(0, availableItems.Count);
            ItemData selectedItem = availableItems[randomIndex];

            // 아이템 생성
            items[i] = CreateItem(selectedItem, Points[i].position);

            // 덱에 추가
            deck.Add(selectedItem.ItemID);

            // 선택된 아이템은 다시 등장하지 않도록 리스트에서 제거
            availableItems.RemoveAt(randomIndex);

            //0.5초마다 아이템 생성
            yield return new WaitForSeconds(0.5f);
        }
    }

    private List<ItemData> GetAvailableItems()
    {
        List<ItemData> availableItems = new List<ItemData>();

        // 덱에 없는 아이템만 선택
        foreach (var itemData in itemDatas)
        {
            if (!deck.Contains(itemData.ItemID))
            {
                availableItems.Add(itemData);
            }
        }

        return availableItems;
    }

    private Item CreateItem(ItemData data, Vector3 position)
    {
        // 프리팹이 연결되어 있다고 가정하고 Instantiate로 생성
        int poolingNumber = 4;
        GameObject itemPrefab = GameManager.instance.poolManager.Get(poolingNumber); // 예시로 Object Pool 사용
        Item item = itemPrefab.GetComponent<Item>();
        item.Init(data);

        // 생성 위치 설정
        item.transform.position = position;

        return item;
    }
}
