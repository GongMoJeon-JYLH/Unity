using System.Collections;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.VolumeComponent;

public class ScrollViewTest : MonoBehaviour
{

    public struct Book
    {
        public string bookTitle;
        public string bookGenre;
        public string bookSummary;
        public string bookUrl;
    };

    const int INDENT = 150;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Book book = new Book
        {
            bookTitle = "�� :�Ѱ� �Ҽ� ",
            bookUrl = "https://data4library.kr/bookV?seq=3746119",
            bookSummary = "2018�� ��, �Ѱ� �۰��� �Ҽ� &lt;��&gt;�� ���Ӱ� �����δ�. �� �� �� ������ ���� ���� ���� ��� ������ �ѷ����� �� �ִ� �� &lt;��&gt;�� �� ���� ������ �� �� �Ҽ� �߰��� ������ ���ߴ� �۰��� �����ս��� �۰� �Բ� ������� �ϴ� �ٶ���������.",
            bookGenre = "���� > �ѱ����� > �Ҽ�"
        };

        string[] texts = new string[]
        {
            //$"���� : ", book.bookTitle, $"Ű���� : ", book.bookGenre, $"���� ��� : ", book.bookSummary, $"��ũ :", book.bookUrl
            $"����<indent={INDENT}>: </indent>", book.bookTitle, $"Ű����<indent={INDENT}>: </indent>", book.bookGenre, $"���� ���<indent={INDENT}>: </indent>", book.bookSummary, $"��ũ<indent={INDENT}>: </indent>", book.bookUrl
        };

        SetTextBoxes(texts);
    }

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
            rowRect.offsetMin = new Vector2(0f, rowRect.offsetMin.y); // ���� offset 0
            rowRect.offsetMax = new Vector2(0f, rowRect.offsetMax.y); // ������ offset 0

            // Content�� VerticalLayoutGroup ����
            VerticalLayoutGroup vlg = content.GetComponent<VerticalLayoutGroup>();
            //vlg.childControlWidth = true;
            //vlg.childControlHeight = true;
            //vlg.childForceExpandWidth = true;
            //vlg.childForceExpandHeight = true;

            TextMeshProUGUI txt1 = row.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI txt2 = row.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
                
            txt1.text = texts[i];
            txt2.text = texts[i + 1];

            // height ����
            StartCoroutine(UpdateWidth(txt1));
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

}
