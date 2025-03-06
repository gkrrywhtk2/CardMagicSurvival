using UnityEngine;

public class Dir_Front : MonoBehaviour
{
    public Transform playerTransform; // 플레이어의 Transform
    public float skillOffset = 0.5f;  // 플레이어로부터 얼마나 떨어질지

    private Vector2 direction; // 이동 방향 벡터
    private Vector2 previousPosition; // 이전 프레임의 플레이어 위치
    public Vector2 skillPosition; // 스킬이 연출될 좌표
    public float angle;
    
    void Start()
    {
        previousPosition = playerTransform.position; // 초기 위치 저장
    }

    void Update()
    {
        Vector2 currentPosition = playerTransform.position;
        
        // 🔹 이동한 경우에만 업데이트
        if ((currentPosition - previousPosition).sqrMagnitude > 0.0001f) // (0.01f)^2보다 크면 이동 감지
        {
            direction = (currentPosition - previousPosition).normalized;

            // 2. 스킬 위치 설정
            skillPosition = currentPosition + direction * skillOffset;
            transform.position = skillPosition;

            // 3. 스킬 회전 설정
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // 🔹 이전 위치 갱신
            previousPosition = currentPosition;
        }
    }
}
