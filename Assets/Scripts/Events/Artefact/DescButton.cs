using TMPro;
using UnityEngine;

public class DescButton : MonoBehaviour
{
    public int index;
    public ArtefactStone artefactStone;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 설명 버튼과 플레이어가 충돌했을 때의 로직을 여기에 작성합니다.
            Debug.Log("플레이어가 설명 버튼과 충돌했습니다.");
            artefactStone.ShowDesc(index);
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
            if (collision.CompareTag("Player"))
            {
                // 플레이어가 설명 버튼에서 벗어났을 때의 로직을 여기에 작성합니다.
                Debug.Log("플레이어가 설명 버튼에서 벗어났습니다.");
                artefactStone.HideDesc();
            }
    }
  
}
