using UnityEngine;

public class PressController : MonoBehaviour
{
    [Header("参照")]
    public Transform ramObj; 
    public ShaftController shaftController; 

    [Header("プレスの動き")]
    public float moveSpeed = 1.0f;     
    public float topY = 5.0f;          
    public float bottomY = 1.0f;       

    [Header("物理パラメータ")]
    public float pressForce = 150.0f;   
    public float yieldPoint = 15.0f; 
    [Range(0.1f, 1.0f)] public float plasticityRate = 0.5f;

    [Header("判定設定")]
    public float hitHeight = 1.2f;     
    
    [Tooltip("プレス機のヘッドの横幅")]
    public float pressWidth = 0.3f; 

    void Update()
    {
        float targetY = Input.GetKey(KeyCode.Space) ? bottomY : topY;
        
        Vector3 newPos = ramObj.position;
        newPos.y = Mathf.MoveTowards(newPos.y, targetY, moveSpeed * Time.deltaTime);
        ramObj.position = newPos;

        if (ramObj.position.y <= hitHeight)
        {
            ApplyPhysicsCorrection();
        }
    }

    void ApplyPhysicsCorrection()
    {
        if (shaftController == null) return;

        Vector3 worldPressForce = Vector3.down * pressForce * Time.deltaTime;

        foreach (var point in shaftController.bonePoints)
        {
            if (point.boneTransform == null) continue;

            Vector3 ramPosFlat = ramObj.position; ramPosFlat.y = 0; 
            Vector3 bonePosFlat = point.boneTransform.position; bonePosFlat.y = 0;
            float dist = Vector3.Distance(ramPosFlat, bonePosFlat);

            if (dist <= pressWidth)
            {
                float weight = 1.0f - (dist / pressWidth);

                // 親の回転とボーンの初期回転を加味して、正確な力を計算
                Transform parentTransform = point.boneTransform.parent;
                Quaternion parentRot = parentTransform != null ? parentTransform.rotation : shaftController.transform.rotation;
                Quaternion restRotationWorld = parentRot * point.initialRotation;
                Vector3 localForce = Quaternion.Inverse(restRotationWorld) * worldPressForce;

                // ★【修正】Y軸ボーンへの力の適用
                point.elasticBendX -= localForce.z * weight; 
                point.elasticBendZ -= localForce.x * weight;

                // 降伏判定（X軸）
                if (Mathf.Abs(point.elasticBendX) > yieldPoint)
                {
                    float overflow = (point.elasticBendX > 0) ? 
                        point.elasticBendX - yieldPoint : 
                        point.elasticBendX + yieldPoint;

                    point.plasticBendX += overflow * plasticityRate;
                    point.elasticBendX = Mathf.Clamp(point.elasticBendX, -yieldPoint, yieldPoint);
                }

                // 降伏判定（Z軸）★YからZに戻しました
                if (Mathf.Abs(point.elasticBendZ) > yieldPoint)
                {
                    float overflow = (point.elasticBendZ > 0) ? 
                        point.elasticBendZ - yieldPoint : 
                        point.elasticBendZ + yieldPoint;

                    point.plasticBendZ += overflow * plasticityRate;
                    point.elasticBendZ = Mathf.Clamp(point.elasticBendZ, -yieldPoint, yieldPoint);
                }
            }
        }
    }
}