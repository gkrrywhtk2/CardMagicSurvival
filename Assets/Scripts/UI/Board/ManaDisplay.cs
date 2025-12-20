using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManaDisplay : MonoBehaviour
{
    public Player_Status player;
    public Slider manaBar;
    public TMP_Text manaText;
    private const string maxManaColor = "#ffa3ef";



    private void Update()
    {
        if (player.playerHP.isLive != true)
            return;

        ManaBarUpdate();
    }
    private void ManaBarUpdate()
    {
        float mana = player.playerMana.mana;
        float maxMana = player.playerMana.maxMana;
        manaBar.value = mana / maxMana;

        //아래는 텍스트
        int manaInt = Mathf.FloorToInt(mana); // 정수로 변환
        manaText.text = $"{manaInt} <color={maxManaColor}>/ {Mathf.FloorToInt(player.playerMana.maxMana)}</color>";
    }

    
}
