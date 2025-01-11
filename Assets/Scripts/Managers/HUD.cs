using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
     public enum InfoType {Time, Wave, Mobcount}
    public InfoType type;


    private TMP_Text thisText;


    private void Awake()
    {
        thisText = GetComponent<TMP_Text>();
    }

    private void Start()
    {

    }
    private void LateUpdate()
    {
        switch (type)
        {
            case InfoType.Time:
            if(GameManager.instance.GamePlayState != true)
                return;
            if(GameManager.instance.waveManager.waveTimeOver != false)
                return;
                float remainTime = GameManager.instance.waveManager.waveGameTime;
                int min = Mathf.FloorToInt(remainTime / 60);
                int sec = Mathf.FloorToInt(remainTime % 60);
                thisText.text = string.Format("{0:D2} : {1:D2}", min, sec);

                break;
            case InfoType.Wave:
                float nowWaveLevel = GameManager.instance.waveManager.nowWave;
                thisText.text = nowWaveLevel.ToString();
                break;
            case InfoType.Mobcount:
                thisText.text = GameManager.instance.spawnManager.mobCount.ToString();
                break;
      
        }
    }
}
