using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMove : MonoBehaviour
{
    Player_Main player;
    public Vector2 inputVec;
    Rigidbody2D rigid;
    SpriteRenderer sprite;
     Animator anim;

    public float speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void Awake() {
        player = GetComponent<Player_Main>();
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
         anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void FixedUpdate() {
        if(player.playerStatus.isLive != true)
        return;
        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }

    private void LateUpdate() {
        anim.SetFloat("speed", inputVec.magnitude);
     if (inputVec.x != 0)
        {
            sprite.flipX = inputVec.x < 0;
          
        }
    }
}
