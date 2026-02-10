using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellCardAccountImage : MonoBehaviour
{
    public Slider slider;
    public Image sliderFill;
    public TMP_Text Text_Slider; // N/M 형식
    public GameObject Uparrow;
    public Color blueColor;
    public Color greenColor;

        public void Init(int id)
    {
        int stock = ServerDataManager.instance.GetCardStock(id);
        int cardLevel = ServerDataManager.instance.GetCardLevel(id);
        int require = AccountCardManager.Instance.GetRequiredCardsForLevelUp(cardLevel);

        require = Mathf.Max(1, require);

        // ✅ 슬라이더를 N/M로 직접 사용
        slider.minValue = 0f;
        slider.maxValue = require;
        slider.wholeNumbers = true; // (선택) 카드 개수는 정수라면 켜도 깔끔

        slider.value = Mathf.Clamp(stock, 0, require);

        Text_Slider.text = $"{stock}/{require}";
        bool isUpgradeable = stock >= require;
        sliderFill.color = isUpgradeable ? greenColor : blueColor;
        Uparrow.SetActive(isUpgradeable);
    }

}
