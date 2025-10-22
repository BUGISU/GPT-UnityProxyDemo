// ChatChoice.cs
using Newtonsoft.Json;

[System.Serializable]
public class ChatChoice
{
    // 이 구조는 ChatUIController에서 resp.Choices[0].Message.Content로 접근하는 걸 맞추기 위함
    // Responses API 결과 포맷에 따라 조정 필요(아래는 일반적인 mapping)
    [JsonProperty("message")]
    public ChatMessageChoice Message { get; set; }
}

[System.Serializable]
public class ChatMessageChoice
{
    // 여기선 assistant가 준 텍스트를 담는 단순한 Content 필드로 둠
    [JsonProperty("content")]
    public string Content { get; set; }
}