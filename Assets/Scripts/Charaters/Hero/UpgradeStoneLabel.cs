using UnityEngine;
using TMPro;

public class UpgradeStoneLabel : MonoBehaviour
{
   [SerializeField] private TMP_Text stoneText;

    private void OnEnable()
    {
        ServerDataManager.OnUpgradeStoneChanged += UpdateStoneUI;

        if (ServerDataManager.instance != null)
            UpdateStoneUI(ServerDataManager.instance.GetCurrentUpgradeStone());
    }

    private void OnDisable()
    {
        ServerDataManager.OnUpgradeStoneChanged -= UpdateStoneUI;
    }

    private void UpdateStoneUI(int v)
    {
        if (stoneText != null)
            stoneText.text = v.ToString();
    }
}
