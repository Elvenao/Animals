using UnityEngine;
using Unity.Cinemachine;

public class ZoomCamera : MonoBehaviour
{
    public CinemachineCamera virtualCamera;
    public float normalFOV = 40f;
    public float zoomFOV = 15f;
    public float zoomSpeed = 10f;

    private float targetFOV;

    void Start()
    {
        targetFOV = normalFOV;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Z))
            targetFOV = zoomFOV;
        else
            targetFOV = normalFOV;

        virtualCamera.Lens.FieldOfView = Mathf.Lerp(
            virtualCamera.Lens.FieldOfView,
            targetFOV,
            Time.deltaTime * zoomSpeed
        );
    }
}