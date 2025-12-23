using UnityEngine;
[CreateAssetMenu(fileName = "Artefact", menuName = "Scriptable Object/AretefactData")]

public class ArtefactScriptableData : ScriptableObject
{
    public int artefactId;
    public string artefactName;
    public string artefactDesc_Main;
    public Sprite artefactImage;
    
}
