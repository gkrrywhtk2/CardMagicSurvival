using UnityEngine;

public class DisableOnParticleStopped : MonoBehaviour
{
    [SerializeField] private ParticleSystem rootPS;

    private void Awake()
    {
        if (rootPS == null) rootPS = GetComponentInChildren<ParticleSystem>(true);
    }

    private void OnEnable()
    {
        // 풀링 재사용 시 안전하게 “처음부터” 재생
        if (rootPS != null)
        {
            rootPS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            rootPS.Play(true);
        }
    }

    // ParticleSystem Main의 Stop Action을 Callback으로 설정해야 호출됨
    private void OnParticleSystemStopped()
    {
        gameObject.SetActive(false);
    }
}
