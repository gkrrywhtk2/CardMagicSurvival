using UnityEngine;

public class JoyStick : MonoBehaviour
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
        Debug.Log("어디서 출력되고있음 ? ");
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
        Debug.Log(moveSpeed);
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


