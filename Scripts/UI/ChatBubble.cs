using UnityEngine;
using TMPro;

public class ChatBubble : MonoBehaviour
{
    public TextMeshProUGUI TextLabel;
    public RectTransform Root;

    public void SetText(string text, bool isUser)
    {
        if (TextLabel != null) TextLabel.text = text;
        // 여기서 isUser에 따라 정렬/색상 바꿔도 됨
        if (isUser) {
            Root.anchorMin = new Vector2(1, Root.anchorMin.y);
            Root.anchorMax = new Vector2(1, Root.anchorMax.y);
            Root.pivot = new Vector2(1, Root.pivot.y);
        } else {
            Root.anchorMin = new Vector2(0, Root.anchorMin.y);
            Root.anchorMax = new Vector2(0, Root.anchorMax.y);
            Root.pivot = new Vector2(0, Root.pivot.y);
        }
    }
}