using UnityEngine;
using ithappy.Animals_FREE;

public class CreatureFlee : MonoBehaviour
{
    public Transform player;

    public float detectDistance = 10f;

    private CreatureMover mover;
    private CreatureGrabbable grabbable;

    private void Awake()
    {
        mover = GetComponent<CreatureMover>();
        grabbable = GetComponent<CreatureGrabbable>();
    }

    private void Update()
    {
        // Si está agarrada no hace nada
        if (grabbable != null && grabbable.estaAgarrada)
        {
            mover.SetInput(Vector2.zero, transform.position + transform.forward, false, false);
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        // Huir
        if (distance < detectDistance)
        {
            Vector3 fleeDirection = (transform.position - player.position).normalized;

            Vector3 target = transform.position + fleeDirection;

            Vector2 axis = new Vector2(0f, 1f);

            mover.SetInput(axis, target, true, false);
        }
        else
        {
            mover.SetInput(Vector2.zero, transform.position + transform.forward, false, false);
        }
    }
}