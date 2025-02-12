using UnityEngine;

public class GoldCoin : MonoBehaviour
{
   public float value;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Cleaner")){
            gameObject.SetActive(false);//다음 스테이지 진입시 클리너 발동, 몬스터 전부 삭제 
        }
    }
}
