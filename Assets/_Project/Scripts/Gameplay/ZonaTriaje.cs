using UnityEngine;

public class ZonaTriaje : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Revisamos si el que entró a la zona es el jugador
        if (other.CompareTag("Player"))
        {
            // Extraemos su sistema de agarre
            SistemaAgarre sistema = other.GetComponent<SistemaAgarre>();

            // Si tiene el script y además trae un perrito en las manos...
            if (sistema != null && sistema.TraeAnimal())
            {
                // Entregamos al perro
                sistema.EntregarAnimalTriaje();

                // Dejamos tu función de diálogo pendiente en la consola
                Debug.Log("[PENDIENTE] UI: El perrito ha pasado a la zona de triaje. ¡Buen rescate!");
            }
        }
    }
}