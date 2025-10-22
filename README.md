# GPT-UnityProxyDemo  
**OpenAI Proxy + Render + Unity를 이용한 ChatGPT 통신 데모 앱**

## 프로젝트 개요

**GPT-UnityProxyDemo**는 OpenAI API를 Unity 환경에서 안전하게 호출하기 위해  
**Render에 배포된 Proxy 서버**를 통해 ChatGPT와 통신하는 데모 프로젝트입니다.  
Unity 내에서 직접 OpenAI API 키를 노출하지 않고,  
Proxy를 중계 서버로 활용하여 ChatGPT와 실시간 대화를 주고받을 수 있습니다.  

이 프로젝트는 **Unity 환경에서 GPT 모델을 활용한 대화형 AI 인터페이스 구축 방법**을  
직접 학습하고자 하는 목적의 개인 프로젝트입니다.

## 기술 스택

| 구분 | 사용 기술 |
|------|------------|
| **엔진** | Unity **6000.2.7f2** |
| **언어** | C# |
| **AI 통신** | OpenAI API (gpt-4o-mini) |
| **프록시 서버** | [openai-proxy](https://github.com/BUGISU/openai-proxy) (Render에 배포) |
| **네트워크 통신** | UnityWebRequest (POST / JSON) |
| **API 테스트 도구** | Postman |
| **IDE** | Rider, Visual Studio |
| **빌드 대상** | Windows / Android (테스트 완료) |

## 프로젝트 구조

```

GPT-UnityProxyDemo/
├── Scripts/
│   ├── ChatService.cs          # 프록시와의 HTTP 통신, 요청/응답 관리
│   ├── ChatRequestModels.cs    # 요청 구조 (role + message)
│   ├── ResponsesModels.cs      # 응답 구조 (Responses API 대응)
│   └── ChatChoice.cs           # 응답 파싱용 클래스
└── Scenes/
└── ChatScene.unity         # Unity 대화 UI 씬

````
## 💬 주요 기능

### **ChatService.cs**  

- Unity에서 프록시 URL(`Render`)로 **POST 요청**  
- 모델 기본값: `gpt-4o-mini`  
- JSON 요청을 직접 생성 후 `UnityWebRequest`를 이용해 전송  
- 응답(JSON)을 `ResponsesRoot` 구조로 파싱  
  ```csharp
  yield return StartCoroutine(SendRequest(json,
      (raw) => {
          var parsed = JsonUtility.FromJson<ResponsesRoot>(raw);
          onSuccess?.Invoke(parsed);
      },
      (err) => { onError?.Invoke(err); }
  ));
  ````

### **ChatRequestModels.cs**

* OpenAI Responses API에 맞는 요청 데이터 구조 정의
* 여러 메시지(`role`, `message`)를 누적하여 **대화형 요청** 구성

  ```csharp
  public class ChatRequest {
      public string Model = "gpt-4o-mini";
      public List<ChatMessage> Messages = new();
  }
  ```

### **ResponsesModels.cs**

* OpenAI Responses API의 응답(JSON)을 Unity에서 파싱하기 위한 구조체 정의

  ```csharp
  [Serializable]
  public class ResponsesRoot {
      public OutputItem[] output;
  }
  ```

### **Unity 대화 UI**

* InputField로 사용자 입력 → ChatService 호출
* TextMeshPro 기반 UI에 GPT 응답 표시
* API 지연 처리, 네트워크 오류 처리 로직 포함

## 동작 흐름

```
[Unity Client]
   ↓ POST /api/proxy/responses
[Render Proxy Server]
   ↓ (API Key 보호)
[OpenAI API]
   ↓
응답(JSON) → Unity 파싱 → 대화 표시
```

## 사용 방법

### Proxy 서버 실행 (Render or Local)

* 프록시 리포지토리: [https://github.com/BUGISU/openai-proxy](https://github.com/BUGISU/openai-proxy)
* Render에 배포된 URL 예시:

  ```
  https://openai-proxy-ta92.onrender.com/api/proxy/responses
  ```

### Unity 설정

1. `ChatService` 스크립트를 Canvas에 추가
2. `ProxyUrl` 필드에 Render URL 입력
3. UI에 InputField와 TextMeshPro 텍스트 연결
4. `SendChatRequest()` 호출로 메시지 전송

## 시연 화면

| **1️⃣ Render Proxy 서버 배포 (onRender)** | **2️⃣ Postman 테스트 (응답 200 OK)** |
|----------------------------------|--------------------------------|
| <img src="./docs/스크린샷%202025-10-22%20144501.png" width="400"/> | <img src="./docs/스크린샷%202025-10-22%20145338.png" width="400"/> |

| **3️⃣ Unity → Proxy 요청 전송 (ChatService)** | **4️⃣ Unity에서 GPT 응답 출력** |
|------------------------------------|--------------------------------|
| <img src="./docs/스크린샷%202025-10-22%20145425.png" width="400"/> | <img src="./docs/스크린샷%202025-10-22%20150051.png" width="400"/> |

> ✅ Unity → Proxy → OpenAI API 전체 흐름이 정상 작동하며,  
> Proxy 서버를 통해 안전하게 ChatGPT와 실시간 대화가 이루어집니다.

## 개발 특징

* Unity 내에서 **직접 OpenAI API Key를 노출하지 않음**
* Render Proxy를 이용한 **보안형 중계 통신 구조**
* **JsonUtility** 기반 직렬화/역직렬화 처리
* API 지연 시 예외 처리 및 콜백 방식의 응답 핸들링
* 간단한 UI 기반으로 **ChatGPT와 동일한 대화 흐름** 구현


## 실행 예시 코드

```csharp
ChatRequest req = new ChatRequest();
req.Messages.Add(new ChatMessage("user", "안녕 GPT! 오늘 날씨 어때?"));

StartCoroutine(chatService.SendChatRequest(req,
    (resp) => {
        string text = resp.output[0].content[0].text;
        uiText.text = $"GPT: {text}";
    },
    (err) => {
        Debug.LogError("요청 실패: " + err);
    }
));
```

## 참고 자료

* 🔗 **Proxy Repository:** [https://github.com/BUGISU/openai-proxy](https://github.com/BUGISU/openai-proxy)
* 📘 **OpenAI Responses API Docs:** [https://api.openai.com/v1/responses](https://api.openai.com/v1/responses)

## 결과
✅ Render 기반 Proxy를 통한 안전한 OpenAI 통신 구조 구현
✅ Unity에서 OpenAI API를 직접 제어하는 **ChatGPT 스타일 대화형 인터페이스 완성**
✅ Unity 환경에서의 GPT 활용 구조 학습 및 확장 가능성 검증


