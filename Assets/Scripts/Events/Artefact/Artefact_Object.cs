using UnityEngine;
using UnityEngine.UI;

public class Artefact_Object : MonoBehaviour
{
    public ArtefactInstance artefactInstance;
    public GameObject image;
    

    public void Init(ArtefactInstance artefactInstance)
    {
        this.artefactInstance = artefactInstance;
        image.GetComponent<Image>().sprite = GameManager.instance.inGameArtefactManager.artefactScriptableDatas[artefactInstance.ID].artefactImage;
    }
}
