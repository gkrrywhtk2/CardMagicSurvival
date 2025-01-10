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
    public GameObject playerGameObject;

    [Header("#ManagerConnect")]
    public ObjectPooling poolManager;//오브젝트 풀링
    public EffectPooling effectPoolManager;//이펙트 풀링
    [Header("#GameControl")]
    public bool cardOneTouch;


 public void Awake()
    {
        instance = this;
        restartButton.gameObject.SetActive(false);
        Application.targetFrameRate = 60;
          Screen.fullScreen = true;//풀스크린
    }

    public void GameRestart(){
         SceneManager.LoadScene(0);
        restartButton.gameObject.SetActive(false);
    }
}
