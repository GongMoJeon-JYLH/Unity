using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestTest1 : MonoBehaviour
{
    public TMP_Text textBox;
    
    public struct Book 
    {
        public string bookTitle;
        public string bookGenre;
        public string bookSummary;
        public string bookUrl;
    };


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

        SetBookUI(book);
    }


    public void SetBookUI(Book book)
    {
        textBox.GetComponent<TMP_Text>().text = FormatLine("제목", book.bookTitle) + "\n" +
                                                                         FormatLine("키워드", book.bookGenre) + "\n" +
                                                                         FormatLine("내용 요약", book.bookSummary) + "\n" +
                                                                         FormatLine("링크", book.bookUrl);
    }

    public string FormatLine(string category, string content)
    {
        const int INDENT = 150;
        int boxWidth = (int)textBox.preferredWidth;
        int charactersPerLine = 22; //// 일단은 하드코딩으로.... 나중에 적용할 때 그 텍스트박스 너비에 맞춰서 바꿔야 함

        if (charactersPerLine > content.Length)
        {
            return $"{category}<indent={INDENT}>: </indent>{content}";
        }

        string str = "";


       while (content.Length > 0)
       {
            if(str.Length > 0)
            {
                if(content.Length > charactersPerLine)
                {
                    str = str + "\n" + $"<indent={INDENT}> </indent>{content.Substring(0, charactersPerLine)}";
                }
                else
                {
                    str = str + "\n" + $"<indent={INDENT}> </indent>{content}";
                }
            }
            else
            {
                str = str + $"<indent={INDENT}> </indent>{content.Substring(0, charactersPerLine)}";
            }

            content = content.Substring(Mathf.Min(charactersPerLine, content.Length));
       }
        
        return $"{category}<indent={INDENT}>: </indent> {str}";
    }


    // 텍스트박스를 스폰하는 식으로 바꿀까
    public RectTransform content;
    public GameObject cellPrefab;
    public int columns = 2;

    void SetTextBoxes(string[] texts)
    {
        int total = texts.Length;
        for (int i = 0; i < total; i += columns)
        {
            GameObject row = new GameObject("Row", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            row.transform.SetParent(content, false);

            HorizontalLayoutGroup hlg = row.GetComponent<HorizontalLayoutGroup>();
            hlg.spacing = 10;
            hlg.childForceExpandHeight = false;
            hlg.childForceExpandWidth = false;

            for (int j = 0; j < columns && i + j < total; j++)
            {
                GameObject cell = Instantiate(cellPrefab);
                cell.transform.SetParent(row.transform, false);

                Text txt = cell.GetComponentInChildren<Text>();
                txt.text = texts[i + j];
            }
        }
    }

}
