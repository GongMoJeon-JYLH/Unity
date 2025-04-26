using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Rec2Setting : MonoBehaviour
{
    public Image panel;
    public Button btn_expand;
    public Image coverImage;
    public TMP_Text textBox;

    float width;
    float textBoxWidth;
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

        textBoxWidth = textBox.rectTransform.sizeDelta.x;
    }

    // ��� Expand
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

    // Expand �����ϴ� easing function
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


    //// UI�� ǥ���� å ���� ǥ��
    public RectTransform content;
    public GameObject rowPrefab;
    int columns = 2;

    void SetTextBoxes(string[] texts)
    {
        for (int i = 0; i < texts.Length; i = i + 2)
        {
            GameObject row = Instantiate(rowPrefab);
            row.transform.SetParent(content, false);

            // Row�� RectTransform ����
            RectTransform rowRect = row.GetComponent<RectTransform>();
            rowRect.anchorMin = new Vector2(0f, 1f); // ��� ����
            rowRect.anchorMax = new Vector2(1f, 1f);  // ��� ����
            rowRect.pivot = new Vector2(0f, 1f);    // ������ ���� ���

            // Content�� VerticalLayoutGroup ����
            VerticalLayoutGroup vlg = content.GetComponent<VerticalLayoutGroup>();
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = true;

            TextMeshProUGUI txt1 = row.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI txt2 = row.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();

            txt1.text = texts[i];
            txt2.text = texts[i + 1];

            // height ����
            StartCoroutine(UpdateHeight(row, txt1, txt2));
        }
    }

    IEnumerator UpdateHeight(GameObject row, TextMeshProUGUI txt1, TextMeshProUGUI txt2)
    {
        yield return null; // 1 ������ ��ٸ���

        LayoutElement loe = row.GetComponent<LayoutElement>();
        if (loe == null)
        {
            loe = row.gameObject.AddComponent<LayoutElement>();
        }

        loe.preferredHeight = Mathf.Max(txt1.preferredHeight, txt2.preferredHeight);
    }



    public void SetBookUI(int bookIdx, Image coverImage)
    {
        var book = HttpManager.Instance.books[bookIdx];
        coverImage.transform.GetChild(0).GetComponent<TMP_Text>().text = book.bookTitle;
        coverImage.transform.GetChild(1).GetComponentInChildren<TMP_Text>().text = FormatLine("����", book.bookTitle) + "\n" +
                                                                         FormatLine("Ű����", book.bookGenre) + "\n" +
                                                                         FormatLine("���� ���", book.bookSummary) + "\n" +
                                                                         FormatLine("��ũ", book.bookUrl);
    }

    // ���ڿ� ���� ����
    public string FormatLine(string category, string content)
    {
        int mspace = 24;
        string monoCat = $"<mspace={mspace}>{category}</mspace>";
        return $"{monoCat} : {content}";
    }
}
