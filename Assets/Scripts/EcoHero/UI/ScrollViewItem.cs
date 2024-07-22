using UnityEngine;
using UnityEngine.UI;

using Text = TMPro.TextMeshProUGUI;

public class ScrollViewItem : MonoBehaviour
{
    public Text itemText;

    public void SetData(string text)
    {
        // Debug.Log(text);
        itemText.text = text;
    }
}