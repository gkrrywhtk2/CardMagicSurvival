using UnityEngine;

public class Player_Effect : MonoBehaviour
{
    public GameObject manaUp; 
    public GameObject manaUp2;
    public GameObject ConcentEffect0;
    public GameObject ConcentEffect1;
    public GameObject levelUpCircleTimeStop;
    public Animator levelUpCircleTimeStopAnim;
    public Animator NextStagePadeOut;
    //
    //Card6
    public GameObject card6_Effect0;
    public GameObject card6_Effect1;


    public void PlayManaUp()
    {
      manaUp.gameObject.SetActive(true);
      manaUp2.gameObject.SetActive(true);
    }

    public void HasteEffect(){
      ConcentEffect0.gameObject.SetActive(true);
      ConcentEffect1.gameObject.SetActive(true);
    }
    
}
