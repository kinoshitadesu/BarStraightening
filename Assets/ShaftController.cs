using UnityEngine;
using System.Collections.Generic;

public class ShaftController : MonoBehaviour
{
    [System.Serializable]
    public class BonePoint
    {
        public Transform boneTransform; 
        public Quaternion initialRotation;
        
        // ★ YからZに戻しました
        public float plasticBendX = 0f; 
        public float plasticBendZ = 0f; 

        public float elasticBendX = 0f;
        public float elasticBendZ = 0f;
    }

    [Header("ボーンリスト")]
    public List<BonePoint> bonePoints = new List<BonePoint>();

    [Header("物理パラメータ")]
    public float springBackSpeed = 5.0f; 

    [Header("操作設定")]
    public float rotateSpeed = 100f; 
    public float slideSpeed = 2.0f;  
    public float slideLimit = 2.0f; 
    
    public enum SlideAxis { World_X, World_Z }
    public SlideAxis slideDirection = SlideAxis.World_Z; 

    private Vector3 initialPosition; 

    void Start()
    {
        initialPosition = transform.position;
        foreach (var point in bonePoints)
        {
            if (point.boneTransform != null)
                point.initialRotation = point.boneTransform.localRotation;
        }
    }

    void LateUpdate()
    {
        float rotateInput = Input.GetAxis("Horizontal"); 
        transform.Rotate(0, -rotateInput * rotateSpeed * Time.deltaTime, 0);

        float slideInput = Input.GetAxis("Vertical");
        float moveAmount = slideInput * slideSpeed * Time.deltaTime;
        Vector3 currentPos = transform.position;

        if (slideDirection == SlideAxis.World_Z)
        {
            currentPos.z = Mathf.Clamp(currentPos.z + moveAmount, initialPosition.z - slideLimit, initialPosition.z + slideLimit);
        }
        else 
        {
            currentPos.x = Mathf.Clamp(currentPos.x + moveAmount, initialPosition.x - slideLimit, initialPosition.x + slideLimit);
        }
        transform.position = currentPos;

        // ================================================================
        // 2. 物理挙動の更新（Y軸ボーン用）
        // ================================================================
        foreach (var point in bonePoints)
        {
            if (point.boneTransform != null)
            {
                point.elasticBendX = Mathf.Lerp(point.elasticBendX, 0f, springBackSpeed * Time.deltaTime);
                point.elasticBendZ = Mathf.Lerp(point.elasticBendZ, 0f, springBackSpeed * Time.deltaTime);

                float totalX = point.plasticBendX + point.elasticBendX;
                float totalZ = point.plasticBendZ + point.elasticBendZ;

                // ★【修正】Y軸ボーンを曲げるため、X軸とZ軸を合成する
                Vector3 bendVector = new Vector3(totalX, 0, totalZ);
                float bendAngle = bendVector.magnitude; 

                if (bendAngle > 0.001f) 
                {
                    Vector3 bendAxis = bendVector.normalized; 
                    Quaternion bendRot = Quaternion.AngleAxis(bendAngle, bendAxis);
                    point.boneTransform.localRotation = point.initialRotation * bendRot;
                }
                else
                {
                    point.boneTransform.localRotation = point.initialRotation;
                }
            }
        }
    }
    
    public float GetTotalBendX(int index)
    {
        if (index < 0 || index >= bonePoints.Count) return 0;
        return bonePoints[index].plasticBendX + bonePoints[index].elasticBendX;
    }

    [ContextMenu("ボーンを自動取得する")]
    public void AutoFindBones()
    {
        bonePoints.Clear();
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.name.Contains("Bone") || child.name.Contains("bone") || child.name.Contains("def"))
            {
                BonePoint newPoint = new BonePoint();
                newPoint.boneTransform = child;
                bonePoints.Add(newPoint);
            }
        }
    }
}