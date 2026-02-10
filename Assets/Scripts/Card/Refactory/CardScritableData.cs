using Game.RankSystem;
using UnityEngine;
[CreateAssetMenu(fileName = "Card", menuName = "Scriptable Object/CardData")]

public class CardScritableData : ScriptableObject
{
  [Header("#Main Info")]
  public int cardId;
  public int cardCost;
  public RankType rank;
  public string cardName;
  public string cardDesc_Main;
  public Sprite cardImage;
  public bool isRangeCard;
  public bool isDirCard;//방향 벡터가 필요한 카드인지? ex) 화염구
  public Sprite nextcardImage;
  public Vector3 hitRange;

  [Header("# Damage Info")]
    public float baseDamage; // 기본 공격력
    public float growthValue; // 성장 계수

    public float GetDamage(int stack)
      {
          return baseDamage + (growthValue * stack);
      }
    
    [Header("# Count Info")]//공격 횟수 정보
    public int baseCount;// 기본 공격 횟수
    public int growthValue_Count;//성장 계수

    public int GetCount(int stack)
      {
          return baseCount + (growthValue_Count * stack);
      }

      [Header("# Duration Info")]//지속 시간 정보
    public float baseDuration;// 기본 지속시간
    public float growthValue_Duration;//성장 계수

    public float GetDuration(int stack)
      {
          return baseDuration + (growthValue_Duration * stack);
      }

      [Header("# Mana Info")]//마나 정보
      public float baseManaRecovery;// 기본 마나 회복량
    public float growthValue_ManaRecovery;//성장 계수

    public float GetManaRecovery(int stack)
      {
          return baseManaRecovery + (growthValue_ManaRecovery * stack);
      }

      [Header("#Haste Info")]//추가 신속 정보
      public float baseSpeedUp;// 기본 마나 회복량
      public float growthValue_baseSpeedUp;//성장 계수

    public float GetSpeedUp(int stack)
      {
          return baseSpeedUp + (growthValue_baseSpeedUp * stack);
      }

       [Header("#Heal Info")]//추가 신속 정보
      public float heal;// 기본 마나 회복량
      public float growthValue_heal;//성장 계수

    public float GetHeal(float stack)
      {
          return heal + (growthValue_heal * stack);
      }


}
