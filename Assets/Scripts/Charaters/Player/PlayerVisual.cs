using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (animator == null) Debug.LogError("[PlayerVisual] Animator not found on this GameObject.", this);
    }

    public void ApplyController(RuntimeAnimatorController controller)
    {
        if (animator == null) return;

        if (controller == null)
        {
            Debug.LogWarning("[PlayerVisual] controller is null. skip apply.", this);
            return;
        }

        animator.runtimeAnimatorController = controller;
    }
}
