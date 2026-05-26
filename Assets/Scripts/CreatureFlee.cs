using UnityEngine;
using ithappy.Animals_FREE;
public class CreatureFlee : MonoBehaviour
{
    public Transform player;

    public float detectDistance = 10f;
    public float stopDistance = 20f;

    private CreatureMover mover;

    private void Awake()
    {
        mover = GetComponent<CreatureMover>();
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // Si el jugador está cerca
        if (distance < detectDistance)
        {
            // Dirección opuesta al jugador
            Vector3 fleeDirection = (transform.position - player.position).normalized;

            // Punto hacia donde mirar/moverse
            Vector3 target = transform.position + fleeDirection;

            // Mover hacia adelante corriendo
            Vector2 axis = new Vector2(0f, 1f);

            mover.SetInput(axis, target, true, false);
        }
        else
        {
            // Detenerse
            mover.SetInput(Vector2.zero, transform.position + transform.forward, false, false);
        }
    }
}