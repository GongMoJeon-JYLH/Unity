using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIManager : MonoBehaviour
{
    private static UIManager instance = null;


    public ScrollRect srollChatWindow;
    public GameObject aiChat;
    public GameObject userChat;
    public TMP_InputField inputChat;
    public GameObject content;

    public GameObject keywordPanel;
    public GameObject text_keyword;

    public Button btn_sendChat;
    public Button btn_Recommand;


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
        //btn_sendChat.onClick.AddListener(HttpManager.Instance.OnClickSendChat);
        //btn_Recommand.onClick.AddListener(HttpManager.Instance.OnClickGetBookRecommendation);
    }

    void Update()
    {
        
    }
}
