using UnityEngine;

public class Meteor : MonoBehaviour
{
    public Vector3 targetPosition; // 낙하할 위치
    public float speed = 10f;      // 낙하 속도

    private bool isMoving = false; // 이동 상태 확인
    private int STACK;
    private int LEVEL;

    // ✅ 고정 크기
    private static readonly Vector3 ImpactScale = new Vector3(3f, 3f, 1f);

    public void Init(Vector3 target, int level)
    {
        targetPosition = target;
        LEVEL = level;
        STACK = level; // 현재 로직 유지(필요 없으면 삭제 가능)
        isMoving = true; // 이동 시작
    }

    private void Update()
    {
        if (!isMoving) return;

        // 목표 지점으로 이동
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // 목표에 도착한 경우
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isMoving = false;
            CreateImpactArea();
        }
    }

    public void CreateImpactArea()
    {
        int poolNumber = 3;
        GameObject impactArea = GameManager.instance.effectPoolManager.Get(poolNumber);
        impactArea.transform.position = targetPosition;

        // ✅ 크기는 고정
        impactArea.transform.localScale = ImpactScale;

        // ✅ 레벨 기반 데미지
        float magicPower = GetDamageByLevel(LEVEL);
        float damage = GameManager.instance.player.playerStatus.DamageReturn(magicPower, out bool isCritical);
        impactArea.GetComponent<Bullet_ExplosionNormal>().Init(damage, isCritical);

        gameObject.SetActive(false);
    }

    /// <summary>
    /// ✅ 레벨 기반 데미지 공식
    /// - Lv1  : 10
    /// - Lv25 : 40
    /// 선형 증가: 10 + (L-1) * (30/24) = 10 + (L-1)*1.25
    /// </summary>
    private float GetDamageByLevel(int level)
    {
        level = Mathf.Clamp(level, 1, 99);
        return 10f + (level - 1) * 1.25f; // Lv25 -> 40
    }
}
