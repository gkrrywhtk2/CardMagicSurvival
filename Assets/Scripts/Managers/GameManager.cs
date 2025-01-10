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
    public GameObject gameStartButton;//게임 시작 버튼


    [Header("#ManagerConnect")]
    public ObjectPooling poolManager;//오브젝트 풀링
    public EffectPooling effectPoolManager;//이펙트 풀링
    public MobSpawnManager spawnManager;
    public DeckManager deckManager;
    [Header("#GameControl")]
    public bool cardOneTouch;
    public bool inGamePlay = false;//인게임 맵의 시간이 흐르고 있을때 ON 


 public void Awake()
    {
        instance = this;
        restartButton.gameObject.SetActive(false);
        Application.targetFrameRate = 60;
          Screen.fullScreen = true;//풀스크린
    }

    public void GameStart(){
        gameStartButton.gameObject.SetActive(false);
        inGamePlay = true;
        deckManager.DeckSetting();
        spawnManager.Spawn_Slime_0();
        spawnManager.Spawn_Slime_1();
    }

    public void GameRestart(){
         SceneManager.LoadScene(0);
        restartButton.gameObject.SetActive(false);
    }
}
