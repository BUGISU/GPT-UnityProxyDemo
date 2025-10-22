using System;
using System.Collections.Generic;

[Serializable]
public class ChatRequest
{
    public string Model = "gpt-4o-mini";
    public List<ChatMessage> Messages = new List<ChatMessage>();
    public int MaxTokens = 256;
}

[Serializable]
public class ChatMessage
{
    // Unity Inspector로도 노출될 수 있게 public 프로퍼티로 둠
    public string Role;
    public string Message;

    public ChatMessage() { }
    public ChatMessage(string role, string message) { Role = role; Message = message; }
}