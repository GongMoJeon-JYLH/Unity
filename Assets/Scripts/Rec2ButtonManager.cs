using UnityEngine;

public class Rec2ButtonManager : MonoBehaviour
{
    public GameObject window_writeBookReport;
    public GameObject window_generateBookReport;

    public void OnClickStartBookReport()
    {
        window_writeBookReport.SetActive(true);
    }

    public void OnClickSubmitBookReport()
    {
        window_generateBookReport.SetActive(true);
        /// 통신 관련 함수
    }
}
