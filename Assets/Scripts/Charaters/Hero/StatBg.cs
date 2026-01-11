using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatBg : MonoBehaviour
{
    public TMP_Text value;
    public TMP_Text value_next;
    public Image backGround;
    public Color green_Color;
    public Color black_Color;
    public Image arrow;
    public enum Type
    {
        ATK, Hp, MoveSpeed, CriChance, CriDamage
    }
    public Type type;

    public void Init(int current, int next, bool lvupready)
    {
        value.text = current.ToString();
        value_next.text = next.ToString();

        bool isIncrease = current != next; // 수정
        UpdateVisual(lvupready, isIncrease);
    }

    public void Init(float current, float next, bool lvupready)
    {
        switch (type)
        {
            case Type.Hp:
                value.text = current.ToString("F0");
                value_next.text = next.ToString("F0");
                break;
            case Type.MoveSpeed:
                value.text = current.ToString("F1");
                value_next.text = next.ToString("F1");
                break;
            case Type.CriChance:
                value.text = (current * 100f).ToString("F1") + "%";
                value_next.text = (next * 100f).ToString("F1") + "%";
                break;
            case Type.CriDamage:
                value.text = (current * 100f).ToString("F0") + "%";
                value_next.text = (next * 100f).ToString("F0") + "%";
                break;
        }
        bool isIncrease = current != next; // 수정
        UpdateVisual(lvupready, isIncrease);
    }

    public void UpdateVisual(bool lvupready, bool increase)
    {
        //초기화
        backGround.color = black_Color;
        value_next.gameObject.SetActive(false);
        arrow.gameObject.SetActive(false);
        
        if(!lvupready) // 간결하게 수정
            return;
            
        backGround.color = increase ? green_Color : black_Color;
        value_next.gameObject.SetActive(increase);
        arrow.gameObject.SetActive(increase);
    }
}