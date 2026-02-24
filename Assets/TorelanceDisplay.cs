using UnityEngine;
using TMPro; // TextMeshProを操作するために必須

public class ToleranceDisplay : MonoBehaviour
{
    // InspectorからText (TMP)を紐づけるための変数
    [SerializeField] private TextMeshProUGUI toleranceText;

    /// <summary>
    /// 画面の公差表示を更新するメソッド
    /// </summary>
    /// <param name="tolerancePerMeter">1mあたりの公差（mm）</param>
    public void UpdateToleranceText(float tolerancePerMeter)
    {
        if (toleranceText != null)
        {
            // 小数点以下2桁（F2）まで表示する設定です
            toleranceText.text = $"1mあたりの公差: {tolerancePerMeter:F2} mm";
        }
        else
        {
            Debug.LogWarning("ToleranceTextがアタッチされていません！");
        }
    }
}