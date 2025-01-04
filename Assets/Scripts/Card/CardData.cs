using UnityEngine;
[CreateAssetMenu(fileName = "Card", menuName = "Scriptable Object/CardData")]

public class CardData : ScriptableObject
{
  [Header("#Main Info")]
  public int cardId;
  public int cardLevel;
  public int cardCost;
  public string cardName;
  public string cardDesc;
  public Sprite cardImage;
  public bool isRangeCard;
  public Vector3 rangeScale;

}
