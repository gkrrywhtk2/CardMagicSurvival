using UnityEngine;
using UnityEngine.UI;

public class DamageText_False : MonoBehaviour
{
   private GameObject parent;
private void Awake() {
    if (transform.parent != null)
    {
        parent = transform.parent.gameObject;
    }
    }

    public void Set_False(){
        parent.gameObject.SetActive(false);
    }
}
