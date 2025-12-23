using UnityEngine;
using System.Collections.Generic;

public class InGameArtefactManager : MonoBehaviour
{
    public ArtefactScriptableData[] artefactScriptableDatas;
    public ArtefactSelect artefactSelect;

    private Dictionary<int, IArtefact> artefactDictionary;

     public List<ArtefactInstance> AllArtefacts = new();
    public List<ArtefactInstance> nowArtefacts = new();

    public void Start()
    {
        // 모든 아티팩트 인스턴스를 초기화합니다.
        foreach (var artefactData in artefactScriptableDatas)
        {
            AllArtefacts.Add(new ArtefactInstance(artefactData.artefactId, 1)); // 기본 레벨 1로 설정
        }
        InitArtefactDictionary();
    }
    public void InitArtefactDictionary(){
        artefactDictionary = new Dictionary<int, IArtefact>
        {
            { 0, gameObject.AddComponent<Arte_0>() },
            { 1, gameObject.AddComponent<Arte_1>() },
            { 2, gameObject.AddComponent<Arte_2>() },
            { 3, gameObject.AddComponent<Arte_3>() },
            { 4, gameObject.AddComponent<Arte_4>() },
        };
    }

     public void ApplyArtefactEffect(int artefactID)
    {
        if (artefactDictionary.ContainsKey(artefactID))
        {
            artefactDictionary[artefactID].Apply();
        }
        else
        {
            Debug.LogWarning("아티팩트 효과가 정의되지 않았습니다.");
        }
    }

}
