using UnityEngine;
using UnityEngine.InputSystem;
using ithappy.Animals_FREE;

public class SistemaAgarre : MonoBehaviour
{
    [Header("Ajustes")]
    public Transform puntoDeAgarre;
    public float radioDeAlcance = 1.5f;
    public LayerMask capasAgarrables;

    [Header("Offset")]
    public Vector3 offsetDeAgarre = new Vector3(0f, 0.3f, 0.5f);

    private GameObject objetoActual;
    private Rigidbody rbActual;

    private RigidbodyConstraints constraintsOriginales;

    void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
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
        Collider[] objetosCercanos =
            Physics.OverlapSphere(
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
            Rigidbody rb = col.GetComponent<Rigidbody>();

            if (rb == null)
                continue;

            float distancia =
                Vector3.Distance(
                    transform.position,
                    col.transform.position
                );

            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                objetoMasCercano = col.gameObject;
            }
        }

        if (objetoMasCercano == null)
            return;

        objetoActual = objetoMasCercano;
        rbActual = objetoActual.GetComponent<Rigidbody>();

        // Guardar constraints originales
        constraintsOriginales = rbActual.constraints;

        // Detener movimiento residual
        rbActual.linearVelocity = Vector3.zero;
        rbActual.angularVelocity = Vector3.zero;

        // Desactivar físicas
        rbActual.isKinematic = true;
        rbActual.useGravity = false;

        // Congelar rotación solamente
        rbActual.constraints =
            RigidbodyConstraints.FreezeRotation;

        // Ignorar colisiones con jugador
        Collider[] collidersObjeto =
            objetoActual.GetComponentsInChildren<Collider>();

        Collider[] collidersJugador =
            GetComponentsInChildren<Collider>();

        foreach (Collider colObjeto in collidersObjeto)
        {
            foreach (Collider colJugador in collidersJugador)
            {
                if (colObjeto != null && colJugador != null)
                {
                    Physics.IgnoreCollision(
                        colObjeto,
                        colJugador,
                        true
                    );
                }
            }
        }

        // Avisar que fue agarrado
        CreatureGrabbable grabbable =
            objetoActual.GetComponent<CreatureGrabbable>();

        if (grabbable != null)
        {
            grabbable.estaAgarrada = true;
            
        }
        CreatureMover mover = objetoActual.GetComponent<CreatureMover>();

        if (mover != null)
        {
            mover.enabled = false;
        }

        Animator anim = objetoActual.GetComponent<Animator>();

        if (anim != null)
        {
            anim.enabled = false;
        }



        // Hacer hijo del punto de agarre
        objetoActual.transform.SetParent(puntoDeAgarre);

        objetoActual.transform.localPosition =
            offsetDeAgarre;

        objetoActual.transform.localRotation =
            Quaternion.identity;
    }

    void Soltar()
    {
        if (objetoActual == null)
            return;

        // Restaurar colisiones
        Collider[] collidersObjeto =
            objetoActual.GetComponentsInChildren<Collider>();

        Collider[] collidersJugador =
            GetComponentsInChildren<Collider>();

        foreach (Collider colObjeto in collidersObjeto)
        {
            foreach (Collider colJugador in collidersJugador)
            {
                if (colObjeto != null && colJugador != null)
                {
                    Physics.IgnoreCollision(
                        colObjeto,
                        colJugador,
                        false
                    );
                }
            }
        }

        // Separar del jugador
        objetoActual.transform.SetParent(null);

        // Restaurar físicas
        rbActual.isKinematic = false;
        rbActual.useGravity = true;

        rbActual.constraints = constraintsOriginales;

        // Avisar que ya no está agarrado
        CreatureGrabbable grabbable =
            objetoActual.GetComponent<CreatureGrabbable>();

        if (grabbable != null)
        {
            grabbable.estaAgarrada = false;
            
        }
        CreatureMover mover = objetoActual.GetComponent<CreatureMover>();

        if (mover != null)
        {
            mover.enabled = true;
        }

        Animator anim = objetoActual.GetComponent<Animator>();

        if (anim != null)
        {
            anim.enabled = true;
        }


        // Limpiar referencias
        objetoActual = null;
        rbActual = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color =
            new Color(0, 1, 0, 0.3f);

        Gizmos.DrawSphere(
            transform.position,
            radioDeAlcance
        );
    }
}