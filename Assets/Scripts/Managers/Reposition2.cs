using UnityEngine;

public class Reposition2 : MonoBehaviour
{
 [SerializeField] private float moveDistance = 40f; // 이동 거리 설정

    void OnTriggerExit2D(Collider2D collision)
   {
       Debug.Log("Trigger Exit Detected with: " + collision.tag);

       // Area 태그 체크 및 오타 확인
       if (collision.tag == "Area")
       {
           Debug.Log("Area 태그가 감지되었습니다.");
       }
       else if (collision.tag.ToLower().Contains("area"))  // area로 시작하는 비슷한 태그를 찾음
       {
           Debug.LogWarning($"'Area' 태그가 '{collision.tag}'로 잘못 입력되었습니다. 태그를 'Area'로 수정해주세요!");
           return;
       }
       else
       {
           Debug.Log("Area 태그가 아닙니다");
           return;
       }

       if (GameManager.instance == null)
       {
           Debug.LogError("GameManager instance가 null입니다");
           return;
       }

       if (GameManager.instance.player == null)
       {
           Debug.LogError("Player reference가 null입니다");
           return;
       }

       Vector3 playerPos = GameManager.instance.player.transform.position;
       Vector3 myPos = transform.position;
       float dirX = playerPos.x - myPos.x;
       float dirY = playerPos.y - myPos.y;
       float diffX = Mathf.Abs(dirX);
       float diffY = Mathf.Abs(dirY);
       dirX = dirX > 0 ? 1 : -1;
       dirY = dirY > 0 ? 1 : -1;
       
       // Ground 태그 체크 및 오타 확인 
       switch (transform.tag) { 
           case "Ground":
               Debug.Log("Ground 태그가 감지되었습니다.");
               if (diffX > diffY) {
                   transform.Translate(Vector3.right * dirX * 40);
               }
               else if (diffX < diffY) {
                   transform.Translate(Vector3.up * dirY * 40);
               }
               break;
           case var tag when tag.ToLower().Contains("ground"):  // ground로 시작하는 비슷한 태그를 찾음
               Debug.LogWarning($"'Ground' 태그가 '{transform.tag}'로 잘못 입력되었습니다. 태그를 'Ground'로 수정해주세요!");
               if (diffX > diffY) {
                   transform.Translate(Vector3.right * dirX * 40);
               }
               else if (diffX < diffY) {
                   transform.Translate(Vector3.up * dirY * 40);
               }
               break;
           case "Enemy":
               break;
           default:
               Debug.LogWarning("Ground나 Enemy 태그가 아닙니다.");
               break;
       }
   }
}
