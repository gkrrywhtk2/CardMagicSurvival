using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
  public float value;//데미지 수치
  Animator anim;
  public TMP_Text damageText;

private void Awake() {
  anim = damageText.GetComponent<Animator>();
}

  public void Init(bool isCritical){
    //데미지를 자연수로 표현
    damageText.text = Mathf.RoundToInt(value).ToString();
    DisplayDamage(isCritical);
  }

 public void DisplayDamage(bool isCritical)
{
    if (isCritical)
    {
        anim.SetBool("IsCritical", true);
    }
    else
    {
         anim.SetBool("IsCritical", false);
    }
}

}
