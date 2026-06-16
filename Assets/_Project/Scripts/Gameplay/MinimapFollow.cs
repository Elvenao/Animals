using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public Transform playerTarget;   // Arrastra a Remy aquí (para la posición)
    public Transform cameraTarget;   // Arrastra tu Main Camera aquí (para la rotación)
    public float height = 50f;

    void LateUpdate()
    {
        if (playerTarget == null || cameraTarget == null) return;

        // 1. Mover el minimapa a la posición del jugador
        Vector3 newPosition = playerTarget.position;
        newPosition.y = height;
        transform.position = newPosition;

        // 2. Rotar el minimapa basándose en la rotación de la cámara (Y-axis)
        transform.rotation = Quaternion.Euler(90f, cameraTarget.eulerAngles.y, 0f);
    }
}