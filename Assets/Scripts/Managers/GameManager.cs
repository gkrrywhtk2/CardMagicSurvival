using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("#GameObjectConnect")]
    public PlayerMove playerMove;//플레이어 오브젝트.
    public Player_Status player;

    [Header("#ManagerConnect")]
    public ObjectPooling poolManager;//오브젝트 풀링


 public void Awake()
    {
        instance = this;
    }
}
