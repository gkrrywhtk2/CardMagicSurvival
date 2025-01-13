using UnityEngine;

public class StopCircle : MonoBehaviour
{

    Transform player;
    private void Awake() {
        player = GameManager.instance.player.dirFront.playerTransform.GetComponent<Transform>();
    }
    private void FixedUpdate() {
        transform.position = player.transform.position;
    }
}
