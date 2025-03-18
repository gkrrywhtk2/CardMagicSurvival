using UnityEngine;

public class Cheat : MonoBehaviour
{
    public void GoldCheat(){
        GameManager.instance.dataManager.goldPoint += 99999;
    }
}
