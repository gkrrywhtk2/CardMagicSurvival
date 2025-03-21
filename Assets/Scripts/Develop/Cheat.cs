using UnityEngine;

public class Cheat : MonoBehaviour
{
    public void GoldCheat(){
        GameManager.instance.dataManager.goldPoint += 99999;
        GameManager.instance.dataManager.traningData.expPoint += 999999;
    }
}
