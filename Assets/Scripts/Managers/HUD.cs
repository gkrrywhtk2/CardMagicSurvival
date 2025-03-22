using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    public enum InfoType {gold, ruby}
    public InfoType type;
    private TMP_Text thisText;
    public DataManager dataManager;


    private void Awake()
    {
        thisText = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        dataManager = GameManager.instance.dataManager;
    }
    private void LateUpdate()
    {
        switch (type)
        {
            case InfoType.gold:
                this.thisText.text = dataManager.goldPoint.ToString();
                break;
            case InfoType.ruby:
                this.thisText.text = dataManager.rubyPoint.ToString();
                break;
        }
    }
}
