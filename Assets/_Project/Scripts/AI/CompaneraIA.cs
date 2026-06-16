using UnityEngine;
using UnityEngine.AI;

public class CompaneraIA : MonoBehaviour
{
    [Header("Configuración de Seguimiento")]
    [Tooltip("Arrastra aquí el PlayerArmature de Remy")]
    public Transform objetivo;

    [Header("Teletransporte")]
    [Tooltip("Si Remy se aleja de golpe más de esta distancia, Amelia se teletransportará con él.")]
    public float distanciaMaxima = 15f;

    private NavMeshAgent agente;
    private Animator animator;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (objetivo != null && agente != null)
        {
            // 1. Calculamos la distancia exacta entre Amelia y Remy
            float distancia = Vector3.Distance(transform.position, objetivo.position);

            // 2. ¿Remy se teletransportó muy lejos?
            if (distancia > distanciaMaxima)
            {
                // Usamos Warp para teletransportarla de forma segura en el NavMesh.
                // Le restamos un poco a la posición para que aparezca un pasito atrás de ti y no adentro de ti.
                Vector3 posicionAparicion = objetivo.position - (objetivo.forward * 1.5f);
                agente.Warp(posicionAparicion);

                Debug.Log("[SafePaws] Amelia se ha teletransportado junto a Remy.");
            }
            else
            {
                // 3. Comportamiento normal: calcular la ruta y caminar hacia él
                agente.SetDestination(objetivo.position);
            }

            // 4. Actualizamos la animación de caminar
            if (animator != null)
            {
                // 1. Pasamos la velocidad real del NavMesh al parámetro Speed
                float velocidadActual = agente.velocity.magnitude;
                animator.SetFloat("Speed", velocidadActual);

                // 2. --- CONTROL DE PRIORIDAD (Caminar vs Hablar) ---
                // Obtenemos la referencia al script de Convai para consultar su estado interno
                Convai.Scripts.Runtime.Core.ConvaiNPC scriptConvai = GetComponent<Convai.Scripts.Runtime.Core.ConvaiNPC>();

                if (velocidadActual > 0.1f)
                {
                    // Si Amelia está caminando, forzamos el parámetro "Talk" a false.
                    // Esto cancela los gestos exagerados de los brazos para que pueda avanzar bien.
                    animator.SetBool("Talk", false);
                }
                else if (scriptConvai != null && scriptConvai.IsCharacterTalking)
                {
                    // Si se detiene por completo, pero la propiedad interna de Convai dice 
                    // que todavía no termina de hablar, reactivamos la animación en el sitio.
                    animator.SetBool("Talk", true);
                }
            }
        }
    }
}