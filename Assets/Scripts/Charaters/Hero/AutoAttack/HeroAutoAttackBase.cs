using UnityEngine;
using System.Collections;
using Game.RankSystem; // RankType

public abstract class HeroAutoAttackBase : MonoBehaviour
{
    [Header("Identity")]
    public int heroId;

    protected AutoAttackManager manager;
    private Coroutine running;

    public void Init(AutoAttackManager mgr)
    {
        manager = mgr;
    }

    public void StartForRank(RankType rank)
    {
        StopAllAttacks();
        running = StartCoroutine(AutoAttackRoutine(rank));
    }

    public void StopAllAttacks()
    {
        if (running != null)
        {
            StopCoroutine(running);
            running = null;
        }
        OnStop();
    }

    protected virtual void OnStop() { }

    // ✅ 캐릭터별로 rank에 맞는 코루틴을 구현
    protected abstract IEnumerator AutoAttackRoutine(RankType rank);
}
