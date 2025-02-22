using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;



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
    public DamageTextPooling damageTextPooling;//데미지 텍스트 풀링
    public MobPooling mobPooling;//몬스터 오브잭트 풀링
    public DeckCardPooling deckCardPooling;// 덱 카드 풀링
    public MobSpawnManager spawnManager;
    public DeckManager deckManager;
    public WaveManager waveManager;
    public ItemManager itemManager;
    public DataManager dataManager;
    public StageManager stageManager;
    public WeaponManager weaponManager;
    public BoardUI boardUI;
    [Header("#GameControl")]
    public bool cardOneTouch;
    public bool GamePlayState = false;//(이동, 카드 사용, 마나 회복, 자동 공격 불가)
    public bool ItemSelectState = false;//true시 이동만 가능
    public GameObject backG;//background image
    public FloatingJoystick joystick;//조이스틱
    public PixelPerfectCamera pixelPerfectCamera;

    public bool night;

     [Header("#DevelepCheat")]
     public bool cheatBoxTogle;
     public GameObject GoldCheat;

 public void Awake()
    {
        instance = this;
        restartButton.gameObject.SetActive(false);
        Application.targetFrameRate = 80;
          Screen.fullScreen = true;//풀스크린
    }

    public void GameStart(){
        gameStartButton.gameObject.SetActive(false);
        GamePlayState = true;
        ItemSelectState = false;
        deckManager.HandSetting();
        spawnManager.Spawn_Slime_0();
        spawnManager.Spawn_Slime_1();
         instance.player.playerStatus.StartHealthRegen();//체력 자동 회복
         instance.boardUI.ShowSeletedTap(2);//2는 카드 탭 세팅, 
         
         LinkToData();//데이터 불러오기
    }

    public void GameRestart(){
         SceneManager.LoadScene(0);
        restartButton.gameObject.SetActive(false);
    }

    public void NextWave(int ItemID){
        nextWaveButton.gameObject.SetActive(false);
        instance.GamePlayState = true;
        backG.gameObject.SetActive(false);
       // waveManager.waveTimeOver = false;
        //waveManager.nowWave += 1;
       //levelUpState = false;
       // waveManager.waveGameTime = 60;
      //  spawnManager.spawnAllow = true;
        //waveManager.CountText.type = HUD.InfoType.Wave;
       // spawnManager.Spawn_Slime_0();
       // spawnManager.Spawn_Slime_1();
      //  waveManager.battleIcon.gameObject.SetActive(true);
       // waveManager.mobIcon.gameObject.SetActive(false);
    }

    public void Pause(){
        GamePlayState = false;//(이동, 카드 사용, 마나 회복, 자동 공격 불가)
        joystick.GetComponent<Image>().raycastTarget = false;
        Time.timeScale = 0;
    }
    public void GamePlay(){
        GamePlayState = true;
         backG.gameObject.SetActive(false);
        ItemSelectState = false;
        instance.player.playerCol.GetComponent<Collider2D>().isTrigger = false; 
        joystick.GetComponent<Image>().raycastTarget = true;
        Time.timeScale = 1;
       
    }
    public void ItemPause(){
        //아이템 획득 이벤트시 특수 이벤트 중지(마나획복, 몬스터 충돌 등)
        instance.player.playerCol.GetComponent<Collider2D>().isTrigger = true; 
       ItemSelectState = true;
    }
    public void NightTogle(){
        
        if(night == false){
              instance.player.hikari.size = 5;
              night = true;
        }
        else{
            instance.player.hikari.size = 10;
              night = false;
        }
      
    }

    public void CheatBoxTogle(){
        if(cheatBoxTogle == false){
            GoldCheat.gameObject.SetActive(true);
            cheatBoxTogle = true;
        }else{
            GoldCheat.gameObject.SetActive(false);
            cheatBoxTogle = false;
        }
    }

    public void LinkToData(){
        //DataManager에 접근하여 현재 상태를 불러옴
        instance.dataManager.SyncStageLevelFromServer();
        instance.dataManager.SyncWeaponData();



          instance.dataManager.ChageToRealValue();//마지막에 있어야 함! 무기 효과 적용 받아야됨
    }

}
