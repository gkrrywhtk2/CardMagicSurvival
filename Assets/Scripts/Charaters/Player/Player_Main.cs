using UnityEngine;

public class Player_Main : MonoBehaviour
{
    //다른 오브젝트가 플레이어 오브젝트에 접근할때 사용하는 클래스
   public PlayerMove playerMove;
   public Player_Status playerStatus;
   public Player_col playerCol;
   public AutoAttack autoAttack;
  public JoyStick_P playerJoyStick;

  public Player_Effect playerEffect;
  public Dir_Front dirFront;
  //
  public Transform fireBallPoint;
  public Transform playerCenterPivot;
 
   private void Awake() {
    playerMove = GetComponent<PlayerMove>();
    playerStatus = GetComponent<Player_Status>();
    playerCol = GetComponent<Player_col>();
    autoAttack = GetComponent<AutoAttack>();
    playerJoyStick = GetComponent<JoyStick_P>();
    playerEffect = GetComponent<Player_Effect>();
    dirFront = GetComponentInChildren<Dir_Front>();
   }

}
