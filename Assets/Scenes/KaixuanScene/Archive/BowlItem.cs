using UnityEngine;

public class BowlItem : MonoBehaviour
{
    [Header("拖入你的托盘物体")]
    public Transform trayTransform;

    [Header("调整碗在托盘上的相对位置")]
    public Vector3 localOffset = new Vector3(0f, 0.15f, 0.1f); // 默认微调高度和前倾

    private void LateUpdate()
    {
        if (trayTransform == null) return;

        // 强行将碗的位置锁定在托盘局部坐标系下，彻底无视物理和动画干扰
        transform.position = trayTransform.TransformPoint(localOffset);
        transform.rotation = trayTransform.rotation;
    }
}