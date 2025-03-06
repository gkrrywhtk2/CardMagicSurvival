using UnityEngine.UI;
using UnityEngine;

public class Icon_MagicCard : MonoBehaviour
{
    public int iconID; // 해당 아이콘의 번호
    public float duration; // 설정된 지속시간
    public Image iconImage;
    public Image durationImage;
    private float usedTime;
    IconManager iconManager;


    public void Init(int ID, float time)
    {
        iconID = ID;
        iconImage.sprite = GameManager.instance.deckManager.cardDatas[iconID].cardImage;
        duration = time;
        usedTime = 0;
        durationImage.fillAmount = 0;
        iconManager = GameManager.instance.iconManager;
    }

    private void Update()
    {
        if (!GameManager.instance.GamePlayState)
            return;

        usedTime += Time.deltaTime;
        usedTime = Mathf.Min(usedTime, duration);

        FillSetting();
    }

    public void FillSetting()
    {
        durationImage.fillAmount = Mathf.Clamp01(usedTime / duration);

        if (usedTime >= duration)
        {
            iconManager?.RemoveIcon(iconID); // 아이콘이 사라질 때 IconManager에 알림
            gameObject.SetActive(false);
        }
    }
}
