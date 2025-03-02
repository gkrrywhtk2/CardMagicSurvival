using UnityEngine;


public class Meteor : MonoBehaviour
{
      public Vector3 targetPosition; // 낙하할 위치
      public float speed = 10f;      // 낙하 속도
  //  public GameObject impactEffect; // 충돌 시 이펙트 Prefab
      private bool isMoving = false; // 이동 상태 확인
      private int ID = 1;
      private int STACK;
       
    //private Quaternion initialRotation;
    public void Init(Vector3 target, int stack)
    {
        targetPosition = target;
        isMoving = true; // 이동 시작
        STACK = stack;
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
    
    //Vector3 impactRange; 스택에 따른 범위 증가 삭제
   //impactRange = GameManager.instance.deckManager.cardDatas[ID].GetRange(STACK);
    impactArea.transform.position = targetPosition;
    impactArea.transform.localScale = GameManager.instance.deckManager.cardDatas[ID].rangeScale_;
   // Debug.Log(STACK);
    // Debug.Log(impactRange);
    //  Debug.Log("Impact Area Scale: " + impactArea.transform.localScale);

    //화염구 충돌 오브젝트 초기화
    float magicPower = GameManager.instance.deckManager.cardDatas[ID].GetDamage(STACK);
    float damage = GameManager.instance.player.playerStatus.DamageReturn(magicPower,out bool isCritical);
    impactArea.GetComponent<Melee>().Init(damage, isCritical);
   
  
    gameObject.SetActive(false);
   }
}
