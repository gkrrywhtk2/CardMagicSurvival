using UnityEngine;

public class EXP_GEM : MonoBehaviour
{
   public float value;//해당 경험치 구슬의 상승량
   private void Awake() {
      value = 100;
   }

   private void OnTriggerEnter2D(Collider2D other) {
    if(other.gameObject.CompareTag("Player")) {
      GameManager.instance.player.playerStatus.playerEXP.AddExp(value); //value 만큼 경험치량 증가
      gameObject.SetActive(false);
    }
   }
}
