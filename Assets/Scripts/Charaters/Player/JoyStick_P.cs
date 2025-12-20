using UnityEngine;

public class JoyStick_P : MonoBehaviour
{
    [Header("Connect")]
    public FloatingJoystick joy;

    private Animator anim;
    private SpriteRenderer spr;
    private Rigidbody2D rigid;

    [Header("Refs (Cache)")]
    [SerializeField] private PlayerHP playerHp;
    [SerializeField] private PlayerMoveSpeed moveSpeed; 

    public Vector2 inputVec;
    public bool nowMove;
    public bool nextStageSetting = false;

    [Header("Next Stage Auto Move")]
    public float nextStageAutoSpeed = 5f;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();

        if (playerHp == null)
            playerHp = GetComponent<PlayerHP>();

        if (moveSpeed == null)
            moveSpeed = GetComponent<PlayerMoveSpeed>();
    }

    private void FixedUpdate()
    {
        if (playerHp != null && !playerHp.isLive) return;
        if (!GameManager.instance.GamePlayState) return;

        JoyStickMove();
        NextStageStopUpdate();
    }

    private void JoyStickMove()
    {
        if (nextStageSetting) return;

        inputVec.x = joy.Horizontal;
        inputVec.y = joy.Vertical;

        float curSpeed = (moveSpeed != null) ? moveSpeed.CurrentSpeed : 0f;

        Vector2 nextVec = inputVec.normalized * curSpeed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);

        nowMove = nextVec.sqrMagnitude > 0.000001f;
    }

    private void NextStageStopUpdate()
    {
        if (!nextStageSetting) return;

        Vector2 autoVec = Vector2.right;
        Vector2 nextVec = autoVec * nextStageAutoSpeed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    private void LateUpdate()
    {
        anim.SetFloat("speed", inputVec.magnitude);

        bool isFlipped = inputVec.x < 0;
        if (inputVec.x != 0 && spr.flipX != isFlipped)
            spr.flipX = isFlipped;
    }
}
