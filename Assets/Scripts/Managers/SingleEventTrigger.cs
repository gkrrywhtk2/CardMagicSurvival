using UnityEngine;
using UnityEngine.EventSystems;

public class SingleEventTrigger : MonoBehaviour
{
    public static SingleEventTrigger single;
    public bool oneTouch = false; 

    private void Awake() {
        single = this;
    }
}
