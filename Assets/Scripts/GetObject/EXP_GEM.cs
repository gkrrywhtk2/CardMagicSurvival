using UnityEngine;

public class EXP_GEM : MonoBehaviour
{
   public float point;
   private void Awake() {
    point = 10;
   }

   private void OnTriggerEnter2D(Collider2D other) {
    if(other.gameObject.CompareTag("Player")) {
      //  GameManager.instance.player.playerStatus.nowexp += point;
        gameObject.SetActive(false);
    }
   }
}
