using UnityEngine;
using System.Collections; // Necesario para las Corrutinas
using Convai.Scripts.Runtime.Core;

public class TriggerBienvenida : MonoBehaviour
{
    [Header("Conexión con Amelia")]
    [Tooltip("Arrastra a tu NPC de Convai aquí")]
    public ConvaiNPC amelia;

    private bool yaHablo = false;

    private void OnTriggerEnter(Collider other)
    {
        // Revisamos si el que pisó la zona es el jugador y si no ha hablado
        if (other.CompareTag("Player") && !yaHablo)
        {
            yaHablo = true; // Bloqueamos para que no se repita

            // Iniciamos la cuenta regresiva antes de mandar el mensaje
            StartCoroutine(MandarMensajeConRetraso());
        }
    }

    private IEnumerator MandarMensajeConRetraso()
    {
        // Magia: Le damos 1.5 segundos a Convai para estabilizar su red
        yield return new WaitForSeconds(1.5f);

        string promptOculto = "Remy acaba de llegar a la ciudad. Dale la bienvenida oficial a la misión de SafePaws. Sé breve y profesional. Recuérdale que debe usar su minimapa para localizar los puntos rojos, acercarse a ellos agachado con la tecla C, y usar la tecla E con su bastón de contención para asegurar al paciente.";

        if (amelia != null)
        {
            // Ahora sí, enviamos la orden de forma segura
            amelia.SendTextDataAsync(promptOculto);
        }
    }
}