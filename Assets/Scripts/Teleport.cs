using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Teleport : MonoBehaviour
{
    public Transform Target;
    public GameObject Player;

    private void OnTriggerEnter(Collider other)
    {
        Player.transform.position = Target.transform.position;
        Player.transform.rotation = Target.transform.rotation;
    }
}
