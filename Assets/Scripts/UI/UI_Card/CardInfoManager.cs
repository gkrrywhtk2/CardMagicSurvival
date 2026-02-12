using Game.CardData;
using TMPro;
using UnityEngine;

public class CardInfoManager : MonoBehaviour
{
    public TMP_Text desc_sub;
    public TMP_Text desc_main;

    public void Init(int cardId, int currentLevel)
    {
        InitDescText(cardId, currentLevel);
    }
    public void InitDescText(int cardId, int currentLevel)
    {
        var data = CardData.Instance.cardScritableData[cardId];
        if (data == null)
        {
            Debug.LogError($"CardData with ID {cardId} not found!");
            return;
        }

        // 메인 설명 갱신 (변수 포함)
        desc_main.text = data.GetParsedDescription(currentLevel);

        // ✅ 서브 설명 갱신 (순수 텍스트)
        if (desc_sub != null)
        {
            desc_sub.text = data.GetParsedSubDescription();
        }
    }
}
