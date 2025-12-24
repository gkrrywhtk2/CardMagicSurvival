using UnityEngine;
using System.Collections;

public class Player_col : MonoBehaviour
{
    private Player_Status playerStatus;
    private PlayerHP playerHP;

    private bool nowHit;
    [SerializeField] private float hitCoolTime = 0.5f;

    private Animator ani;
    private Rigidbody2D rigid;
    private CapsuleCollider2D capsuleCollider;

    private void Awake()
    {
        playerStatus = GetComponent<Player_Status>();
        playerHP = playerStatus != null ? playerStatus.playerHP : GetComponent<PlayerHP>();

        ani = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    public void HitCalCulator(float damage)
    {
        // 몬스터로부터 피격 시 호출
        if (nowHit) return;
        if (playerHP == null) return;
        if (!playerHP.isLive) return;

        // ✅ 데미지 적용
        playerHP.TakeDamage(damage);

        // ✅ 사망 체크는 PlayerHP 기준
        if (!playerHP.isLive)
        {
            PlayerDeathSetting();
            return;
        }

        nowHit = true;
        StartCoroutine(HitTimer());
    }

    private IEnumerator HitTimer()
    {
        yield return new WaitForSeconds(hitCoolTime);
        nowHit = false;
    }

    public void PlayerDeathSetting()
    {
        // ✅ 여기서 isLive를 또 만져도 되지만, 이미 TakeDamage에서 false로 됐을 가능성이 큼
        if (playerHP != null) playerHP.isLive = false;

        // rigid.bodyType = RigidbodyType2D.Static;
        if (capsuleCollider != null) capsuleCollider.isTrigger = true;
        if (ani != null) ani.SetTrigger("Death");
    }

    public void PlayerDeath()
    {
        if (playerHP != null) playerHP.isLive = false;
        GameManager.instance.restartButton.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Gold"))
        {
            // TODO: 골드 획득 처리
        }
        if (collision.CompareTag("ArtefactStone"))
        {
            // TODO: 아티팩트 이벤트 처리
            ArtefactStone artefactStone = collision.GetComponent<ArtefactStone>();
            if (artefactStone != null)
            {
                artefactStone.TriggerArtefactEvent();
            }
        }
        if (collision.CompareTag("Artefact"))
        {
            Artefact_Object artefact = collision.GetComponent<Artefact_Object>();
            if (artefact != null)
            {
                ArtefactInstance artefactInstance = artefact.aretefactInstance;
                GameManager.instance.inGameArtefactManager.ApplyArtefactEffect(artefactInstance);
                artefact.EndEvent();
            } 
        }
    }

    public int ReturnGoldValue(float value)
    {
        // LUK은 Player_Status에 남아있으니 그대로 사용
        float luk = (playerStatus != null) ? playerStatus.LUK : 0f;
        float returnValue = value * (1f + luk / 100f);
        return (int)returnValue;
    }
}
