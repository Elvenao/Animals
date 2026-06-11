using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using ithappy.Animals_FREE;
using StarterAssets;

public class SistemaAgarre : MonoBehaviour
{
    [Header("Ajustes de Detección")]
    public float radioDeAlcance = 1.5f;
    public LayerMask capasAgarrables;

    [Header("Estética y Transportadora")]
    public GameObject cajaTransportadora;
    public Animator animatorRemy;
    public GameObject remyMesh;

    [Header("Cinemática de Captura")]
    public Camera camaraPrincipal;
    public Camera camaraCaptura;
    public GameObject lazoInstrumento;
    public float duracionCinematica = 2f;

    [Header("Control de Sigilo Durante Captura")]
    public StarterAssetsInputs starterInputs;
    private bool crouchAntesDeCinematica;

    [Header("Ajuste del Lazo")]
    public Vector3 posicionLocalLazo = new Vector3(0.35f, -0.35f, 0.8f);
    public Vector3 rotacionLocalLazo = new Vector3(0f, 0f, 90f);
    public Vector3 escalaLocalLazo = new Vector3(1.5f, 1.5f, 1.5f);

    private GameObject objetoActual;
    private bool enCinematica = false;
    

    void Start()
    {
        if (camaraPrincipal == null)
            camaraPrincipal = Camera.main;

        if (starterInputs == null)
            starterInputs = GetComponent<StarterAssetsInputs>();

        if (camaraCaptura != null)
            camaraCaptura.gameObject.SetActive(false);

        if (cajaTransportadora != null)
            cajaTransportadora.SetActive(false);

        if (lazoInstrumento != null)
            lazoInstrumento.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame &&
            !enCinematica)
        {
            IntentarInteraccion();
        }
    }

    public void IntentarInteraccion()
    {
        if (objetoActual == null)
            AgarrarElMasCercano();
        else
            Soltar();
    }

    void AgarrarElMasCercano()
    {
        Collider[] objetosCercanos = Physics.OverlapSphere(
            transform.position,
            radioDeAlcance,
            capasAgarrables
        );

        if (objetosCercanos.Length == 0)
            return;

        GameObject objetoMasCercano = null;
        float distanciaMinima = Mathf.Infinity;

        foreach (Collider col in objetosCercanos)
        {
            CreatureGrabbable grabbableEncontrado =
                col.GetComponent<CreatureGrabbable>();

            if (grabbableEncontrado == null)
                continue;

            float distancia = Vector3.Distance(transform.position, col.transform.position);

            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                objetoMasCercano = col.gameObject;
            }
        }

        if (objetoMasCercano == null)
            return;

        objetoActual = objetoMasCercano;
        StartCoroutine(SecuenciaDeCaptura());
    }

    private IEnumerator SecuenciaDeCaptura()
    {
        enCinematica = true;

        if (starterInputs != null)
        {
            crouchAntesDeCinematica = starterInputs.crouch;
            starterInputs.crouch = true;
            starterInputs.sprint = false;
        }

        if (animatorRemy != null)
            animatorRemy.SetBool("Crouch", true);

        Debug.Log("[SafePaws] Iniciando cinemática de captura.");

        if (remyMesh != null)
            remyMesh.SetActive(false);

        // 1. Primero prendemos y aseguramos la cámara de captura
        if (camaraCaptura != null)
        {
            camaraCaptura.gameObject.SetActive(true);
            camaraCaptura.enabled = true;
            camaraCaptura.targetDisplay = 0; // Display 1
            camaraCaptura.depth = 100;

            Debug.Log("[SafePaws] Cámara de captura activada.");
        }
        else
        {
            Debug.LogWarning("[SafePaws] No está asignada camaraCaptura.");
        }

        // 2. Después apagamos la cámara principal
        if (camaraPrincipal != null)
        {
            camaraPrincipal.enabled = false;
            Debug.Log("[SafePaws] Cámara principal desactivada.");
        }
        else
        {
            Debug.LogWarning("[SafePaws] No está asignada camaraPrincipal.");
        }

        // 3. Activamos el lazo
        if (lazoInstrumento != null)
        {
            lazoInstrumento.SetActive(true);
            lazoInstrumento.transform.localPosition = posicionLocalLazo;
            lazoInstrumento.transform.localEulerAngles = rotacionLocalLazo;
            lazoInstrumento.transform.localScale = escalaLocalLazo;

            Debug.Log("[SafePaws] Lazo activado.");
        }
        else
        {
            Debug.LogWarning("[SafePaws] No está asignado lazoInstrumento.");
        }

        // 4. Mantener agachado durante TODA la cinemática
        float tiempo = 0f;

        while (tiempo < duracionCinematica)
        {
            ForzarAgachadoDuranteCaptura();

            tiempo += Time.deltaTime;
            yield return null;
        }

        // 5. Apagamos el lazo
        if (lazoInstrumento != null)
            lazoInstrumento.SetActive(false);

        // 6. Primero regresamos la cámara principal
        if (camaraPrincipal != null)
        {
            camaraPrincipal.enabled = true;
            Debug.Log("[SafePaws] Cámara principal reactivada.");
        }

        // 7. Luego apagamos la cámara de captura
        if (camaraCaptura != null)
        {
            camaraCaptura.enabled = false;
            camaraCaptura.gameObject.SetActive(false);

            Debug.Log("[SafePaws] Cámara de captura apagada.");
        }

        if (remyMesh != null)
            remyMesh.SetActive(true);

        FinalizarCaptura();

        enCinematica = false;

        Debug.Log("[SafePaws] Cinemática terminada.");
    }

    private void ForzarAgachadoDuranteCaptura()
    {
        if (starterInputs != null)
        {
            starterInputs.crouch = true;
            starterInputs.sprint = false;
        }

        if (animatorRemy != null)
        {
            animatorRemy.SetBool("Crouch", true);
        }
    }

    private void FinalizarCaptura()
    {
        if (objetoActual == null)
            return;

        CreatureGrabbable grabbable = objetoActual.GetComponent<CreatureGrabbable>();

        if (grabbable != null)
            grabbable.estaAgarrada = true;

        objetoActual.SetActive(false);

        if (cajaTransportadora != null)
            cajaTransportadora.SetActive(true);

        if (animatorRemy != null)
            animatorRemy.SetBool("IsCarrying", true);
    }

    void Soltar()
    {
        if (objetoActual == null)
            return;

        if (cajaTransportadora != null)
            cajaTransportadora.SetActive(false);

        if (animatorRemy != null)
            animatorRemy.SetBool("IsCarrying", false);

        objetoActual.transform.position = transform.position + (transform.forward * 1.5f);
        objetoActual.SetActive(true);

        CreatureGrabbable grabbable = objetoActual.GetComponent<CreatureGrabbable>();

        if (grabbable != null)
            grabbable.estaAgarrada = false;

        objetoActual = null;
    }

    public bool TraeAnimal()
    {
        return objetoActual != null;
    }

    public void EntregarAnimalTriaje()
    {
        if (objetoActual == null)
            return;

        if (cajaTransportadora != null)
            cajaTransportadora.SetActive(false);

        if (animatorRemy != null)
            animatorRemy.SetBool("IsCarrying", false);

        Destroy(objetoActual);
        objetoActual = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, radioDeAlcance);
    }
}