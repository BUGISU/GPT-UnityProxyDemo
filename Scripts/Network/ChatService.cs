using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ChatService : MonoBehaviour
{
    [Header("Proxy (권장)")]
    public string ProxyUrl = "https://openai-proxy-ta92.onrender.com/api/proxy/responses";
    public string ProxyKey = ""; // 프록시가 별도 키를 요구하면 여기에
    public int Timeout = 60;

    // SendChatRequest: ChatRequest -> ResponsesRequest -> POST -> parse ResponsesRoot -> callback
    public IEnumerator SendChatRequest(ChatRequest req, Action<ResponsesRoot> onSuccess, Action<string> onError)
    {
        if (req == null || req.Messages == null || req.Messages.Count == 0) {
            onError?.Invoke("No messages to send");
            yield break;
        }

        // build ResponsesRequest body
        var inputs = new InputItem[req.Messages.Count];
        for (int i=0;i<req.Messages.Count;i++) {
            inputs[i] = new InputItem {
                role = req.Messages[i].Role,
                content = new RequestContent[] {
                    new RequestContent { type = "input_text", text = req.Messages[i].Message }
                }
            };
        }

        var body = new ResponsesRequest {
            model = string.IsNullOrEmpty(req.Model) ? "gpt-4o-mini" : req.Model,
            input = inputs
        };

        string json = JsonUtility.ToJson(body);
        Debug.Log($"[ChatService] Sending JSON to proxy: {json}");

        yield return StartCoroutine(SendRequest(json,
            (raw) => {
                Debug.Log("[ChatService] Raw response from proxy: " + raw);
                try {
                    var parsed = JsonUtility.FromJson<ResponsesRoot>(raw);
                    onSuccess?.Invoke(parsed);
                } catch (Exception ex) {
                    onError?.Invoke("Failed to parse response: " + ex.Message + "\nRaw: " + raw);
                }
            },
            (err) => {
                onError?.Invoke(err);
            }
        ));
    }

    // 기존 SendRequest: 문자열 바디 보내고 raw text 반환
    public IEnumerator SendRequest(string jsonBody, System.Action<string> onSuccess, System.Action<string> onError)
    {
        // 1. Trim & debug print
        string url = (ProxyUrl ?? "").Trim();
        Debug.Log($"[ChatService] ProxyUrl -> '{url}'");

        // 2. Validate URL
        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
            onError?.Invoke("Malformed URL: " + url);
            yield break;
        }

        using (var uwr = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            uwr.uploadHandler = new UploadHandlerRaw(bodyRaw);
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");

            if (!string.IsNullOrEmpty(ProxyKey))
            {
                uwr.SetRequestHeader("X-Proxy-Key", ProxyKey);
            }

            uwr.timeout = Timeout;

            yield return uwr.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            bool success = (uwr.result == UnityWebRequest.Result.Success);
#else
            bool success = (!uwr.isNetworkError && !uwr.isHttpError);
#endif

            if (success)
            {
                onSuccess?.Invoke(uwr.downloadHandler.text);
            }
            else
            {
                string resp = uwr.downloadHandler != null ? uwr.downloadHandler.text : "<no body>";
                onError?.Invoke($"Error {uwr.responseCode}: {uwr.error}\n{resp}");
                Debug.LogError($"[ChatService] Request failed. Code:{uwr.responseCode}, err:{uwr.error}, body:{resp}");
            }
        }
    }
}
