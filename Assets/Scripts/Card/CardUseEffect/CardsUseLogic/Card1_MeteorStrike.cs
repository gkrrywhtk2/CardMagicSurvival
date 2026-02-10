using UnityEngine;
using UnityEngine.EventSystems;
using Game.RankSystem;
using Assets.PixelFantasy.PixelTileEngine.Scripts;

public class Card1_MeteorStrike : MonoBehaviour, ICardUse
{

    public void Use(PointerEventData eventData)
    {
        MagicCard card = eventData.pointerDrag.GetComponent<MagicCard>();
        int Level = card.level;
        
        if (card == null)
        {
            Debug.LogError("MagicCard component is missing on pointerDrag object!");
            return;
        }
         // 화염구 생성
        int poolNumber = 2;
        Meteor meteor = GameManager.instance.objectPooling.Get(poolNumber).GetComponent<Meteor>();

        // 생성 위치 설정
        Vector3 startPosition = GameManager.instance.player.fireBallPoint.transform.position;
        meteor.transform.position = startPosition;

        // 드랍 포인트를 목표로 초기화
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, Camera.main.nearClipPlane));
        targetPosition.z = 0;

        meteor.Init(targetPosition, Level); 

        // 방향 벡터 계산
        Vector3 direction = targetPosition - startPosition;

        // 회전 각도 계산 (Atan2 사용)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 화염구 회전 적용
        meteor.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }


}
