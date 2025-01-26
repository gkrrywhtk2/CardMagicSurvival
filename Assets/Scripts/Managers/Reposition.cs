using System.Globalization;
using UnityEngine;

public class Reposition : MonoBehaviour
{

    public Transform center;
    Collider2D coll;

    private void Awake() {
        coll = GetComponent<Collider2D>();
    }
   private void OnTriggerExit2D(Collider2D collison) {
    if(!collison.CompareTag("Area"))
        return;    

    Vector3 playerpos = GameManager.instance.playerGameObject.transform.position;
        Vector3 thispos = center.transform.position;

    
        switch(transform.tag){
        case "Ground":
        float diffX = playerpos.x - thispos.x;
        float diffY = playerpos.y - thispos.y;
      // Debug.Log(playerpos);
        float dirX = diffX < 0 ? -1:1;
        float dirY = diffY < 0 ? -1:1;

        diffX = Mathf.Abs(diffX);
        diffY = Mathf.Abs(diffY);
        


        if(diffX > diffY){
            transform.Translate(Vector3.right * dirX * 40);
            //Debug.Log("좌우 이동" + diffX + "," +  diffY );
        }
        else if(diffX < diffY){
             transform.Translate(Vector3.up * dirY * 40);
              //  Debug.Log("상하 이동" + diffX + "," +  diffY );
        }
        break;
    case "Monster":

        Vector3 playerDir = GameManager.instance.player.joystickP.inputVec;
        if(coll.enabled){
            transform.Translate(playerDir * 22 + new Vector3(Random.Range(-3f,3f), Random.Range(-3f,3f),0));
        }
        break;
    }
   }
}
