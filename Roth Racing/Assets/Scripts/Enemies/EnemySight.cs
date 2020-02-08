using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    EnemyController enemyController;

    void Start()
    {
        enemyController = transform.parent.gameObject.GetComponent<EnemyController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !enemyController.hasNoticedPlayer)
        {
            enemyController.NoticePlayer();
        }
    }

}
