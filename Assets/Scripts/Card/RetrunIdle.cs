using UnityEngine;

public class RetrunIdle : MonoBehaviour
{
    private RandomCard Card;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void Awake() {
        Card = GetComponentInParent<RandomCard>();
    }

    public void ImageSetting(){
        Card.ImageSetting();
    }
}
