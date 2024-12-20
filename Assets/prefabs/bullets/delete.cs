using UnityEngine;

public class delete : MonoBehaviour
{
    public GameObject mainOB;
    private void Awake() {
       
    }
 public void destroy(){
    mainOB.SetActive(false);
 }
}
