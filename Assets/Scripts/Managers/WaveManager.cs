using UnityEngine;


public class WaveManager : MonoBehaviour
{
    public int nowWave;//현재 웨이브 레벨
    public float waveGameTime;//각 웨이브 목표 시간
    public float nowGameTime;//현재 시점 시간
    public bool waveTimeOver;//웨이브 시간 다 지났을 경우 true

    public int getGold;//획득한 골드 수
    //Game Obect
    public GameObject battleIcon;
    public GameObject mobIcon;
    public HUD CountText;

    private void Update(){
        TimeDown();
        //Cheak_WaveClear();웨이브는 개발중단 되었음
    }

    private void TimeDown()
    {
        if (GameManager.instance.GamePlayState != true)
            return;
        if(waveTimeOver != false)
            return;
        if(GameManager.instance.ItemSelectState == true)
            return;

            waveGameTime -= Time.deltaTime;

        if (waveGameTime <= 0)
        {
            //WaveTimeOver();
        }

    }
    public void WaveTimeOver(){
        //웨이브는 개발 중단
          //몬스터 스폰 중지
            waveTimeOver = true;
            GameManager.instance.spawnManager.spawnAllow = false;
            //남은 몬스터가 있다면 몬스터를 처치하세요
            battleIcon.gameObject.SetActive(false);
            mobIcon.gameObject.SetActive(true);
            CountText.type = HUD.InfoType.Mobcount;
    }
    public void Cheak_WaveClear(){
        if(waveTimeOver == false)
            return;
        if(GameManager.instance.spawnManager.mobCount > 0)
            return;
        
        GameManager.instance.ItemSelectState = true;//바꿔야함 웨이브 오버 스테이트로
        GameManager.instance.nextWaveButton.gameObject.SetActive(true);
    }

}
