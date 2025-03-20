using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionBT :MonoBehaviour
{
    // написал класс для кнопок пород, чтобы можно было управлять текстом и индикатором
    [SerializeField] GameObject Indicator;
    public Button button;
    public TextMeshProUGUI text;
    public void ShowIndicator(bool t)
    {
        Indicator.gameObject.SetActive(t);
    }
}