using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform cameraTarget;
    public float distance = 2f;
    public float heightOffset = 0f;
    public float followSpeed = 3f;

    void LateUpdate()
    {
        if (cameraTarget == null) return;

        // 목표 위치 계산
        Vector3 targetPos = cameraTarget.position + cameraTarget.forward * distance + Vector3.up * heightOffset;

        // 부드럽게 이동 (Lerp)
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);

        // 카메라를 향하도록 회전
        transform.rotation = Quaternion.LookRotation(transform.position - cameraTarget.position);
    }
}