using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField]
    private PickupType type;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            switch (type)
            {
                case PickupType.Ammo:
                    PlayerData.instance.PickupAmmo();
                    break;
                default:
                    Debug.Log("PICKUP TYPE NOT DEFINED");
                    break;
            }
            Destroy(gameObject);
        }
    }
}
