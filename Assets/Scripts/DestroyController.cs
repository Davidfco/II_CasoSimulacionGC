using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DestroyController : MonoBehaviour
{
    [SerializeField]
    LayerMask ignoreMask;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController2D controller = other.GetComponent<CharacterController2D>();
            controller.Die();
            return;
        }

        if ((ignoreMask & (1 << other.gameObject.layer)) != 0)
        {
            return;
        }

        Destroy(other.gameObject);
    }
}
