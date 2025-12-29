using UnityEngine;

public class PoisonOnHit : MonoBehaviour
{
    [HideInInspector] public float poisonDps;
    [HideInInspector] public float duration;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!enabled) return;
        if (!collision.CompareTag("Monster")) return;

        Monster_Poison poison = collision.GetComponent<Monster_Poison>();
        if (poison == null) return;

        poison.Apply(poisonDps, duration);
    }
}
