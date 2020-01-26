using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            playerController.gameObject.GetComponent<CircleCollider2D>().enabled = false;
            GameManager.instance.RegisterParticipantReachingFinish(playerController.racerInfo, true);
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            EnemyController enemyController = collision.gameObject.GetComponent<EnemyController>();
            enemyController.gameObject.GetComponent<CircleCollider2D>().enabled = false;
            GameManager.instance.RegisterParticipantReachingFinish(enemyController.racerInfo);
        }
    }
}
