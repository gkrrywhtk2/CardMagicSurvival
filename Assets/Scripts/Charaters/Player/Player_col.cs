using UnityEngine;
using System.Collections;

public class Player_col : MonoBehaviour
{
    
    Player_Status playerStatus;
    private bool nowHit;
    private float hitCoolTime = 0.5f;
    Animator ani;
    Rigidbody2D rigid;
    CapsuleCollider2D capsuleCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void Awake(){
    playerStatus = GetComponent<Player_Status>();
    ani = GetComponent<Animator>();
    rigid = GetComponent<Rigidbody2D>();
    capsuleCollider = GetComponent<CapsuleCollider2D>();

}
    // Update is called once per frame
    void Update()
    {
        
    }

   public void HitCalCulator(float damage){
    //몬스터로부터 피격시 호출
    if(nowHit == true)
        return;
    if(playerStatus.isLive != true)
        return;
    if(GameManager.instance.levelUpState == true)
        return;
    
    playerStatus.health -= damage;
    if(playerStatus.health <= 0){

        playerStatus.isLive = false;
        //rigid.bodyType = RigidbodyType2D.Static;
        capsuleCollider.isTrigger = true;
        ani.SetTrigger("Death");
    }
    nowHit = true;
    StartCoroutine(HitTimer());

   }
   private IEnumerator HitTimer(){
    yield return new WaitForSeconds(hitCoolTime);
    nowHit = false;
   }
   public void PlayerDeath(){
    GameManager.instance.player.playerStatus.isLive = false;
    GameManager.instance.restartButton.SetActive(true);
   }
   
}
