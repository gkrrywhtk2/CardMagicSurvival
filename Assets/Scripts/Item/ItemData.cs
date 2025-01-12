using UnityEngine;
using RANK;
[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object/ItemData")]

public class ItemData : ScriptableObject    
{

  [Header("#Main Info")]
    public int ItemID;
   
    public Rank rank;
    public string ItemName;
    public string ItemDesc;
    public Sprite ItemSprite;
}
    namespace RANK
    {
            public enum Rank{normal, rare, epic, legend};
    }
    

