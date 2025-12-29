using UnityEngine;
using Game.RankSystem; // RankType

public class Player_Main : MonoBehaviour
{
    //다른 오브젝트가 플레이어 오브젝트에 접근할때 사용하는 클래스
    public PlayerMove playerMove;
    public Player_Status playerStatus;
    public Player_col playerCol;
    public AutoAttack autoAttack;
    public JoyStick_P joystickP;

    public Player_Effect playerEffect;
    public Dir_Front dirFront;
    public DIr_FrontForCard dirFront_forCard;

    public Transform fireBallPoint;
    public Transform playerCenterPivot;
    public PlayerCritical playerCritical;
    public PlayerHP playerHP;
    public PlayerVisual playerVisual;
    public AutoAttackManager autoAttackManager;

    // =========================
    // ✅ Hero 적용 결과 "저장만"
    // =========================
    [Header("Hero Applied (Read Only)")]
    [SerializeField] private int currentHeroId;
    [SerializeField] private int currentLevel;
    [SerializeField] private RankType currentRank;
    [SerializeField] private int currentExp;

    [Header("Base Stats (Applied)")]
    [SerializeField] private float baseHP;
    [SerializeField] private int baseATK;
    [SerializeField] private float baseMoveSpeed;
    [SerializeField] private float baseCritChance;
    [SerializeField] private float baseCritDamage;

    // Getter
    public int CurrentHeroId => currentHeroId;
    public int CurrentLevel => currentLevel;
    public RankType CurrentRank => currentRank;
    public int CurrentExp => currentExp;

    public float BaseHP => baseHP;
    public int BaseATK => baseATK;
    public float BaseMoveSpeed => baseMoveSpeed;
    public float BaseCritChance => baseCritChance;
    public float BaseCritDamage => baseCritDamage;

    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        playerStatus = GetComponent<Player_Status>();
        playerCol = GetComponent<Player_col>();
        autoAttack = GetComponent<AutoAttack>();
        joystickP = GetComponent<JoyStick_P>();
        playerEffect = GetComponent<Player_Effect>();
        dirFront = GetComponentInChildren<Dir_Front>();
        dirFront_forCard = GetComponentInChildren<DIr_FrontForCard>();
        playerHP = GetComponent<PlayerHP>();
        playerCritical = GetComponent<PlayerCritical>();
        playerVisual = GetComponent<PlayerVisual>();
    }

    /// <summary>
    /// ✅ HeroManager가 계산한 스탯을 Player_Main에 "base"로 저장만 하는 함수
    /// </summary>
    public void ApplyHeroStats(int heroId, int level, RankType rank, int exp, StatBlock stats)
    {
        currentHeroId = heroId;
        currentLevel = level;
        currentRank = rank;
        currentExp = exp;

        baseHP = stats.hp;
        baseATK = stats.attack;
        baseMoveSpeed = stats.moveSpeed;
        baseCritChance = stats.critChance;
        baseCritDamage = stats.critDamage;

        SendStatus();//플레이어 스탯 전송   
    }

    public void StatCheak(int value)
    {
        bool cheak = value == 1 ? true : false;
    }
    public void SendStatus()
    {
        playerHP.InitHealth(baseHP);
        playerStatus.totalATK = baseATK;
        playerStatus.playerMoveSpeed.baseSpeed = baseMoveSpeed;
        playerCritical.baseCritChance = baseCritChance;
        playerCritical.critMultiplier = baseCritDamage;
    }
}
