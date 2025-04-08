using System.Collections;
using System.Net.Http;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum ResponseType
{
    UserInfo,
    Chat,
    Book
}

[System.Serializable]
public struct HttpInfo
{
    public string url;
    public string method; // "GET" or "POST"
    public string body;
    public string contentType;
    public ResponseType responseType;
}

// ��û ������ ������
[System.Serializable]
public struct LoginData
{
    public string name;
    public int idNum;
}

[System.Serializable]
public struct ChatData
{
    public string userMessage;
}

// ���� ������ ������
[System.Serializable]
public struct ChatResponse
{
    public string responseText;
}

[System.Serializable]
public struct BookResponse
{
    public string bookTitle;
    public string bookReason;
    public string imageUrl;
    public string bookUrl;
}


public class HttpManager : MonoBehaviour
{
    public GameObject nameBox;
    public GameObject inputBox;
    public GameObject outputBox;
    public Image coverImage;
    string server = "";

    private void Start()
    {
        server = "";
    }

    #region ��ư�� �ٴ� �Լ���

    public void OnClickSendUserIdAndUserNum()
    {
        LoginData loginData = new LoginData
        {
            name = nameBox.GetComponent<TMP_InputField>().text,
            idNum = 1
        };

        HttpInfo info = new HttpInfo
        {
            url = server,
            method = "POST",
            body = JsonUtility.ToJson(loginData),
            contentType = "application/json",
            responseType = ResponseType.UserInfo
        };

        StartCoroutine(SendRequest(info, result =>
        {
            Debug.Log("���� ���� �� ��������");
        }));
    }

    public void OnClickSendChat()
    {
        string userMessage = inputBox.GetComponent<TMP_InputField>().text;

        // �� �޽����� ����
        if (string.IsNullOrWhiteSpace(userMessage))
            return;

        ChatData chatData = new ChatData
        {
            userMessage = userMessage
        };

        HttpInfo info = new HttpInfo
        {
            url = server,
            method = "POST",
            body = JsonUtility.ToJson(chatData),
            contentType = "application/json",
            responseType = ResponseType.Chat
        };

        StartCoroutine(SendRequest(info, result =>
        {
            ChatResponse response = (ChatResponse)result;
            outputBox.GetComponent<TextMeshProUGUI>().text = response.responseText;
        }));
    }


    public void OnClickGetBookRecommendation()
    {
        HttpInfo info = new HttpInfo
        {
            url = server,
            method = "GET",
            responseType = ResponseType.Book
        };

        StartCoroutine(SendRequest(info, result =>
        {
            BookResponse book = (BookResponse)result;
            outputBox.GetComponent<TextMeshProUGUI>().text = book.bookTitle + " ------ " + book.bookReason;
            StartCoroutine(LoadImageFromUrl(book.imageUrl));
        }));
    }
    #endregion

    private IEnumerator LoadImageFromUrl(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            coverImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
        }
        else
        {
            Debug.LogError("�̹��� �ҷ����� ����: " + request.error);
        }
    }



    #region ��� ���� �Լ�
    // ��û�� ���� ���� Ÿ�� �ٸ� -> DoneRequest ��� Callback(onSuccess) ��� 
    public IEnumerator SendRequest(HttpInfo info, System.Action<object> onSuccess)
    {
        UnityWebRequest request;

        if (info.method == "POST")
        {
            request = new UnityWebRequest(info.url, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(info.body);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", info.contentType);
        }
        else
        {
            request = UnityWebRequest.Get(info.url);
            request.downloadHandler = new DownloadHandlerBuffer();
        }

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            switch (info.responseType)
            {
                case ResponseType.UserInfo:
                    onSuccess?.Invoke(null); 
                    break;
                case ResponseType.Chat:
                    ChatResponse chat = JsonUtility.FromJson<ChatResponse>(request.downloadHandler.text);
                    onSuccess?.Invoke(chat);
                    break;
                case ResponseType.Book:
                    BookResponse book = JsonUtility.FromJson<BookResponse>(request.downloadHandler.text);
                    onSuccess?.Invoke(book);
                    break;
                default:
                    Debug.Log("���� Ÿ�� ����");
                    break;
            }
        }
        else
        {
            Debug.LogError("��û ����: " + request.error);
        }
    }

    #endregion


}
