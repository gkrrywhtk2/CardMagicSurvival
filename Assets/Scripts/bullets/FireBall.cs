using UnityEngine;
using UnityEngine.Analytics;

public class FireBall : MonoBehaviour
{
      public Vector3 targetPosition; // 낙하할 위치
    public float speed = 10f;      // 낙하 속도
  //  public GameObject impactEffect; // 충돌 시 이펙트 Prefab
      private bool isMoving = false; // 이동 상태 확인
    //private Quaternion initialRotation;
    private void Start()
    {
       
    }
    public void Initialize(Vector3 target)
    {
        targetPosition = target;
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
                
                Explode();        // 충돌 효과 실행
            }
        }
    }
   

   public void Explode(){
    int effectNum = 3;
    GameObject effect = GameManager.instance.effectPoolManager.Get(effectNum);
    effect.transform.position = targetPosition;

    //화염구 충돌 오브젝트 초기화
    float damage = 20;
    int per = -2;//infinity
    float bulletspeed = 0;//null
    Vector3 dir = Vector3.zero;//null
    int effectNumber = -1;//null
     global::bullet.bulletType type = global::bullet.bulletType.bullet;
    effect.GetComponent<bullet>().Init(damage, per, bulletspeed, dir,effectNumber,type);
    gameObject.SetActive(false);

   }
}
