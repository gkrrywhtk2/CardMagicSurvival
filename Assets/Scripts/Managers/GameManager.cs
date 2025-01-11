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
    public GameObject nextWaveButton;//다음 웨이브 버튼


    [Header("#ManagerConnect")]
    public ObjectPooling poolManager;//오브젝트 풀링
    public EffectPooling effectPoolManager;//이펙트 풀링
    public MobSpawnManager spawnManager;
    public DeckManager deckManager;
    public WaveManager waveManager;
    [Header("#GameControl")]
    public bool cardOneTouch;
    public bool GamePlayState = false;//(이동, 카드 사용, 마나 회복, 자동 공격 불가)
    public bool waveOverState = false;//true시 이동만 가능


 public void Awake()
    {
        instance = this;
        restartButton.gameObject.SetActive(false);
        Application.targetFrameRate = 60;
          Screen.fullScreen = true;//풀스크린
    }

    public void GameStart(){
        gameStartButton.gameObject.SetActive(false);
        GamePlayState = true;
        waveOverState = false;
        deckManager.DeckSetting();
        spawnManager.Spawn_Slime_0();
        spawnManager.Spawn_Slime_1();
    }

    public void GameRestart(){
         SceneManager.LoadScene(0);
        restartButton.gameObject.SetActive(false);
    }

    public void NextWave(){
        nextWaveButton.gameObject.SetActive(false);
        waveManager.waveTimeOver = false;
        waveManager.nowWave += 1;
        waveOverState = false;
        waveManager.waveGameTime = 60;
        spawnManager.spawnAllow = true;
        waveManager.CountText.type = HUD.InfoType.Wave;
        spawnManager.Spawn_Slime_0();
        spawnManager.Spawn_Slime_1();
        waveManager.battleIcon.gameObject.SetActive(true);
        waveManager.mobIcon.gameObject.SetActive(false);
    }
}
