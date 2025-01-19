using UnityEngine;
[CreateAssetMenu(fileName = "Card", menuName = "Scriptable Object/CardData")]

public class CardData : ScriptableObject
{
  public enum CardRank{normal, rare, epic, legend};
  [Header("#Main Info")]
  public int cardId;
  public int cardCost;
  public CardRank rank;
  public string cardName;
  public string cardDescLv1;
    public string cardDescLv2;
      public string cardDescLv3;
  public Sprite cardImage;
  public bool isRangeCard;
  public Vector3 rangeScale;
  public Sprite nextcardImage;

}
