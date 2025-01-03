using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("#GameObjectConnect")]
    public PlayerMove playerMove;//플레이어 오브젝트.
    public Player_Main player;
    public GameObject restartButton;

    [Header("#ManagerConnect")]
    public ObjectPooling poolManager;//오브젝트 풀링


 public void Awake()
    {
        instance = this;
        restartButton.gameObject.SetActive(false);
        Application.targetFrameRate = 60;
    }

    public void GameRestart(){
         SceneManager.LoadScene(0);
        restartButton.gameObject.SetActive(false);
    }
}
