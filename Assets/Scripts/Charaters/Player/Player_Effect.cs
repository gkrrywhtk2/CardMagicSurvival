using UnityEngine;

public class Player_Effect : MonoBehaviour
{
    public GameObject manaUp; 
    public GameObject manaUp2;

    public void PlayManaUp()
    {
      manaUp.gameObject.SetActive(true);
      manaUp2.gameObject.SetActive(true);
    }
}
