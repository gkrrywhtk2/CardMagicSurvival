using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    private Player_Main player;
    private Player_Status playerStatus;
    private PlayerHP playerHp;

    public Vector2 inputVec;

    private Rigidbody2D rigid;
    private SpriteRenderer sprite;
    private Animator anim;

    public float speed;

    private void Awake()
    {
        player = GetComponent<Player_Main>();
        playerStatus = GetComponent<Player_Status>();
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        // ✅ HP 캐싱 (인스펙터/Status/직접 컴포넌트 순으로 탐색)
        if (playerStatus != null && playerStatus.playerHP != null)
            playerHp = playerStatus.playerHP;
        else
            playerHp = GetComponent<PlayerHP>();
    }

    private void FixedUpdate()
    {
        // ✅ 생존 체크는 PlayerHP 기준
        if (playerHp != null && !playerHp.isLive) return;

        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    private void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }

    private void LateUpdate()
    {
        anim.SetFloat("speed", inputVec.magnitude);

        if (inputVec.x != 0)
            sprite.flipX = inputVec.x < 0;
    }
}
