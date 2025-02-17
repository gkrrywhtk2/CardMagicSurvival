using UnityEngine;
using UnityEngine.UI;

public class JoyStick_P : MonoBehaviour
{
    [Header("Connect")]
    public FloatingJoystick joy;
    Animator anim;
    SpriteRenderer spr;
    Rigidbody2D rigid;
    public Vector2 inputVec;
    public bool nowMove;
    public float speed;
    //next stage
    public bool nextStageSetting = false;
    //weaponSetting
        public SpriteRenderer weaponR;
    public SpriteRenderer weaponL;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
    }
     private void FixedUpdate(){
        if(GameManager.instance.player.playerStatus.isLive != true)
        return;
        if(GameManager.instance.GamePlayState != true)
        return;
        
        JoyStickMove();
        NextStageStopUpdate();
     }

     private void JoyStickMove()
    {
        if(nextStageSetting == true)
            return;
        inputVec.x = joy.Horizontal;
        inputVec.y = joy.Vertical;

       // float moveSpeed = GameManager.instance.player.playerMove.speed;
        Vector2 nextVec = inputVec.normalized * speed * Time.deltaTime;
        Vector2 targetPosition = rigid.position + nextVec;
        rigid.MovePosition(targetPosition);
        nowMove = nextVec.magnitude > 0;
       
    }
       private void NextStageStopUpdate()
    {
        if(nextStageSetting == false)
            return;
        Vector2 inputVec;
        inputVec.x = 1;
        inputVec.y = 0;;

       // float moveSpeed = GameManager.instance.player.playerMove.speed;
        Vector2 nextVec = inputVec.normalized * 5 * Time.deltaTime;
        Rigidbody2D rigid = GameManager.instance.player.GetComponent<Rigidbody2D>();
        Vector2 targetPosition = rigid.position + nextVec;
        rigid.MovePosition(targetPosition);
       
    }
    private void LateUpdate()
{
    anim.SetFloat("speed", inputVec.magnitude);

    bool isFlipped = inputVec.x < 0;
    if (inputVec.x != 0 && spr.flipX != isFlipped)
    {
        spr.flipX = isFlipped;
        weaponR.gameObject.SetActive(!isFlipped);
        weaponL.gameObject.SetActive(isFlipped);
    }
}

}