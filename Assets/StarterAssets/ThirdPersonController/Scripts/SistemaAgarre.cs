using UnityEngine;
using UnityEngine.InputSystem;
using ithappy.Animals_FREE;

public class SistemaAgarre : MonoBehaviour
{
    [Header("Ajustes de Detección")]
    public float radioDeAlcance = 1.5f;
    public LayerMask capasAgarrables;

    [Header("Estética y Transportadora")]
    [Tooltip("El modelo de la caja que pusiste dentro de la columna de Remy")]
    public GameObject cajaTransportadora;
    public Animator animatorRemy;

    private GameObject objetoActual;

    void Start()
    {
        // Nos aseguramos de que la caja empiece oculta por seguridad
        if (cajaTransportadora != null)
        {
            cajaTransportadora.SetActive(false);
        }
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            IntentarInteraccion();
        }
    }

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
        Collider[] objetosCercanos = Physics.OverlapSphere(transform.position, radioDeAlcance, capasAgarrables);

        if (objetosCercanos.Length == 0) return;

        GameObject objetoMasCercano = null;
        float distanciaMinima = Mathf.Infinity;

        foreach (Collider col in objetosCercanos)
        {
            // Filtramos asegurándonos de que sea un animal agarrable
            CreatureGrabbable grabbableEncontrado = col.GetComponent<CreatureGrabbable>();
            if (grabbableEncontrado == null) continue;

            float distancia = Vector3.Distance(transform.position, col.transform.position);

            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                objetoMasCercano = col.gameObject;
            }
        }

        if (objetoMasCercano == null) return;

        // 1. Guardar referencia y avisarle al perro
        objetoActual = objetoMasCercano;
        CreatureGrabbable grabbable = objetoActual.GetComponent<CreatureGrabbable>();
        if (grabbable != null)
        {
            grabbable.estaAgarrada = true;
        }

        // 2. EL CAMBIO VISUAL: Ocultamos al perro de la calle
        objetoActual.SetActive(false);

        // 3. Mostramos la transportadora en los brazos de Remy
        if (cajaTransportadora != null)
        {
            cajaTransportadora.SetActive(true);
        }

        // 4. Activamos la animación de cargar
        if (animatorRemy != null)
        {
            animatorRemy.SetBool("IsCarrying", true);
        }
    }

    void Soltar()
    {
        if (objetoActual == null) return;

        // 1. Ocultamos la transportadora
        if (cajaTransportadora != null)
        {
            cajaTransportadora.SetActive(false);
        }

        // 2. Quitamos la animación de cargar a Remy
        if (animatorRemy != null)
        {
            animatorRemy.SetBool("IsCarrying", false);
        }

        // 3. Reaparecemos al perro en frente del jugador (1.5 metros adelante)
        objetoActual.transform.position = transform.position + (transform.forward * 1.5f);
        objetoActual.SetActive(true);

        // 4. Le avisamos al perro que está libre
        CreatureGrabbable grabbable = objetoActual.GetComponent<CreatureGrabbable>();
        if (grabbable != null)
        {
            grabbable.estaAgarrada = false;
        }

        // Limpiar la referencia
        objetoActual = null;
    }

    // Este método nos dice si Remy trae un perro o tiene las manos vacías
    public bool TraeAnimal()
    {
        return objetoActual != null;
    }

    // Este método se activa cuando llegas a la mesa del hospital
    public void EntregarAnimalTriaje()
    {
        if (objetoActual == null) return;

        // 1. Ocultamos la caja y quitamos la animación
        if (cajaTransportadora != null) cajaTransportadora.SetActive(false);
        if (animatorRemy != null) animatorRemy.SetBool("IsCarrying", false);

        // 2. Desaparecemos al perro permanentemente (ya está a salvo)
        Destroy(objetoActual);
        objetoActual = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, radioDeAlcance);
    }
}