using UnityEngine;

public class RodDeformationManager : MonoBehaviour
{
    [Header("UI連携")]
    // 先ほど作成したUI更新用スクリプトを紐づけます
    [SerializeField] private ToleranceDisplay toleranceDisplay;

    [Header("丸棒のパラメータ")]
    [Tooltip("丸棒の全長（メートル）")]
    [SerializeField] private float rodLengthMeters = 2.0f; 

    void Update()
    {
        // 1. 材料力学の計算モデルから、現在の「最大たわみ量(mm)」を取得する
        float currentMaxDeflectionMm = CalculateMaxDeflection();

        // 2. 1mあたりの公差（mm/m）を計算する
        // 例: 全長2mで最大たわみが3mmなら、1mあたりは1.5mmとなる
        float tolerancePerMeter = currentMaxDeflectionMm / rodLengthMeters;

        // 3. UIを更新する
        if (toleranceDisplay != null)
        {
            toleranceDisplay.UpdateToleranceText(tolerancePerMeter);
        }
    }

    /// <summary>
    /// 現在の丸棒の最大たわみ量を計算・取得するメソッド
    /// </summary>
    /// <returns>最大たわみ量 (mm)</returns>
    private float CalculateMaxDeflection()
    {
        // ※ ここにオイラー・ベルヌーイの梁の理論や、塑性変形を考慮した
        // 計算ロジック、あるいはUnityのMeshの頂点座標から曲がりを
        // 算出する処理が入ります。
        
        // 今回は連携のテスト用に、時間経過で変化する仮の数値を返します
        // （実際には曲げモーメントから算出した y(x) の最大絶対値などを返します）
        float mockDeflection = Mathf.PingPong(Time.time, 5.0f); 
        return mockDeflection;
    }
}