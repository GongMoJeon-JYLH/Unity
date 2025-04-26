using System.Collections;
using System.Xml;
using TMPro;
using Unity.Android.Gradle.Manifest;
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
            bookTitle = "흰 :한강 소설 ",
            bookUrl = "https://data4library.kr/bookV?seq=3746119",
            bookSummary = "2018년 봄, 한강 작가의 소설 &lt;흰&gt;을 새롭게 선보인다. 이 년 전 오월에 세상에 나와 빛의 겹겹 오라기로 둘러싸인 적 있던 그 &lt;흰&gt;에 새 옷을 입히게 된 건 소설 발간에 즈음해 행했던 작가의 퍼포먼스가 글과 함께 배었으면 하는 바람에서였다.",
            bookGenre = "문학 > 한국문학 > 소설"
        };

        string[] texts = new string[]
        {
            $"제목<indent={INDENT}>: </indent>", book.bookTitle, $"키워드<indent={INDENT}>: </indent>", book.bookGenre, $"내용 요약<indent={INDENT}>: </indent>", book.bookSummary, $"링크<indent={INDENT}>: </indent>", book.bookUrl
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
            
            // Row의 RectTransform 설정
            RectTransform rowRect = row.GetComponent<RectTransform>();
            rowRect.anchorMin = new Vector2(0f, 1f); // 상단 정렬
            rowRect.anchorMax = new Vector2(1f, 1f);  // 상단 정렬
            rowRect.pivot = new Vector2(0f, 1f);    // 기준점 좌측 상단

            // Content의 VerticalLayoutGroup 설정
            VerticalLayoutGroup vlg = content.GetComponent<VerticalLayoutGroup>();
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = true;

            TextMeshProUGUI txt1 = row.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI txt2 = row.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
                
            txt1.text = texts[i];
            txt2.text = texts[i + 1];

            // height 설정
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

}
