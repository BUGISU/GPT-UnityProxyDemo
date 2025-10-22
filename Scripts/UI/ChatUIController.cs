using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatUIController : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField InputField;
    public Button SendButton;
    public RectTransform Content; // ScrollRect.content
    public GameObject ChatBubblePrefab;

    [Header("Network")]
    public ChatService chatService;

    private void Start()
    {
        SendButton.onClick.AddListener(OnSendClicked);
    }

    private void OnSendClicked()
    {
        string text = InputField.text?.Trim();
        if (string.IsNullOrEmpty(text)) return;

        // 1. 화면에 사용자 메시지 표시
        SpawnBubble(text, true);

        // 2. 요청 만들기 (간단한 한 턴)
        ChatRequest request = new ChatRequest();
        request.Model = "gpt-4o-mini";
        request.Messages.Add(new ChatMessage("user", text));
        request.MaxTokens = 256;

        // 3. 전송 (UI 비활성)
        SendButton.interactable = false;
        StartCoroutine(chatService.SendChatRequest(request, OnSuccessParsed, OnError));

        InputField.text = "";
    }

    private void OnSuccessParsed(ResponsesRoot resp)
    {
        SendButton.interactable = true;
        try {
            string assistantText = "[응답 없음]";
            if (resp != null && resp.output != null && resp.output.Length > 0) {
                var outItem = resp.output[0];
                if (outItem != null && outItem.content != null && outItem.content.Length > 0) {
                    assistantText = outItem.content[0].text ?? "[빈 텍스트]";
                }
            }
            SpawnBubble(assistantText, false);
        } catch (System.Exception ex) {
            OnError("Parsing error: " + ex.Message);
        }

        Canvas.ForceUpdateCanvases();
    }

    private void OnError(string err)
    {
        SendButton.interactable = true;
        Debug.LogError("Chat Error: " + err);
        SpawnBubble("[에러] " + err, false);
    }

    private void SpawnBubble(string text, bool isUser)
    {
        GameObject go = Instantiate(ChatBubblePrefab, Content);
        var bubble = go.GetComponent<ChatBubble>();
        if (bubble != null) bubble.SetText(text, isUser);
    }
}
