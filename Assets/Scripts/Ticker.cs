using UnityEngine;
using TMPro;

public class Ticker : MonoBehaviour
{
    [SerializeField] Color posColor;
    [SerializeField] Color negColor;

    TMP_Text tmpText;
    void Awake()
    {
        tmpText = GetComponentInChildren<TMP_Text>();
    }

    public void SetText(string text)
    {
        tmpText.text = text;
    }

    public void SetValue(int value)
    {
        tmpText.text = $"{(value>0?'+':'-')}{value}";
        if (value>0)
            SetColor(posColor);
        else
            SetColor(negColor);
    }

    public void SetColor(Color color)
    {
        tmpText.color = color;
    }

    public void SetPosition(Vector2 position)
    {
        transform.localPosition = position;
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}
