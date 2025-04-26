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
            bookTitle = "�� :�Ѱ� �Ҽ� ",
            bookUrl = "https://data4library.kr/bookV?seq=3746119",
            bookSummary = "2018�� ��, �Ѱ� �۰��� �Ҽ� &lt;��&gt;�� ���Ӱ� �����δ�. �� �� �� ������ ���� ���� ���� ��� ������ �ѷ����� �� �ִ� �� &lt;��&gt;�� �� ���� ������ �� �� �Ҽ� �߰��� ������ ���ߴ� �۰��� �����ս��� �۰� �Բ� ������� �ϴ� �ٶ���������.",
            bookGenre = "���� > �ѱ����� > �Ҽ�"
        };

        SetBookUI(book);
    }


    public void SetBookUI(Book book)
    {
        textBox.GetComponent<TMP_Text>().text = FormatLine("����", book.bookTitle) + "\n" +
                                                                         FormatLine("Ű����", book.bookGenre) + "\n" +
                                                                         FormatLine("���� ���", book.bookSummary) + "\n" +
                                                                         FormatLine("��ũ", book.bookUrl);
    }

    public string FormatLine(string category, string content)
    {
        const int INDENT = 150;
        int boxWidth = (int)textBox.preferredWidth;
        int charactersPerLine = 22; //// �ϴ��� �ϵ��ڵ�����.... ���߿� ������ �� �� �ؽ�Ʈ�ڽ� �ʺ� ���缭 �ٲ�� ��

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


    // �ؽ�Ʈ�ڽ��� �����ϴ� ������ �ٲܱ�
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
