using UnityEngine;

public class JoyStick_P : MonoBehaviour
{
     [Header("Connect")]
    public FloatingJoystick joy;
    Animator anim;
    SpriteRenderer spr;
    Rigidbody2D rigid;
    public Vector2 inputVec;
    public bool nowMove;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
    }
     private void FixedUpdate(){
        if(GameManager.instance.player.playerStatus.isLive != true)
        return;
        if(GameManager.instance.inGamePlay != true)
        return;

    JoyStickMove();
     }

     private void JoyStickMove()
    {
        inputVec.x = joy.Horizontal;
        inputVec.y = joy.Vertical;

        float moveSpeed = GameManager.instance.player.playerMove.speed;
        Vector2 nextVec = inputVec.normalized * moveSpeed * Time.deltaTime;
        Vector2 targetPosition = rigid.position + nextVec;
        rigid.MovePosition(targetPosition);
        nowMove = nextVec.magnitude > 0;
       
    }
     private void LateUpdate()
    {
        anim.SetFloat("speed", inputVec.magnitude);
        if (inputVec.x != 0)
        {
            spr.flipX = inputVec.x < 0;
          
        }
    }
}