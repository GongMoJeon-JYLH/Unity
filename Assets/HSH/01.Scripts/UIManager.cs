using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TMPro.Examples;
using Michsky.MUIP;
using System.Collections;


public class UIManager : MonoBehaviour
{
    private static UIManager instance = null;

    //public TextMeshProUGUI tmp_Chat;

    // 채팅 을 위한 변수
    public ScrollRect srollChatWindow;
    public GameObject aiChat;
    public GameObject userChat;
    public TMP_InputField inputChat;
    public GameObject content;
    // 채팅 로딩
    public GameObject loadingBar;
    // 채팅 입력
    public GameObject btn_clickChat;
    // 채팅 아이콘
    public Sprite userIcon;
    public Sprite aIcon;
    // 말풍선
    public Sprite userBalloon;
    public Sprite aiBalloon;

    // 이름 패널, 채팅 패널
    public GameObject panel_chat;
    public GameObject panel_name;


    // 키워드 및 유저타입 
    public GameObject keywordPanel;
    public GameObject text_keyword;

    public GameObject userTypePanel;
    public GameObject btn_switchPanel;

    public TextMeshProUGUI tmp_userType;
    public TextMeshProUGUI tmp_userTypeReason;

    private bool isPanelActive = true;

    private string aiTalk;
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
    public static UIManager Instance
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

    void Start()
    {

        loadingBar.SetActive(false);
        //tmp_Chat.text = "<color=#00FFFF>추천봇</color>: 혹시 최근에 인상 깊게 읽었던 책이나 관심 있는 주제가 있으신가요?";
        isPanelActive = keywordPanel.activeSelf;
        PanelChange(true);
        //btn_sendChat.onClick.AddListener(HttpManager.Instance.OnClickSendChat);
        //btn_Recommand.onClick.AddListener(HttpManager.Instance.OnClickGetBookRecommendation);
        btn_switchPanel.GetComponent<ButtonManager>().isInteractable = false;

        aiTalk = ": 혹시 최근에 인상 깊게 읽었던 책이나 관심 있는 주제가 있으신가요?";
    }

    void Update()
    {
        
    }

    public void PanelChange(bool isOn)
    {
        panel_name.SetActive(isOn);
        panel_chat.SetActive(!isOn);

        SetHeight(aiTalk, aIcon, aiBalloon);
    }

    public void OnClickSwitchPanel()
    {
        isPanelActive = !isPanelActive;
        keywordPanel.SetActive(isPanelActive);
        userTypePanel.SetActive(!isPanelActive);

        if(isPanelActive)
        {
            btn_switchPanel.GetComponent<ButtonManager>().buttonText = "키워드!";
        }
        else
        {
            btn_switchPanel.GetComponent<ButtonManager>().buttonText = "유저타입!";
        }
        print(btn_switchPanel.GetComponent<ButtonManager>().buttonText); 

    }

    public void SetHeight(string text, Sprite sprite, Sprite balloon)
    {
        GameObject row = Instantiate(aiChat);
        row.transform.SetParent(content.transform, false);

        row.GetComponent<Image>().sprite = balloon;

        Image image = row.transform.GetChild(1).GetComponent<Image>();
        image.sprite = sprite;
        // Row의 RectTransform 설정
        RectTransform rowRect = row.GetComponent<RectTransform>();
        rowRect.anchorMin = new Vector2(0f, 1f); // 상단 정렬
        rowRect.anchorMax = new Vector2(1f, 1f);  // 상단 정렬
        rowRect.pivot = new Vector2(0f, 1f);    // 기준점 좌측 상단
        rowRect.offsetMin = new Vector2(0f, rowRect.offsetMin.y); // 왼쪽 offset 0
        rowRect.offsetMax = new Vector2(0f, rowRect.offsetMax.y); // 오른쪽 offset 0

        // Content의 VerticalLayoutGroup 설정
        VerticalLayoutGroup vlg = content.GetComponent<VerticalLayoutGroup>();

        TextMeshProUGUI txt1 = row.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();

        txt1.text = text;

        StartCoroutine(UpdateHeight(row, txt1));

    }
    IEnumerator UpdateHeight(GameObject row, TextMeshProUGUI txt1)
    {
        yield return new WaitForEndOfFrame(); // 1 프레임 기다리기

        LayoutElement loe = row.GetComponent<LayoutElement>();
        if (loe == null)
        {
            loe = row.gameObject.AddComponent<LayoutElement>();
        }

        loe.preferredHeight = txt1.preferredHeight;
    }

}
