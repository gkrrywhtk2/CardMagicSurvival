using Game.RankSystem;
using UnityEngine;
using UnityEngine.UI;

public class SelectCard : MonoBehaviour
{
    public int index;//123 인덱스
    public Image focus;
    public CardImage cardImage;
    public int cardId;//현재 저장되어있는 카드의 아이디
    public RankType rank;

    public void Init(int id, RankType ranktype)
    {
        cardId = id;
        rank = ranktype;
    }
    

    public void OnFocusCard()
    {
        focus.gameObject.SetActive(true);
    }
    public void OffFocusCard()
    {
        focus.gameObject.SetActive(false);
    }

}
