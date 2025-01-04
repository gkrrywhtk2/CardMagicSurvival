using UnityEngine;

public class effectDestroy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void delete(){
        gameObject.SetActive(false);
    }
}
