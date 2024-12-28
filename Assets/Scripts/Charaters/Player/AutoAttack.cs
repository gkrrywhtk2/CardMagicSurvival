using Unity.VisualScripting;
using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Player_Main player;
    public float autoAttackMaxPoint = 100;
    public float autoAttackCurrentPoint = 0;
    public float autoAttackRecovery = 80;
    Animator anim;
    Scaner scaner;
    SpriteRenderer sprite;
    
    private void Awake() {
        player = GetComponent<Player_Main>();
         anim = GetComponent<Animator>();
         scaner = GetComponent<Scaner>();
         sprite = GetComponent<SpriteRenderer>();
    }
    private void FixedUpdate() {
        AutoAttackPointUp();
        
    }
    private void AutoAttackPointUp(){

    if(player.playerStatus.isLive != true)
        return;
    if(player.playerMove.inputVec.magnitude > 0.01f)
        return;

        autoAttackCurrentPoint += autoAttackRecovery * Time.fixedDeltaTime;
        if(autoAttackCurrentPoint>= autoAttackMaxPoint && scaner.nearestTarget != null){
            autoAttackCurrentPoint = 0;
            AutoAttack_exe();
            

    }
    }
    private void AutoAttack_exe(){
          if (scaner.nearestTarget == null)
            return;

            

        anim.SetTrigger("attack");
        Vector3 targetPos = scaner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;
        //
        //공격방향으로 스프라이트 반전
        sprite.flipX = dir.x < 0;

        //
        float damage = 10;//임시
        int bulletNumber = 1;
        int per = 0;//관통 현재 0
        float bulletspeed = 6;
        Transform bullet = GameManager.instance.poolManager.Get(bulletNumber).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.right, dir);
        //bullet 오브젝트에 정보 전달.
        bullet.GetComponent<bullet>().Init(damage, per, bulletspeed, dir);
    }
}
