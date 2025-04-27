using UnityEngine;
using UnityEngine.UI;

public class SizeFit : MonoBehaviour
{

    private RectTransform targetRect;
    private GameObject child;
    private ContentSizeFitter contentFitter;
    void Start()
    {
        child = transform.GetChild(0).gameObject;
        targetRect = transform.GetComponent<RectTransform>();
        contentFitter = targetRect.GetComponent<ContentSizeFitter>();
        SetHeight();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHeight()
    {
        // ContentSizeFitter 비활성화
        if (contentFitter != null)
            contentFitter.enabled = false;
        Vector2 sizeChild = child.GetComponent<RectTransform>().sizeDelta;
        // height 수정
        Vector2 size = targetRect.sizeDelta;
        size.y = sizeChild.y;
        targetRect.sizeDelta = size;
        print(sizeChild.y);
        print(size.y);

        // 필요하면 다시 ContentSizeFitter 활성화
        if (contentFitter != null)
            contentFitter.enabled = true;

    }
}
