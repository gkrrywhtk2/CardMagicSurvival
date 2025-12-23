using UnityEngine;
using Game.RankSystem;


public class Meteor : MonoBehaviour
{
      public Vector3 targetPosition; // 낙하할 위치
      public float speed = 10f;      // 낙하 속도
  //  public GameObject impactEffect; // 충돌 시 이펙트 Prefab
      private bool isMoving = false; // 이동 상태 확인
      private int STACK;
      private RankType rank;
       
    //private Quaternion initialRotation;
    public void Init(Vector3 target, RankType rank)
    {
        targetPosition = target;
        this.rank = rank;
        isMoving = true; // 이동 시작
    }
     private void Update()
    {
        if (isMoving)
        {
            // 목표 지점으로 이동
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // 목표에 도착한 경우
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false; // 이동 중지
                
                CreateImpactArea();        // 충돌 효과 실행
            }
        }
    }
   

    public void CreateImpactArea(){
    int poolNumber = 3;
    GameObject impactArea = GameManager.instance.effectPoolManager.Get(poolNumber);
    impactArea.transform.position = targetPosition;
    impactArea.transform.localScale = GetDurationByRank(rank);

    float magicPower = GetDamageByRank(rank);
    float damage = GameManager.instance.player.playerStatus.DamageReturn(magicPower,out bool isCritical);
    impactArea.GetComponent<Bullet_ExplosionNormal>().Init(damage, isCritical);

    gameObject.SetActive(false);
   }

    private Vector3 GetDurationByRank(RankType rank)
    {
        switch (rank)
        {
            case RankType.Uncommon:   return  new Vector3(2f, 2f, 1); //임시;
            case RankType.Rare:       return new Vector3(2.5f, 2.5f, 1); //임시;;
            case RankType.Epic:       return new Vector3(4f, 4f, 1); //임시;;
            case RankType.Legendary:  return new Vector3(5, 5, 1); //임시;;
            default:                 return new Vector3(1, 1, 1); //임시;;
        }
    }
     private float GetDamageByRank(RankType rank)
    {
        switch (rank)
        {
            case RankType.Uncommon:   return  10;
            case RankType.Rare:       return 20;
            case RankType.Epic:       return 30;
            case RankType.Legendary:  return 40;
            default:                 return 10;
        }
    }
}
