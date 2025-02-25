using UnityEngine;

public class RetrunIdle : MonoBehaviour
{
    public RandomCard Card;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void Awake() {
        
    }

    public void ImageSetting(){
        Card.ImageSetting();
    }
     public void AutoSelect(){
        //처음 생성될때 0번째 랜덤 카드 자동 선택
      //  int index = 0;//첫번째 randomCard
       // GameManager.instance.deckManager.RandomCardSelectedSetting(index);
    }
}
