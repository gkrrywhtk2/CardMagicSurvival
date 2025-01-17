using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawnManager : MonoBehaviour
{
    public MobSpawnData[] spawndata;//data of monster.
    public Transform[] spawnpoint;
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
        int mob_id = 0;
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
        int mob_id = 1;
        yield return new WaitForSeconds(spawnCooltime);
            if(spawnAllow){
                MonsterSpawn(mob_id);
                StartCoroutine(Spawn_Slime1());
            }
    }
    public void MonsterSpawn(int mob_id){
            if(GameManager.instance.GamePlayState == false)
                return;

        GameObject monster = GameManager.instance.poolManager.Get(0);
        mobCount++;
        monster.GetComponent<Monster>().Init(spawndata[mob_id]);
        monster.transform.position = spawnpoint[Random.Range(1, spawnpoint.Length)].position;
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

}

