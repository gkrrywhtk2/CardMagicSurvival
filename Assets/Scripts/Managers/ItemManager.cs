using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using RANK;

public class ItemManager : MonoBehaviour
{

    public ItemData[] itemDatas_normal;         // 아이템 데이터 노말
    public ItemData[] itemDatas_rare;         // 아이템 데이터 희귀
    public ItemData[] itemDatas_epic;         // 아이템 데이터 영웅
    public ItemData[] itemDatas_legend;         // 아이템 데이터 전설
    public List<int> deck = new List<int>(); // 현재 회득 아이템
    public Transform[] Points; // 아이템 생성 위치
    public Item[] items;  //아이템 
    
     public void SpawnItems_(Rank rank)
    {
        StartCoroutine(spawnItem(rank));
    }
    IEnumerator spawnItem(Rank rank){
         List<ItemData> availableItems = GetAvailableItems(rank); // 사용 가능한 아이템 필터링
        Vector3[] positon_0 = new Vector3[Points.Length];
        for (int i = 0; i < Points.Length; i++)
        {
        positon_0[i] = Points[i].position;
        }
        if (availableItems.Count < Points.Length)
        {
            Debug.LogWarning("생성할 수 있는 아이템의 개수가 부족합니다.");
            yield break;
        }

        // 아이템 위치에 생성
        for (int i = 0; i < Points.Length; i++)
        {
            //0.5초마다 아이템 생성
            yield return new WaitForSeconds(0.5f);

            // 랜덤 아이템 선택
            int randomIndex = Random.Range(0, availableItems.Count);
            ItemData selectedItem = availableItems[randomIndex];

            // 아이템 생성
            items[i].gameObject.SetActive(true);
            items[i].Init(selectedItem);
            items[i].transform.position = positon_0[i];
           
            //CreateItem(selectedItem, Points[i].position);

            // 덱에 추가는 아이템 선택 후 사용
            //deck.Add(selectedItem.ItemID);

            //생성된 아이템은 다시 등장하지 않도록 리스트에서 제거
            availableItems.RemoveAt(randomIndex);

           
        }
    }

    private List<ItemData> GetAvailableItems(Rank rank)
    {
        List<ItemData> availableItems = new List<ItemData>();

        switch(rank){
            case Rank.normal:
                foreach (var itemData in itemDatas_normal)
            {
                if (!deck.Contains(itemData.ItemID))
                {
                    availableItems.Add(itemData);
                }
            }
            break;
            case Rank.rare:
                foreach (var itemData in itemDatas_rare)
            {
                if (!deck.Contains(itemData.ItemID))
                {
                    availableItems.Add(itemData);
                }
            }
            break;
            case Rank.epic:
                foreach (var itemData in itemDatas_epic)
            {
                if (!deck.Contains(itemData.ItemID))
                {
                    availableItems.Add(itemData);
                }
            }
            break;
            case Rank.legend:
                foreach (var itemData in itemDatas_legend)
            {
                if (!deck.Contains(itemData.ItemID))
                {
                    availableItems.Add(itemData);
                }
            }
            break;
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

    public void ItemSelected(int ItemID, int originid){
            GameManager.instance.player.playerEffect.levelUpCircleTimeStop.transform.position = GameManager.instance.player.dirFront.playerTransform.position;
            GameManager.instance.player.playerEffect.levelUpCircleTimeStopAnim.SetTrigger("Over");
            deck.Add(ItemID);
            items[0].gameObject.SetActive(false);
            items[1].gameObject.SetActive(false);
            items[2].gameObject.SetActive(false);
            GameManager.instance.NextWave(ItemID, originid);
    }
}
