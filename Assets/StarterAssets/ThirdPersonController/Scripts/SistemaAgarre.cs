using UnityEngine;
using UnityEngine.InputSystem;

public class SistemaAgarre : MonoBehaviour
{
    [Header("Ajustes")]
    public Transform puntoDeAgarre; // Donde se pegará el objeto (tu pecho/manos)
    [Tooltip("Distancia a la que puedes agarrar objetos (Radio de la burbuja)")]
    public float radioDeAlcance = 1.5f;
    public LayerMask capasAgarrables;

    private GameObject objetoActual;
    private Rigidbody rbActual;

    void Update()
    {
        // Teclado (Tecla E)
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            IntentarInteraccion();
        }
    }

    // Esta función la llama tu botón en Android y la tecla E
    public void IntentarInteraccion()
    {
        if (objetoActual == null)
        {
            AgarrarElMasCercano();
        }
        else
        {
            Soltar();
        }
    }

    void AgarrarElMasCercano()
    {
        // 1. Crea una burbuja invisible alrededor del jugador y detecta todo lo que esté en la capa "Agarrable"
        Collider[] objetosCercanos = Physics.OverlapSphere(transform.position, radioDeAlcance, capasAgarrables);

        // Si no encontró nada, salimos
        if (objetosCercanos.Length == 0) return;

        // 2. Buscar cuál es el objeto más cercano (por si hay varios)
        GameObject objetoMasCercano = null;
        float distanciaMinima = Mathf.Infinity;

        foreach (Collider col in objetosCercanos)
        {
            float distancia = Vector3.Distance(transform.position, col.transform.position);
            if (distancia < distanciaMinima)
            {
                // Verificamos que tenga Rigidbody para poder agarrarlo
                if (col.GetComponent<Rigidbody>() != null)
                {
                    distanciaMinima = distancia;
                    objetoMasCercano = col.gameObject;
                }
            }
        }

        // 3. Si encontramos un objeto válido, lo agarramos
        if (objetoMasCercano != null)
        {
            objetoActual = objetoMasCercano;
            rbActual = objetoActual.GetComponent<Rigidbody>();

            // Desactivamos físicas
            rbActual.isKinematic = true;
            rbActual.useGravity = false;

            // Opcional: Desactivamos el collider mientras lo cargamos para que no choque con tu cuerpo
            // objetoActual.GetComponent<Collider>().enabled = false;

            // Lo pegamos al punto de agarre
            objetoActual.transform.position = puntoDeAgarre.position;
            objetoActual.transform.rotation = puntoDeAgarre.rotation;
            objetoActual.transform.parent = puntoDeAgarre;
        }
    }

    void Soltar()
    {
        if (objetoActual != null)
        {
            // Reactivamos físicas
            rbActual.isKinematic = false;
            rbActual.useGravity = true;

            // Reactivamos el collider si lo desactivaste antes
            // objetoActual.GetComponent<Collider>().enabled = true;

            // Desvinculamos
            objetoActual.transform.parent = null;

            // Limpieza
            objetoActual = null;
            rbActual = null;
        }
    }

    // Dibujo de la burbuja para que veas el área en el Editor (Pestaña Scene)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f); // Verde transparente
        Gizmos.DrawSphere(transform.position, radioDeAlcance);
    }
}