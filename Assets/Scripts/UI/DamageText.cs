using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
  public float value;//데미지 수치
  public TMP_Text damageText;

  public void Init(){
    //데미지를 자연수로 표현
    damageText.text = Mathf.RoundToInt(value).ToString();
  }
}
