using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 30f;

    private Vector2 targetPosition;
    private Vector2 dirNormalized;
    private Rigidbody2D rigidBody2D;

    public Racer racerInfo;
    private void Start()
    {
        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
    }

    public void InitializeEnemy(Racer enemyInfo)
    {
        racerInfo = enemyInfo;
        moveSpeed = racerInfo.moveSpeed;
        targetPosition = GameManager.instance.finishLineCoordinates;
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.currentGameState == GameState.MainMenu ||
           GameManager.instance.currentGameState == GameState.StartingRace ||
           GameManager.instance.currentGameState == GameState.GameOver)
            return;

        SetDirNormalized();
        MoveEnemy();
    }

    private void SetDirNormalized()
    {
        dirNormalized = new Vector2(targetPosition.x - transform.position.x, targetPosition.y - transform.position.y);
        dirNormalized = dirNormalized.normalized;
    }

    private void MoveEnemy()
    {
        Vector2 moveVector = dirNormalized *
            moveSpeed *
            Time.deltaTime;

        rigidBody2D.velocity = moveVector;
    }

}
