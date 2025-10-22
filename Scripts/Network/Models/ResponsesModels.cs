using System;

[Serializable]
public class ResponsesRequest
{
    public string model;
    public InputItem[] input;
}

[Serializable]
public class InputItem
{
    public string role;
    public RequestContent[] content;
}

[Serializable]
public class RequestContent
{
    public string type;
    public string text;
}

// ------------------ 응답 모델 ------------------

[Serializable]
public class ResponsesRoot
{
    public OutputItem[] output;
    // 필요하면 더 많은 필드 추가 가능 (id, model 등)
}

[Serializable]
public class OutputItem
{
    public string id;
    public string type; // "message"
    public string status; // "completed"
    public ResponseContent[] content;
    public string role; // "assistant"
}

[Serializable]
public class ResponseContent
{
    public string type;  // "output_text"
    public string text;  // 실제 출력 텍스트
}