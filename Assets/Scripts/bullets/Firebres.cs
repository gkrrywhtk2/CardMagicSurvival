using UnityEngine;

public class Firebres : MonoBehaviour
{
    Scaner scaner;

     public Transform playerTransform; // 플레이어의 Transform
    public float skillOffset = 0.5f;  // 플레이어로부터 얼마나 떨어질지

    private Vector2 direction; // 스킬 방향 벡터

    private void Awake() {
        scaner = GetComponentInParent<Scaner>();
    }

    void Update()
    {
        // 1. 방향 벡터 계산 (플레이어의 바라보는 방향)
        direction = GameManager.instance.player.playerJoyStick.inputVec;
        if (direction.sqrMagnitude > 0.1f) // 입력이 있는 경우에만 갱신
        {
            direction = direction.normalized;

            // 2. 스킬 위치 설정
            Vector2 skillPosition = (Vector2)playerTransform.position + direction * skillOffset;
            transform.position = skillPosition;

            // 3. 스킬 회전 설정
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

   
}
