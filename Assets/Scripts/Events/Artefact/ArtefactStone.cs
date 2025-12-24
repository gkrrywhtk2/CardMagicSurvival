using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ArtefactStone : MonoBehaviour
{
    public Animator AreaAnim;
    private Collider2D stoneCollider;
    public Artefact_Object[] artefact_Objects;
    SpriteRenderer spriteRenderer;
    public GameObject artefactStoneEffect;
    public GameObject shadowEffect;

    [Header("Description UI")]
    public GameObject DescFrame;
    public Transform[] descPos;//설명창 위치 배열
    public TMP_Text titleText;
    public TMP_Text descriptionText;


    // InGameArtefactManager를 참조하기 위한 변수
    private InGameArtefactManager artefactManager;

   

    private void Start()
    {
        stoneCollider = GetComponent<Collider2D>();
        artefactManager = GameManager.instance.inGameArtefactManager;
        spriteRenderer = GetComponent<SpriteRenderer>();
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

        // 먼저 모든 선택지 오브젝트를 비활성화 상태로 시작
        foreach (var obj in artefact_Objects) obj.gameObject.SetActive(false);

        //플레이어의 이동 속도 0으로
        GameManager.instance.player.playerStatus.playerMoveSpeed.LockMovement();

        yield return new WaitForSeconds(1f);

        if (artefactManager == null || artefactManager.AllArtefacts.Count == 0) yield break;

     

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
             artefact_Objects[i].gameObject.SetActive(true);
            artefact_Objects[i].Init(selectedData); 

            // TODO: 여기서 artefact_Objects[i]의 애니메이션(예: Fade-in 또는 Scale Up)을 
            // 실행해주면 연출이 훨씬 고급스러워집니다.
        }

         //플레이어의 이동 금지 해제
        GameManager.instance.player.playerStatus.playerMoveSpeed.UnlockMovement();
    
    }

   

    public void EndArtefactEvent()
    {

        // 모든 선택지 오브젝트 비활성화
       foreach (var obj in artefact_Objects)
        {
            if (obj != null)
                obj.gameObject.SetActive(false);
        }

        //충돌 비활성화
        stoneCollider.enabled = false;

        //아티팩트 스톤 이펙트 비활성화
        if (artefactStoneEffect != null)
        {
            artefactStoneEffect.SetActive(false);
            shadowEffect.SetActive(false);
        }

       // ✅ Stone 투명화 (Color는 struct라 새로 할당)
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 0f;
            spriteRenderer.color = color;
        }
        GameManager.instance.player.playerStatus.EndArtefactEvent();
        AreaAnim.SetBool("AreaEvent", false);
    }

    private void OnEnable()
    {
        if (stoneCollider != null)
            stoneCollider.enabled = true;

        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }

        //아티팩트 스톤 이펙트 활성화
        if (artefactStoneEffect != null)
        {
            artefactStoneEffect.SetActive(true);
            shadowEffect.SetActive(true);
        }
    }
    public void ShowDesc(int index)
    {
        DescFrame.transform.position = descPos[index].position;
        titleText.text = artefact_Objects[index].GetName();
        descriptionText.text = artefact_Objects[index].GetDesc();
    }
    public void HideDesc()
    {
        DescFrame.transform.position = new Vector3(10000, 10000, 0);
    }

}