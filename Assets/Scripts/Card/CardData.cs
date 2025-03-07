using UnityEngine;
[CreateAssetMenu(fileName = "Card", menuName = "Scriptable Object/CardData")]

public class CardData : ScriptableObject
{
  public enum CardRank{normal, rare, epic, legend};
  public enum InfoTag{damage, count, duration, range, manarecovery, speedUp, Heal}
  [Header("#Main Info")]
  public int cardId;
  public int cardCost;
  public CardRank rank;
  public InfoTag[] infoTags;
  public string cardName;
  public string cardDesc_Main;
  public Sprite cardImage;
  public bool isRangeCard;
  public bool isDirCard;//방향 벡터가 필요한 카드인지? ex) 화염구
  public Vector3 rangeScale_Card;
  public Sprite nextcardImage;

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

      [Header("# Range Info")]//범위 정보
        public Vector3 rangeScale_;
        public float growthValue_Range;//성장 계수
         public Vector3 GetRange(int stack)
{
    // rangeScale_.x와 rangeScale_.y에 성장 계수를 곱해서 증가시킨 값으로 새로운 Vector3를 반환
        return new Vector3(
        rangeScale_.x + (growthValue_Range * stack), // x축 값 증가
        rangeScale_.y + (growthValue_Range * stack), // y축 값 증가
        0 // z축 값은 고정
    );
}
      public float GetRangeForUser(int stack)
    {
        return Mathf.Round(GetRange(stack).x * 10) / 10f; // 소수점 한 자리로 고정
      }

       [Header("# Mana Info")]//마나 정보
      public int baseManaRecovery;// 기본 마나 회복량
    public int growthValue_ManaRecovery;//성장 계수

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
