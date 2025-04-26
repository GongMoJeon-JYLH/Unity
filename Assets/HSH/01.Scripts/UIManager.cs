using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TMPro.Examples;


public class UIManager : MonoBehaviour
{
    private static UIManager instance = null;


    public ScrollRect srollChatWindow;
    public GameObject aiChat;
    public GameObject userChat;

    public TextMeshProUGUI tmp_Chat;
    public TMP_InputField inputChat;
    public GameObject content;
    public GameObject panel_chat;
    public GameObject panel_name;

    public GameObject keywordPanel;
    public GameObject text_keyword;

    public GameObject loadingBar;

    public GameObject btn_clickChat;

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
        tmp_Chat.text = "<color=#00FFFF>추천봇</color>: 혹시 최근에 인상 깊게 읽었던 책이나 관심 있는 주제가 있으신가요?";

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

   
}
