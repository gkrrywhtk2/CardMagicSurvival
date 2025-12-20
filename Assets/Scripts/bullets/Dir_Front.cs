using UnityEngine;

public class Dir_Front : MonoBehaviour
{
    [Header("Refs")]
    public Transform playerTransform;     // 플레이어 Transform
    public JoyStick_P joystickP;          // JoyStick_P 참조 (인스펙터에 연결)

    [Header("Settings")]
    public float skillOffset = 10f;
    public float inputThreshold = 0.01f;  // 입력 감지 임계값

    private Vector2 lastInputDir = Vector2.right; // 입력 없을 때 유지할 마지막 방향
    public Vector2 skillPosition;
    public float angle;

    private void Reset()
    {
        // 같은 오브젝트/부모에서 자동 연결 시도(선택)
        if (playerTransform == null) playerTransform = GameObject.FindWithTag("Player")?.transform;
        if (joystickP == null && playerTransform != null) joystickP = playerTransform.GetComponent<JoyStick_P>();
    }

    void Update()
    {
        if (playerTransform == null || joystickP == null) return;

        // ✅ "조작 중일 때만" 방향 갱신 (몬스터 밀림은 inputVec=0이라 무시됨)
        Vector2 input = joystickP.inputVec;
        if (input.sqrMagnitude > inputThreshold * inputThreshold)
        {
            lastInputDir = input.normalized;
        }

        // ✅ 위치는 항상 플레이어 기준으로 따라감 (밀려도 따라가되 방향은 유지)
        Vector2 playerPos = playerTransform.position;
        skillPosition = playerPos + lastInputDir * skillOffset;
        transform.position = skillPosition;

        // ✅ 회전도 lastInputDir 기준
        angle = Mathf.Atan2(lastInputDir.y, lastInputDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
