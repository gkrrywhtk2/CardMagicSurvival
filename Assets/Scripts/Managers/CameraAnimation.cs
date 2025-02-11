using UnityEngine;

public class CameraAnimation : MonoBehaviour
{
    Animator anim;
    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    public void BossAnimOff(){
         anim.SetBool("Boss", false);
    }
}
