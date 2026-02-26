using Game.CardData;
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

        var data =  CardData.Instance.cardScritableData[1];

        // ✅ 크기는 고정
        impactArea.transform.localScale = ImpactScale;

        // ✅ 레벨 기반 데미지
        float magicPower = data.GetDamage(LEVEL);
        float damage = GameManager.instance.player.playerStatus.DamageReturn(magicPower, out bool isCritical);
        impactArea.GetComponent<Bullet_ExplosionNormal>().Init(damage, isCritical);

        gameObject.SetActive(false);
    }
}
