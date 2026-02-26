using Game.RankSystem;
using UnityEngine;
using UnityEngine.Localization; // 추가
using UnityEngine.Localization.SmartFormat.PersistentVariables; // 추가
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
  public LocalizedString locallizedName;
  public LocalizedString localizedDesc_Sub;

  public LocalizedString localizedDesc_Main;

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

      public string GetName()
    {
      // 연결된 키가 없거나 비어있으면 빈 문자열 혹은 기본 메시지 반환
          if (locallizedName == null || locallizedName.IsEmpty) return "";

          // 변수가 없으므로 바로 GetLocalizedString 호출
          return locallizedName.GetLocalizedString();
    }

      // Sub 설명(순수 텍스트)을 반환하는 함수
    public string GetParsedSubDescription()
    {
        // 연결된 키가 없거나 비어있으면 빈 문자열 혹은 기본 메시지 반환
        if (localizedDesc_Sub == null || localizedDesc_Sub.IsEmpty) return "";

        // 변수가 없으므로 바로 GetLocalizedString 호출
        return localizedDesc_Sub.GetLocalizedString();
    }


        // 2. 레벨(stack)을 넣으면 최종 번역문을 반환하는 함수
   private const string LEVEL_COLOR = "#FFA500"; // 주황색

  public string GetParsedDescription(int stack)
  {
      if (localizedDesc_Main == null || localizedDesc_Main.IsEmpty)
      {
          Debug.LogError($"{name} 에셋의 LocalizedDesc_Main에 키가 할당되지 않았습니다!");
          return "설명 없음";
      }

      // 레벨에 따라 변하는 값들 -> 주황색 문자열로 주입
      SetColoredIntVariable("dmg", GetDamage(stack));
      SetColoredIntVariable("cnt", GetCount(stack));
      SetColoredIntVariable("heal", GetHeal(stack));
      SetColoredIntVariable("mana", GetManaRecovery(stack));

      SetColoredFloatVariable("dur", GetDuration(stack));
      SetColoredFloatVariable("spd", GetSpeedUp(stack));
      SetColoredFloatVariable("range", hitRange.x);

      return localizedDesc_Main.GetLocalizedString();
  }

  private void SetColoredIntVariable(string key, float value)
  {
      int v = Mathf.RoundToInt(value);
      localizedDesc_Main[key] = new StringVariable
      {
          Value = $"<color={LEVEL_COLOR}>{v}</color>"
      };
  }

  private void SetColoredFloatVariable(string key, float value)
  {
      float v = Mathf.Round(value * 10f) * 0.1f;
      localizedDesc_Main[key] = new StringVariable
      {
          Value = $"<color={LEVEL_COLOR}>{v:0.0}</color>"
      };
  }


}
