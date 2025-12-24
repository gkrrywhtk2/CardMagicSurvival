using UnityEngine;
using System.Collections.Generic;

public class InGameArtefactManager : MonoBehaviour
{
    public ArtefactScriptableData[] artefactScriptableDatas;

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

        public void ApplyArtefactEffect(ArtefactInstance artefactInstance)
    {
        int ID = artefactInstance.ID;
        int level = artefactInstance.level;
        
        if (artefactDictionary.ContainsKey(ID))
        {
            artefactDictionary[ID].Apply();
            nowArtefacts.Add(new ArtefactInstance(ID, level));
            
            // ✅ 추가된 직후 로그
            Debug.Log($"[Artefact] Added ID:{ID}, Level:{level} | Total Count: {nowArtefacts.Count}");
        }
        else
        {
            Debug.LogWarning($"[Artefact] Effect not defined for ID:{ID}");
        }
    }
}
