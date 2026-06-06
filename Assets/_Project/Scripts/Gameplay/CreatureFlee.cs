using UnityEngine;
using ithappy.Animals_FREE;

public class CreatureFlee : MonoBehaviour
{
    [Header("Configuración de Sigilo y Fuga")]
    public Transform player;

    [Tooltip("Distancia a la que el animal entra en pánico si vas RÁPIDO")]
    public float distanciaAsustoMax = 12f;

    [Tooltip("Distancia a la que el animal entra en pánico si vas LENTO (en sigilo)")]
    public float distanciaAsustoMin = 4f;

    [Tooltip("Metros extra antes del pánico donde el animal solo CAMINARÁ alejándose")]
    public float zonaDeAdvertencia = 6f;

    public float limiteVelocidadSigilo = 3f;

    private CreatureMover mover;
    private CreatureGrabbable grabbable;
    private Vector3 ultimaPosicionJugador;

    private void Awake()
    {
        mover = GetComponent<CreatureMover>();
        grabbable = GetComponent<CreatureGrabbable>();
    }

    private void Start()
    {
        if (player != null)
        {
            ultimaPosicionJugador = player.position;
        }
    }

    private void Update()
    {
        // 1. Si ya fue recolectado, desactiva todo movimiento
        if (grabbable != null && grabbable.estaAgarrada)
        {
            mover.SetInput(Vector2.zero, transform.position + transform.forward, false, false);
            return;
        }

        // 2. Calcular velocidad real del jugador
        float velocidadJugador = Vector3.Distance(player.position, ultimaPosicionJugador) / Time.deltaTime;
        ultimaPosicionJugador = player.position;

        // 3. Definir los radios de los estados dinámicamente
        float radioPanico = (velocidadJugador > limiteVelocidadSigilo) ? distanciaAsustoMax : distanciaAsustoMin;
        float radioIncomodidad = radioPanico + zonaDeAdvertencia;

        float distanciaAlJugador = Vector3.Distance(transform.position, player.position);

        // 4. Lógica de los 3 Estados (Idle, Caminar, Correr)
        if (distanciaAlJugador < radioIncomodidad)
        {
            Vector3 direccionHuida = (transform.position - player.position).normalized;
            Vector3 destino = transform.position + direccionHuida;

            // Vector2(0, 1) simula empujar el joystick hacia adelante
            Vector2 ejeMovimiento = new Vector2(0f, 1f);

            if (distanciaAlJugador < radioPanico)
            {
                // ESTADO 3: Muy cerca o mucho ruido -> CORRER (run = true)
                mover.SetInput(ejeMovimiento, destino, true, false);
            }
            else
            {
                // ESTADO 2: Cerca pero sin peligro inminente -> CAMINAR (run = false)
                mover.SetInput(ejeMovimiento, destino, false, false);
            }
        }
        else
        {
            // ESTADO 1: Fuera del radar -> IDLE (Sin movimiento)
            mover.SetInput(Vector2.zero, transform.position + transform.forward, false, false);
        }
    }
}