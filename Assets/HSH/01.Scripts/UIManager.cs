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
        
    }

    void Update()
    {
        
    }
}
