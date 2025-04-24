using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ToggleSetting : MonoBehaviour
{
    public Image panel;
    public Button btn_expand;
    public Image coverImage;

    float width;
    Vector2 originalPos;
    Vector2 newPos = new Vector2(-500, -540);
    Vector2 originalSize;
    Vector2 newSize = new Vector2(1000, 1080);

    bool expanded = false;

    public GameObject[] coverImages;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        width = panel.rectTransform.sizeDelta.x;
        originalPos = panel.rectTransform.anchoredPosition;
        originalSize = panel.rectTransform.sizeDelta;
    }

    // 토글 Expand
    public void Expand()
    {
        if (!expanded)
        {
            //panel.rectTransform.anchoredPosition = newPos;
            //panel.rectTransform.sizeDelta = newSize;
            StartCoroutine(AnimateSize(panel.rectTransform.sizeDelta, newSize, 0.5f));
            btn_expand.GetComponentInChildren<TextMeshProUGUI>().text = "close"; 

            foreach(var ci in coverImages)
            {
                ci.gameObject.SetActive(true);
            }

            expanded = true;
        }
        else
        {
            //panel.rectTransform.anchoredPosition = originalPos;
            //panel.rectTransform.sizeDelta = originalSize;
            StartCoroutine(AnimateSize(panel.rectTransform.sizeDelta, originalSize, 0.5f));
            btn_expand.GetComponentInChildren<TextMeshProUGUI>().text = "expand";

            foreach (var ci in coverImages)
            {
                ci.gameObject.SetActive(false);
            }

            expanded = false;
        }
    }

    // Expand 연출하는 easing function
    IEnumerator AnimateSize(Vector2 from, Vector2 to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float easedT = EaseOutBounce(t);
            panel.rectTransform.sizeDelta = Vector2.Lerp(from, to, easedT);
            elapsed += Time.deltaTime;
            yield return null;
        }

        panel.rectTransform.sizeDelta = to;
    }

    float EaseOutBounce(float x)
    {
        float n1 = 7.5625f;
        float d1 = 2.75f;

        if (x < 1f / d1)
        {
            return n1 * x * x;
        }
        else if (x < 2f / d1)
        {
            x -= 1.5f / d1;
            return n1 * x * x + 0.75f;
        }
        else if (x < 2.5f / d1)
        {
            x -= 2.25f / d1;
            return n1 * x * x + 0.9375f;
        }
        else
        {
            x -= 2.625f / d1;
            return n1 * x * x + 0.984375f;
        }
    }


    // UI에 표지와 책 정보 표시
    public void SetBookUI(int bookIdx, Image coverImage)
    {
        var book = HttpManager.Instance.books[bookIdx];
        coverImage.transform.GetChild(0).GetComponent<TMP_Text>().text = book.bookTitle;
        coverImage.transform.GetChild(1).GetComponentInChildren<TMP_Text>().text = FormatLine("제목", book.bookTitle) + "\n" +
                                                                         FormatLine("키워드", book.bookGenre) + "\n" +
                                                                         FormatLine("내용 요약", book.bookSummary) + "\n" +
                                                                         FormatLine("링크", book.bookUrl);
    }

    // 문자열 간격 조정
    public string FormatLine(string category, string content)
    {
        return $"<{category.PadRight(8)} : {content}>";
    }
}
