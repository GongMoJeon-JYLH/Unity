using Michsky.MUIP;
using System;
using System.Collections;
using System.Net.Http;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.U2D;
using UnityEngine.UI;
//using UnityEngine.UIElements;
using static ScrollViewTest;
using static System.Net.Mime.MediaTypeNames;
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

// 보내는 구조체
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

// 받아오는 구조체
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
    public string author;
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
    public GameObject btn_expand;
    public GameObject btn_getRec;
    public Rec2Setting toggleSetting;

    public UnityEngine.UI.Image[] coverImages;

    public string server = ""; // 에디터에서 조정  

    private void Start()
    {
        btn_expand.GetComponent<ButtonManager>().isInteractable = false;
        btn_getRec.GetComponent<ButtonManager>().isInteractable = false;

    }

    //private void Update()
    //{
    //    if(btn_getRec)
    //    {
    //        if(btn_getRec.GetComponent<ButtonManager>().isInteractable == false)
    //        {
    //            btn_getRec.GetComponent<ButtonManager>().
    //        }
    //    }
    //}

    // UserInfo
    FullLoginData thisUserInfo = new FullLoginData
    {
        name = "",
        userId = ""
    };

    #region 버튼에 붙는 함수들

    // 입력한 닉네임 보내고 서버로부터 고유번호 받음
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
            Debug.Log("유저 정보 잘 보내졌다");

            if (result == null)
            {
                Debug.LogError("응답이 null입니다.");
                return;
            }

            FullLoginData response = (FullLoginData)result;
            thisUserInfo.name = response.name;
            thisUserInfo.userId = response.userId;
            UIManager.Instance.PanelChange(false);

        }));
    }

    // 챗을 보내고 답변을 받음
    public void OnClickSendChat()
    {
        //string userMessage = inputBox.GetComponent<TMP_InputField>().text;
        string userMessage = UIManager.Instance.inputChat.GetComponent<TMP_InputField>().text;

        // 빈 메시지는 무시
        if (string.IsNullOrWhiteSpace(userMessage))
            return;


        UIManager.Instance.inputChat.interactable = false;
        UIManager.Instance.loadingBar.SetActive(true);
        //UIManager.Instance.tmp_Chat.text += "\n <color=blue>나</color>:" + userMessage;
        UIManager.Instance.btn_clickChat.GetComponent<ButtonManager>().isInteractable = false;
        //GameObject userChatObject = GameObject.Instantiate(UIManager.Instance.aiChat, UIManager.Instance.content.transform);
        //Image image = userChatObject.transform.GetChild(1).GetComponent<Image>();
        //image.sprite = UIManager.Instance.userIcon;
        //userChatObject.GetComponentInChildren<TextMeshProUGUI>().text = ": " + userMessage;

        UIManager.Instance.SetHeight(": " + userMessage, UIManager.Instance.userIcon, UIManager.Instance.userBalloon);


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
            //outputBox.GetComponent<TextMeshProUGUI>().text = response.responseText;
            //UIManager.Instance.tmp_Chat.text += "\n <color=#00FFFF>추천봇</color>:" + response.responseText;
            //GameObject aiChatObject = GameObject.Instantiate(UIManager.Instance.aiChat, UIManager.Instance.content.transform);
            //Image image = userChatObject.transform.GetChild(1).GetComponent<Image>();
            //image.sprite = UIManager.Instance.aIcon;
            //aiChatObject.GetComponentInChildren<TextMeshProUGUI>().text = ": " + response.responseText;

            UIManager.Instance.SetHeight(": " + response.responseText, UIManager.Instance.aIcon, UIManager.Instance.aiBalloon);


            UIManager.Instance.loadingBar.SetActive(false);

            print(response.canRecommend);
            
            btn_getRec.GetComponent<ButtonManager>().isInteractable = response.canRecommend;
            UIManager.Instance.inputChat.GetComponent<TMP_InputField>().text = "";
            UIManager.Instance.btn_clickChat.GetComponent<ButtonManager>().isInteractable = true;
            UIManager.Instance.inputChat.interactable = true;



        }));
    }

    // 받아온 책 추천 일단 배열로 저장, 그 외 업데이트할 나머지 UI 요소들도 받아옴
    public BookResponse[] books = new BookResponse[3];
    public UnityEngine.UI.Image coverImage;

    const int INDENT = 150;


    public BookListResponse list;

    // 로그인 데이터를 보내고 책 추천을 받아옴
    public void OnClickGetBookRecommendation()
    {
        //GameObject go = Instantiate(UIManager.Instance.text_keyword, UIManager.Instance.keywordPanel.transform);

        // Ű���� �ؽ�Ʈ ������ 
        //go.GetComponent<TextMeshProUGUI>().text = "��Ÿ��???";

        // Ű���� ��ġ ����
        //go.GetComponent<RectTransform>().anchoredPosition = new Vector2(UnityEngine.Random.Range(-270.0f, 270.0f), UnityEngine.Random.Range(-140.0f, 140.0f));
        //go.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(-70.0f, 70.0f));
        UIManager.Instance.loadingBar.SetActive(true);


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
            print("뭘로?");
            btn_expand.GetComponent<ButtonManager>().isInteractable = true;
            list = (BookListResponse)result;
            books = list.recommendations;

            for (int i = 0; i < list.keywords.Length; i++)
            {
                GameObject go = Instantiate(UIManager.Instance.text_keyword, UIManager.Instance.keywordPanel.transform);

                // Ű���� �ؽ�Ʈ ������ 
                go.GetComponent<TextMeshProUGUI>().text = list.keywords[i];

                // Ű���� ��ġ ����
                //go.GetComponent<RectTransform>().anchoredPosition = new Vector2(UnityEngine.Random.Range(-270.0f, 270.0f), UnityEngine.Random.Range(-140.0f, 140.0f));
                //go.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(-70.0f, 70.0f));
            }

            UIManager.Instance.tmp_userType.text = "유저타입: " + list.userType;
            UIManager.Instance.tmp_userTypeReason.text = "이유: " + list.userTypeReason;

            coverImage.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = $"<{books[0].bookTitle}>\n";
            StartCoroutine(LoadImageFromUrl(books[0].imageUrl, coverImage));

            for (int i = 0; i < books.Length; i++)
            {
                var book = books[i];
                StartCoroutine(LoadImageFromUrl(book.imageUrl, coverImages[i]));
                coverImages[i].transform.GetChild(0).GetComponent<TMP_Text>().text = $"<{book.bookTitle}>\n"; 
            }

            UIManager.Instance.loadingBar.SetActive(false);
            UIManager.Instance.btn_switchPanel.GetComponent<ButtonManager>().isInteractable = true;

        }));
    }

    #endregion

    // 이미지 URL 을 받아와 이미지로 로드
    public IEnumerator LoadImageFromUrl(string url, UnityEngine.UI.Image coverImage)
    {
        // 프로토콜을 https로 강제 변경
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
                Debug.LogError($"이미지 불러오기 실패: {request.error}");
            }
        }
    }

    #region 통신 관련 함수
    // 요청에 따라 응답 타입 다름 -> DoneRequest 대신 Callback(onSuccess) 사용  ��� 
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
                    onSuccess?.Invoke(bookList);
                    break;
                default:
                    Debug.Log("응답 타입 없음");
                    break;
            }
        }
        else
        {
            Debug.LogError("요청 실패: " + request.error);
        }
    }

    #endregion


}
