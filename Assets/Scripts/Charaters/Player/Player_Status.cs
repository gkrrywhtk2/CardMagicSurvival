using UnityEngine;

public class Player_Status : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool isLive;
    public float health;
    public float maxHealth = 100;
    private void Awake() {
        isLive = true;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
