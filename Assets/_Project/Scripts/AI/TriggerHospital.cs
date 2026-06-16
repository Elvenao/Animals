using UnityEngine;
using Convai.Scripts.Runtime.Core;

public class TriggerHospital : MonoBehaviour
{
    [Header("Conexión con Amelia")]
    public ConvaiNPC amelia;

    private bool yaHablo = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !yaHablo)
        {
            yaHablo = true;

            // Instrucción específica para la Zona 1 que definimos en tu documento
            string promptOculto = "Remy acaba de entrar al hospital con un paciente rescatado. Felicítalo brevemente. Dile que vaya primero a Recepción para registrar el caso y que después pase inmediatamente al área de Evaluación/Triaje para determinar la gravedad de las lesiones del paciente.";

            if (amelia != null)
            {
                amelia.SendTextDataAsync(promptOculto);
            }
        }
    }
}