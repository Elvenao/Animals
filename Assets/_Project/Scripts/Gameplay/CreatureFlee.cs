using UnityEngine;
using ithappy.Animals_FREE;
using StarterAssets;

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

    [Header("Configuración de Confianza")]
    [Tooltip("Radio de acercamiento cuando estás AGACHADO")]
    public float radioConfianza = 8f;

    [Tooltip("A qué distancia se detendrá para no empujarte")]
    public float distanciaParada = 1.5f;

    public float limiteVelocidadSigilo = 3f;

    private CreatureMover mover;
    private CreatureGrabbable grabbable;
    private StarterAssetsInputs playerInputs;
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
            playerInputs = player.GetComponent<StarterAssetsInputs>();
        }
    }

    private void Update()
    {
        // 1. FRENO DE EMERGENCIA (Solución a los errores rojos en consola)
        if (grabbable != null && grabbable.estaAgarrada)
        {
            if (mover.enabled)
            {
                // Forzamos al motor a detenerse y luego lo apagamos por completo
                mover.SetInput(Vector2.zero, transform.position + transform.forward, false, false);
                mover.enabled = false;
            }
            return;
        }

        // Si el motor fue apagado, no calculamos nada más
        if (!mover.enabled) return;

        float distanciaAlJugador = Vector3.Distance(transform.position, player.position);

        // 2. LÓGICA DE CONFIANZA (Mientras mantienes presionada la tecla)
        if (playerInputs != null && playerInputs.crouch && distanciaAlJugador < radioConfianza)
        {
            if (distanciaAlJugador > distanciaParada)
            {
                Vector3 direccionAcercamiento = (player.position - transform.position).normalized;
                Vector3 destino = transform.position + direccionAcercamiento;
                mover.SetInput(new Vector2(0f, 1f), destino, false, false);
            }
            else
            {
                mover.SetInput(Vector2.zero, transform.position + transform.forward, false, false);
            }

            // CRUCIAL: Actualizamos la posición aquí para evitar picos de velocidad al soltar la tecla
            ultimaPosicionJugador = player.position;
            return; // Salimos del Update
        }

        // 3. LÓGICA DE MIEDO (Se ejecuta en cuanto sueltas la tecla)
        float velocidadJugador = Vector3.Distance(player.position, ultimaPosicionJugador) / Time.deltaTime;
        ultimaPosicionJugador = player.position;

        float radioPanico = (velocidadJugador > limiteVelocidadSigilo) ? distanciaAsustoMax : distanciaAsustoMin;
        float radioIncomodidad = radioPanico + zonaDeAdvertencia;

        if (distanciaAlJugador < radioIncomodidad)
        {
            Vector3 direccionHuida = (transform.position - player.position).normalized;
            Vector3 destino = transform.position + direccionHuida;
            Vector2 ejeMovimiento = new Vector2(0f, 1f);

            if (distanciaAlJugador < radioPanico)
            {
                mover.SetInput(ejeMovimiento, destino, true, false); // Corre despavorido
            }
            else
            {
                mover.SetInput(ejeMovimiento, destino, false, false); // Camina para alejarse
            }
        }
        else
        {
            mover.SetInput(Vector2.zero, transform.position + transform.forward, false, false); // Se queda quieto
        }
    }
}