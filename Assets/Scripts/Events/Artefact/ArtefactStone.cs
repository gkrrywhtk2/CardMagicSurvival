using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArtefactStone : MonoBehaviour
{
    public Animator AreaAnim;
    private Collider2D stoneCollider;


    // InGameArtefactManager를 참조하기 위한 변수
   

    private void Awake()
    {
        stoneCollider = GetComponent<Collider2D>();
        
    }

    public void TriggerArtefactEvent()
    {
        StartCoroutine(StartArtefactEventRoutine());
    }

    private IEnumerator StartArtefactEventRoutine()
    {
        if (stoneCollider != null)
            stoneCollider.enabled = false;

        GameManager.instance.player.playerStatus.StartArtefactEvent();
        AreaAnim.SetBool("AreaEvent", true);

        yield return new WaitForSeconds(1f);

        GameManager.instance.inGameArtefactManager.artefactSelect.CallSetRandomArtefacts();
    }

   

    public void EndArtefactEvent()
    {
        GameManager.instance.player.playerStatus.EndArtefactEvent();
        AreaAnim.SetBool("AreaEvent", false);
    }

    private void OnEnable()
    {
        if (stoneCollider != null)
            stoneCollider.enabled = true;
    }
}