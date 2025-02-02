using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using RANK;


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
    if(GameManager.instance.ItemSelectState == true)
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

   private void OnTriggerEnter2D(Collider2D collison) {
    if(!collison.CompareTag("Gem_Item"))
        return;

    Gem_Item gem = collison.GetComponent<Gem_Item>();
    gem.Gem_Point.gameObject.SetActive(false);
    StartCoroutine(Get_Gem(gem.rank));
   }


   IEnumerator Get_Gem(Rank rank){
    GameManager.instance.player.playerEffect.levelUpCircleTimeStop.gameObject.SetActive(true);
    GameManager.instance.player.joystickP.speed = 0;
    GameManager.instance.itemManager.SpawnItems_(rank);
    GameManager.instance.ItemPause();
    yield return new WaitForSeconds(2);
     GameManager.instance.player.joystickP.speed = 3;
   
   }
   
}
