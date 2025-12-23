using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArtefactSelect : MonoBehaviour
{
    public InGameArtefactManager artefactManager;
    public Artefact_Object[] artefact_Objects;

    public void CallSetRandomArtefacts()
    {
        // 중복 호출 방지를 위해 진행 중인 코루틴이 있다면 멈춰주는 것이 안전할 수 있습니다.
        StopAllCoroutines(); 
        StartCoroutine(SetRandomArtefactsFromManager());
    }

    private IEnumerator SetRandomArtefactsFromManager()
    {
        if (artefactManager == null || artefactManager.AllArtefacts.Count == 0) yield break;

        // 먼저 모든 선택지 오브젝트를 비활성화 상태로 시작
        foreach (var obj in artefact_Objects) obj.gameObject.SetActive(false);

        // 1. 인덱스 리스트 생성 및 셔플 (기존 로직 동일)
        List<int> indices = new List<int>();
        for (int i = 0; i < artefactManager.AllArtefacts.Count; i++) indices.Add(i);

        for (int i = 0; i < indices.Count; i++)
        {
            int temp = indices[i];
            int randomIndex = Random.Range(i, indices.Count);
            indices[i] = indices[randomIndex];
            indices[randomIndex] = temp;
        }

        // 2. 순차적 등장 연출
        int selectionCount = Mathf.Min(artefact_Objects.Length, indices.Count);

        for (int i = 0; i < selectionCount; i++)
        {
            // 대기 시간을 데이터 설정 '전'에 둘지 '후'에 둘지에 따라 느낌이 다릅니다.
            // 등장 직전에 0.5~1초 정도 쉬어주면 좋습니다.
            yield return new WaitForSeconds(0.5f); 

            int dataIndex = indices[i];
            ArtefactInstance selectedData = artefactManager.AllArtefacts[dataIndex];

            // 데이터 초기화를 먼저 하고 오브젝트를 켭니다.
            artefact_Objects[i].Init(selectedData); 
            artefact_Objects[i].gameObject.SetActive(true);
            
            // TODO: 여기서 artefact_Objects[i]의 애니메이션(예: Fade-in 또는 Scale Up)을 
            // 실행해주면 연출이 훨씬 고급스러워집니다.
        }
    }
}