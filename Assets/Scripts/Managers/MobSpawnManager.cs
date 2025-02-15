using System.Collections;
//using Assets.PixelFantasy.PixelMonsters.Common.Scripts;
using MonsterType;
using UnityEngine;

namespace MonsterType{
       public enum MobType{normal, boss};
}
public class MobSpawnManager : MonoBehaviour
{
    public MobSpawnData[] spawndata;//data of monster.
    public BossSpawnData[] bossSpawndata;//data of boss.
    public Transform[] spawnpoint;
    public Transform bossSpawnPoint;
    public bool spawnAllow;// 스폰 허용, 웨이브 시작시 true, 웨이브 종료시 false
    public int mobCount;//현재 남아있는 몬스터의 수
    public float spawnTime_slime_0;//슬라임0 스폰 쿨타임 f초 마다 실행
    public bool slime_0;//
     public float spawnTime_slime_1;//슬라임1 스폰 쿨타임
    public bool slime_1;//
     private void Awake()
    {
        spawnpoint = GetComponentsInChildren<Transform>();
        spawnAllow = true;
        spawnTime_slime_0 = 2f;
        spawnTime_slime_1 = 2f;
    }

    public void Spawn_Slime_0()
    {
        //슬라임0 스폰 시작 함수
        slime_0 = true;
        StartCoroutine(Spawn_Slime0());
    }
     IEnumerator Spawn_Slime0()
    {

        if (slime_0 != true)
            yield break;
        float spawnCooltime = spawnTime_slime_0;
        int mob_id = 4;
        yield return new WaitForSeconds(spawnCooltime);
            if(spawnAllow){
                MonsterSpawn(mob_id);
                StartCoroutine(Spawn_Slime0());
            }
      
    }
    public void Spawn_Slime_1()
    {
        //슬라임0 스폰 시작 함수
        slime_1 = true;
        StartCoroutine(Spawn_Slime1());
    }
    IEnumerator Spawn_Slime1()
    {

        if (slime_1 != true)
            yield break;
        float spawnCooltime = spawnTime_slime_1;
        int mob_id = 3;
        yield return new WaitForSeconds(spawnCooltime);
            if(spawnAllow){
                MonsterSpawn(mob_id);
                StartCoroutine(Spawn_Slime1());
            }
    }
    public void MonsterSpawn(int mob_id){
            if(GameManager.instance.GamePlayState == false)
                return;

            if(GameManager.instance.ItemSelectState == true)
                return;

        Monster monster = GameManager.instance.poolManager.Get(0).GetComponent<Monster>();
        mobCount++;
        monster.GetComponent<Monster>().Init(spawndata[mob_id]);
        if(monster.mobType == MobType.normal){
            monster.transform.position = spawnpoint[Random.Range(1, spawnpoint.Length)].position;
        }else if(monster.mobType == MobType.boss){
            monster.transform.localScale = new Vector3(3,3,1);
            monster.transform.position = bossSpawnPoint.position;
            GameManager.instance.stageManager.bossObject = monster;//보스몬스터 링크
        }
    
    }

    public void BossSpawn(int boss_id){
        GameObject boss = GameManager.instance.mobPooling.Get(1);
        boss.GetComponent<Boss>().Init(bossSpawndata[boss_id]);
        boss.transform.position = bossSpawnPoint.position;
    }


}





    [System.Serializable]
public class MobSpawnData
{
    public int mob_id;
    public float health;
    public float maxHealth;
    public float speed;
    public float damage;
    public MobType mobType;

}
[System.Serializable]
public class BossSpawnData
{
    public int boss_id;
    public float health;
    public float maxHealth;
    public float speed;
    public float damage;

}

