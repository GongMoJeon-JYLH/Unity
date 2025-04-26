using Michsky.MUIP;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
using UnityEngine.Networking;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;
using static UnityEngine.Rendering.VolumeComponent;

public class Rec2Setting : MonoBehaviour
{
    public Image panel;
    public GameObject btn_expand;
    public Image coverImage;

    float width;
    Vector2 originalPos;
    Vector2 newPos = new Vector2(0, -1080);
    Vector2 originalSize;
    Vector2 newSize = new Vector2(1100, 750);

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
            btn_expand.GetComponent<ButtonManager>().buttonText = "닫기"; 

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
            btn_expand.GetComponent<ButtonManager>().buttonText = "펼치기";

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


    //// UI에 표지와 책 정보 표시
    public RectTransform content;
    public GameObject rowPrefab;
    int columns = 2;

    public void SetTextBoxes(string[] texts)
    {
        for (int i = 0; i < texts.Length; i = i + 2)
        {
            GameObject row = Instantiate(rowPrefab);
            row.transform.SetParent(content, false);

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
            TextMeshProUGUI txt2 = row.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();

            txt1.text = texts[i];
            txt2.text = texts[i + 1];

            // height 설정
            StartCoroutine(UpdateWidth(txt1));
            StartCoroutine(UpdateHeight(row, txt1, txt2));
        }
    }

    IEnumerator UpdateHeight(GameObject row, TextMeshProUGUI txt1, TextMeshProUGUI txt2)
    {
        yield return null; // 1 프레임 기다리기

        LayoutElement loe = row.GetComponent<LayoutElement>();
        if (loe == null)
        {
            loe = row.gameObject.AddComponent<LayoutElement>();
        }

        loe.preferredHeight = Mathf.Max(txt1.preferredHeight, txt2.preferredHeight);
    }

    IEnumerator UpdateWidth(TextMeshProUGUI txt1)
    {
        yield return null;

        LayoutElement loe1 = txt1.gameObject.GetComponent<LayoutElement>();
        if (loe1 == null)
        {
            loe1 = txt1.gameObject.AddComponent<LayoutElement>();
        }

        //LayoutElement loe2 = txt2.gameObject.GetComponent<LayoutElement>();
        //if (loe1 == null)
        //{
        //    loe2 = txt2.gameObject.AddComponent<LayoutElement>();
        //}

        loe1.preferredWidth = txt1.preferredWidth;
        //loe2.preferredWidth = txt2.preferredWidth;

    }

    public void SetMainDetailText(int idx)
    {
        var book = HttpManager.Instance.books[idx];
        int INDENT = 150;

        string[] texts = new string[]
        {
            $"제목<indent={INDENT}>: </indent>", book.bookTitle, $"키워드<indent={INDENT}>: </indent>", book.bookGenre, $"내용 요약<indent={INDENT}>: </indent>", book.bookSummary, $"링크<indent={INDENT}>: </indent>", book.bookUrl
        };

        SetTextBoxes(texts);

        //coverImage.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = HttpManager.Instance.
    }

    //public void SetBookUI(int bookIdx, Image coverImage)
    //{
    //    var book = HttpManager.Instance.books[bookIdx];
    //    coverImage.transform.GetChild(0).GetComponent<TMP_Text>().text = book.bookTitle;
    //    coverImage.transform.GetChild(1).GetComponentInChildren<TMP_Text>().text = FormatLine("제목", book.bookTitle) + "\n" +
    //                                                                     FormatLine("키워드", book.bookGenre) + "\n" +
    //                                                                     FormatLine("내용 요약", book.bookSummary) + "\n" +
    //                                                                     FormatLine("링크", book.bookUrl);
    //}

    //// 문자열 간격 조정
    //public string FormatLine(string category, string content)
    //{
    //    int mspace = 24;
    //    string monoCat = $"<mspace={mspace}>{category}</mspace>";
    //    return $"{monoCat} : {content}";
    //}
}
