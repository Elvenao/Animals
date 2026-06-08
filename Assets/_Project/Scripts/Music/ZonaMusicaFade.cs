using UnityEngine;
using System.Collections;

public class ZonaMusicaFade : MonoBehaviour
{
    [Header("Configuración de Audio")]
    public AudioSource audioSource;
    public float volumenMaximo = 1f;
    public float tiempoFade = 2.0f;

    private Coroutine rutinaActual;
    private SphereCollider miCollider;

    private void Start()
    {
        miCollider = GetComponent<SphereCollider>();

        if (audioSource != null)
        {
            audioSource.volume = 0f; 
        }

        Collider[] objetosAdentro = Physics.OverlapSphere(transform.position, miCollider.radius);
        foreach (Collider obj in objetosAdentro)
        {
            if (obj.CompareTag("Player"))
            {
                Debug.Log($"[Audio] El jugador ya estaba en {gameObject.name}. Iniciando Fade In.");
                if (rutinaActual != null) StopCoroutine(rutinaActual);
                
                // Reiniciamos a 0 también aquí por seguridad
                if (audioSource != null) audioSource.volume = 0f;
                
                rutinaActual = StartCoroutine(FadeAudio(volumenMaximo));
                break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[Audio] El jugador ENTRÓ a {gameObject.name}. Subiendo volumen.");
            if (rutinaActual != null) StopCoroutine(rutinaActual);

            // EL TRUCO: Forzamos el volumen a 0 al llegar por si el teletransporte 
            // rompió el evento de salida anterior. ¡Así siempre empezará el Fade!
            if (audioSource != null)
            {
                audioSource.volume = 0f;
            }

            rutinaActual = StartCoroutine(FadeAudio(volumenMaximo));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[Audio] El jugador SALIÓ de {gameObject.name}. Bajando volumen.");
            if (rutinaActual != null) StopCoroutine(rutinaActual);
            rutinaActual = StartCoroutine(FadeAudio(0f));
        }
    }

    private IEnumerator FadeAudio(float volumenObjetivo)
    {
        if (audioSource == null) yield break;

        float volumenInicial = audioSource.volume;
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < tiempoFade)
        {
            tiempoTranscurrido += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(volumenInicial, volumenObjetivo, tiempoTranscurrido / tiempoFade);
            yield return null;
        }

        audioSource.volume = volumenObjetivo;
    }
}