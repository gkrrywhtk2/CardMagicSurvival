using UnityEngine;
// 만약 인터페이스가 다른 파일에 있다면 해당 네임스페이스가 필요할 수 있습니다.

public class Arte_1 : MonoBehaviour, IArtefact
{
    // IArtefact 인터페이스에 정의된 메서드를 구현합니다.
    public void Apply()
    {
        // 여기에 "무딘 검"이나 "사과" 같은 아이템의 구체적인 효과를 코딩합니다.
        Debug.Log("Arte_: 효과가 적용되었습니다!");
        GameManager.instance.player.playerStatus.totalATK += 5; //플레이어의 총 공격력을 5 증가시킴
        GameManager.instance.inGameArtefactManager.nowArtefacts.Add(new ArtefactInstance(0, 1));//현재 보유 아티팩트 리스트에 추가
    }
}