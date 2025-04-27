using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TMPro.Examples;
using Michsky.MUIP;


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

    // 이름 패널, 채팅 패널
    public GameObject panel_chat;
    public GameObject panel_name;


    // 키워드 및 유저타입 
    public GameObject keywordPanel;
    public GameObject text_keyword;

    public GameObject userTypePanel;
    public GameObject btn_switchPanel;
    bool isPanelActive = true;


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
    }

    void Update()
    {
        
    }

    public void PanelChange(bool isOn)
    {
        panel_name.SetActive(isOn);
        panel_chat.SetActive(!isOn);
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
        print(isPanelActive); 

    }

}
