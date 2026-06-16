using UnityEngine;
using System.Collections;
using StarterAssets; // Le decimos a Unity que busque en tu paquete de movimiento

public class BloqueoInicial : MonoBehaviour
{
    [Tooltip("Segundos de espera antes de poder moverse")]
    public float tiempoDeCarga = 4.0f;

    // Variables internas que se llenarán solas
    private ThirdPersonController controladorMovimiento;
    private StarterAssetsInputs lectorDeTeclas;

    private void Start()
    {
        // 1. El script busca automáticamente los componentes en el cuerpo de Remy
        controladorMovimiento = GetComponent<ThirdPersonController>();
        lectorDeTeclas = GetComponent<StarterAssetsInputs>();

        // 2. Iniciamos la cuenta regresiva
        StartCoroutine(RutinaDeDesbloqueo());
    }

    private IEnumerator RutinaDeDesbloqueo()
    {
        // APAGAMOS TODO (Congelamos a Remy)
        if (controladorMovimiento != null) controladorMovimiento.enabled = false;
        if (lectorDeTeclas != null) lectorDeTeclas.enabled = false;

        // Esperamos el tiempo definido para que la IA cargue
        yield return new WaitForSeconds(tiempoDeCarga);

        // ENCENDEMOS TODO (Remy ya puede moverse)
        if (controladorMovimiento != null) controladorMovimiento.enabled = true;
        if (lectorDeTeclas != null) lectorDeTeclas.enabled = true;
    }
}