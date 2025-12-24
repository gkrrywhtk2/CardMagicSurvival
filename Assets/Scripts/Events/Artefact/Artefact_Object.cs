using UnityEngine;
using UnityEngine.UI;

public class Artefact_Object : MonoBehaviour
{
    
    public SpriteRenderer image;
    public ArtefactInstance aretefactInstance;
    public ArtefactStone artefactStone;
    private void Awake()
    {
      
    }


  public void Init(ArtefactInstance artefact)
    {
        aretefactInstance = artefact;
        image.sprite = GameManager.instance.inGameArtefactManager.artefactScriptableDatas[aretefactInstance.ID].artefactImage;
    }
    public void EndEvent()
    {
        artefactStone.EndArtefactEvent();
    }
    public string GetName()
    {
        return GameManager.instance.inGameArtefactManager.artefactScriptableDatas[aretefactInstance.ID].artefactName;
    }
     public string GetDesc()
    {
        return GameManager.instance.inGameArtefactManager.artefactScriptableDatas[aretefactInstance.ID].artefactDesc_Main;
    }
}
