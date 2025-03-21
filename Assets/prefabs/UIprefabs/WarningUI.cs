using TMPro;
using UnityEngine;

public class WarningUI : MonoBehaviour
{
    public TMP_Text text;
    void Awake()
    {
        text = GetComponentInChildren<TMP_Text>();
    }
}
