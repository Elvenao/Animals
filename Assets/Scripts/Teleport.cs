using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform destination;


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entró: " + other.gameObject.name + " Tag: " + other.tag);

        if (other.CompareTag("Player"))
        {
            CharacterController cc = other.GetComponent<CharacterController>();
            cc.enabled = false;
            other.transform.position = destination.position ;
            cc.enabled = true;
        }
    }
}