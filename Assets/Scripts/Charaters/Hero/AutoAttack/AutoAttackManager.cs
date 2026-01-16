using System.Collections.Generic;
using UnityEngine;
using Game.RankSystem; // RankType

public class AutoAttackManager : MonoBehaviour
{
    [Header("Hero AutoAttack Scripts (attach to this object)")]
    public HeroAutoAttackBase[] heroAutoAttacks; // 인스펙터용

    private Dictionary<int, HeroAutoAttackBase> map;
    private HeroAutoAttackBase current;

    [Header("Level")]
    public int CskillLv;
    public int RskillLv;
    public int EskillLv;
    public int LskillLv;
    public int MskillLv;

    private void Awake()
    {
        BuildMap();
        StopAll(); // 안전
    }

    private void BuildMap()
    {
        map = new Dictionary<int, HeroAutoAttackBase>();

        // 인스펙터로 안 넣어도, 같은 오브젝트에 붙어있는 컴포넌트 자동 수집 가능
        if (heroAutoAttacks == null || heroAutoAttacks.Length == 0)
            heroAutoAttacks = GetComponents<HeroAutoAttackBase>();

        foreach (var h in heroAutoAttacks)
        {
            if (h == null) continue;

            int id = h.heroId;
            if (map.ContainsKey(id))
            {
                Debug.LogError($"[AutoAttackManager] Duplicate heroId auto-attack script: {id}", this);
                continue;
            }

            map.Add(id, h);
            h.gameObject.SetActive(true); // 같은 오브젝트라 의미는 적지만 안전
            h.Init(this);
        }
    }

    public void Apply(int heroId, RankType rank, int C, int R, int E, int L, int M)
    {
        if (map == null) BuildMap();

        // 1) 기존 종료
        StopAll();

        // 2) 새로 시작
        if (!map.TryGetValue(heroId, out var handler) || handler == null)
        {
            Debug.LogError($"[AutoAttackManager] No auto-attack handler for heroId={heroId}", this);
            return;
        }

        current = handler;
        current.StartForRank(rank);

        //스킬 레벨 적용
        CskillLv = C;
        RskillLv = R;
        EskillLv = E;
        LskillLv = L;
        MskillLv = M;
    }

    public void StopAll()
    {
        if (current != null)
        {
            current.StopAllAttacks();
            current = null;
        }
    }
}
