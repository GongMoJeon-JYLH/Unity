using System;
using System.Collections;
using System.Net.Http;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.VolumeComponent;

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
}

[System.Serializable]
public struct ChatData
{
    public string userMessage;
    public string userId;
}

// ���� ������ ������
[System.Serializable]
public struct FullLoginData
{
    public string name;
    public string userId;
}

[System.Serializable]
public struct ChatResponse
{
    public string responseText;
    public bool canRecommend;
}

[System.Serializable]
public struct BookListResponse
{
    public BookResponse[] recommendations;
    public string[] keywords;
    public string userType;
    public string userTypeReason;
}

[System.Serializable]
public struct BookResponse
{
    public string bookTitle;
    public string bookKeyword;
    public string imageUrl;
    public string bookUrl;
    public string bookSummary;
    public string bookGenre;
}


public class HttpManager : MonoBehaviour
{
    private static HttpManager instance = null;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static HttpManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    public GameObject nameBox;
    public GameObject inputBox;
    public GameObject outputBox;
    public UnityEngine.UI.Button btn_expand;
    public UnityEngine.UI.Button btn_getRec;
    public Rec2Setting toggleSetting;

    public UnityEngine.UI.Image[] coverImages;

    public string server = ""; // �����Ϳ��� ���� 

    // UserInfo
    FullLoginData thisUserInfo = new FullLoginData
    {
        name = "",
        userId = ""
    };

    #region ��ư�� �ٴ� �Լ���

    // �Է��� �г��� ������ �����κ��� ������ȣ ����
    public void OnClickSendUserIdAndGetUserNum()
    {
        LoginData loginData = new LoginData
        {
            name = nameBox.GetComponent<TMP_InputField>().text,
        };

        HttpInfo info = new HttpInfo
        {
            url = server + "/users",
            method = "POST",
            body = JsonUtility.ToJson(loginData),
            contentType = "application/json",
            responseType = ResponseType.UserInfo
        };

        StartCoroutine(SendRequest(info, result =>
        {
            Debug.Log("���� ���� �� ��������");

            if (result == null)
            {
                Debug.LogError("������ null�Դϴ�.");
                return;
            }

            FullLoginData response = (FullLoginData)result;
            thisUserInfo.name = response.name;
            thisUserInfo.userId = response.userId;
        }));
    }

    // ê�� ������ �亯�� ����
    public void OnClickSendChat()
    {
        //string userMessage = inputBox.GetComponent<TMP_InputField>().text;
        string userMessage = UIManager.Instance.inputChat.GetComponent<TMP_InputField>().text;

        // �� �޽����� ����
        if (string.IsNullOrWhiteSpace(userMessage))
            return;

        UIManager.Instance.loadingBar.SetActive(true);
        GameObject userChat = GameObject.Instantiate(UIManager.Instance.userChat, UIManager.Instance.content.transform);
        userChat.GetComponent<TextMeshProUGUI>().text += userMessage;


        ChatData chatData = new ChatData
        {
            userMessage = userMessage,
            userId = thisUserInfo.userId
        };

        HttpInfo info = new HttpInfo
        {
            url = server + "/chat",
            method = "POST",
            body = JsonUtility.ToJson(chatData),
            contentType = "application/json",
            responseType = ResponseType.Chat
        };

        StartCoroutine(SendRequest(info, result =>
        {
            ChatResponse response = (ChatResponse)result;
            outputBox.GetComponent<TextMeshProUGUI>().text = response.responseText;

            GameObject aiChat = GameObject.Instantiate(UIManager.Instance.aiChat, UIManager.Instance.content.transform);
            userChat.GetComponent<TextMeshProUGUI>().text += response.responseText;

            UIManager.Instance.loadingBar.SetActive(false);


            if (response.canRecommend)
            {
                btn_getRec.interactable = true;
            }

            inputBox.GetComponent<TMP_InputField>().text = "";

        }));
    }

    // �޾ƿ� å ��õ �ϴ� �迭�� ����, �� �� ������Ʈ�� ������ UI ��ҵ鵵 �޾ƿ�
    public BookResponse[] books = new BookResponse[3];
    public UnityEngine.UI.Image coverImage;

    const int INDENT = 150;


    public BookListResponse list;

