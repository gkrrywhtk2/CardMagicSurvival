using UnityEngine;

public class DIr_FrontForCard : MonoBehaviour
{
    public Transform playerTransform; // 플레이어의 Transform
    public float skillOffset = 0.5f;  // 플레이어로부터 얼마나 떨어질지

    private Vector2 direction; // 방향 벡터
    public Vector2 thisPosition; // 스킬이 연출될 좌표
    public float angle;


    public void ArrowDirSetting(Vector2 cardVec){
        //화살표 방향 세팅
        Vector2 playerVec = playerTransform.position;

          // 2.위치 설정
            thisPosition = playerVec + direction * skillOffset;
            transform.position = thisPosition;

        direction = (cardVec - playerVec).normalized;

        //회전 설정
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);


    }
}