    // �α��� �����͸� ������ å ��õ�� �޾ƿ�
    public void OnClickGetBookRecommendation()
    {
        GameObject go = Instantiate(UIManager.Instance.text_keyword, UIManager.Instance.keywordPanel.transform);

        // Ű���� �ؽ�Ʈ ������ 
        go.GetComponent<TextMeshProUGUI>().text = "��Ÿ��???";

        // Ű���� ��ġ ����
        go.GetComponent<RectTransform>().anchoredPosition = new Vector2(UnityEngine.Random.Range(-270.0f, 270.0f), UnityEngine.Random.Range(-140.0f, 140.0f));
        go.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(-70.0f, 70.0f));



        HttpInfo info = new HttpInfo
        {
            url = server + "/book-recommend",
            method = "POST",
            responseType = ResponseType.Book,
            body = JsonUtility.ToJson(thisUserInfo),
            contentType= "application/json"
        };

        StartCoroutine(SendRequest(info, result =>
        {
            btn_expand.interactable = true;
            books = (BookResponse[])result;


            var firstBook = books[0];
            string[] texts = new string[]
            {
                $"����<indent={INDENT}>: </indent>", firstBook.bookTitle, $"Ű����<indent={INDENT}>: </indent>", firstBook.bookGenre, $"���� ���<indent={INDENT}>: </indent>", firstBook.bookSummary, $"��ũ<indent={INDENT}>: </indent>", firstBook.bookUrl
                //$"���� : ", firstBook.bookTitle, $"Ű���� : ", firstBook.bookGenre, $"���� ��� : ", firstBook.bookSummary, $"��ũ :", firstBook.bookUrl
            };

            //toggleSetting.SetBookUI(0, coverImage);
            toggleSetting.SetTextBoxes(texts);

            list = (BookListResponse)result;



            for (int i = 0; i < list.keywords.Length; i++)
            {
                GameObject go = Instantiate(UIManager.Instance.text_keyword, UIManager.Instance.keywordPanel.transform);

                // Ű���� �ؽ�Ʈ ������ 
                go.GetComponent<TextMeshProUGUI>().text = list.keywords[i];

                // Ű���� ��ġ ����
                go.GetComponent<RectTransform>().anchoredPosition = new Vector2(UnityEngine.Random.Range(-270.0f, 270.0f), UnityEngine.Random.Range(-140.0f, 140.0f));
                go.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(-70.0f, 70.0f));

            }

            toggleSetting.SetBookUI(0, coverImage);

            StartCoroutine(LoadImageFromUrl(books[0].imageUrl, coverImage));

            //StringBuilder sb = new StringBuilder();
            //foreach (BookResponse book in books)
            //{
            //    sb.AppendLine($"<b>å �帣</b> : {book.bookGenre}\n<b>��õ����</b> : {book.bookReason}\n<b>���� ���</b> : {book.bookSummary}\n<b>��ũ</b> :{book.bookUrl}\n<b>��ũ</b> :{book.bookUrl}");
            //}

            //outputBox.GetComponent<TextMeshProUGUI>().text = sb.ToString();

            //Debug.Log(books[0].imageUrl);

            for (int i = 0; i < books.Length; i++)
            {
                var book = books[i];
                StartCoroutine(LoadImageFromUrl(book.imageUrl, coverImages[i]));
                coverImages[i].transform.GetChild(1).GetComponent<TMP_Text>().text = $"<{book.bookTitle}>\n"; // ���߿� �۰� �̸� ���ϱ� ////////////////
            }
        }));
    }

    #endregion

    // �̹��� URL �� �޾ƿ� �̹����� �ε�
    public IEnumerator LoadImageFromUrl(string url, UnityEngine.UI.Image coverImage)
    {
        // ���������� https�� ���� ����
        if (url.StartsWith("http://"))
            url = "https://" + url.Substring(7);

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                coverImage.sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );
            }
            else
            {
                Debug.LogError($"�̹��� �ҷ����� ����: {request.error}");
            }
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
                    FullLoginData fullLoginData = JsonUtility.FromJson<FullLoginData>(request.downloadHandler.text);
                    onSuccess?.Invoke(fullLoginData); 
                    break;
                case ResponseType.Chat:
                    ChatResponse chat = JsonUtility.FromJson<ChatResponse>(request.downloadHandler.text);
                    onSuccess?.Invoke(chat);
                    break;
                case ResponseType.Book:
                    BookListResponse bookList = JsonUtility.FromJson<BookListResponse>(request.downloadHandler.text);
                    onSuccess?.Invoke(bookList.recommendations);
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
