using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool initialized = false;
    private Rigidbody2D rigidBody2D;
    private bool isStopping = false;
    public Racer racerInfo;

    private void Start()
    {
        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
    }

    public void InitializePlayer(Racer playerInfo)
    {
        racerInfo = playerInfo;
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.currentGameState == GameState.MainMenu ||
            GameManager.instance.currentGameState == GameState.StartingRace ||
            GameManager.instance.currentGameState == GameState.GameOver)
            return;
        if (PlayerData.instance.canMove)
            CheckPlayerInput();
        else if (!isStopping)
            StartCoroutine(StopCompletelyAfterXSeconds(0.4f));
    }

    #region INPUT
    private void CheckPlayerInput()
    {
        // MOVE
        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            MovePlayer(Direction.Left);
        }
        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            MovePlayer(Direction.Right);
        }
        if (Input.GetAxisRaw("Vertical") < 0)
        {
            MovePlayer(Direction.Down);
        }
        else if (Input.GetAxisRaw("Vertical") > 0)
        {
            MovePlayer(Direction.Up);
        }

        // STOP MOVE
        if (Input.GetAxisRaw("Horizontal") == 0)
        {
            MovePlayer(Direction.Left, false);
        }
        if (Input.GetAxisRaw("Vertical") == 0)
        {
            MovePlayer(Direction.Down, false);
        }
    }
    #endregion

    #region MOVEMENT
    private IEnumerator StopCompletelyAfterXSeconds(float xSeconds)
    {
        isStopping = true;
        yield return new WaitForSeconds(xSeconds);
        StopPlayerCompletely();
    }
    
    private void StopPlayerCompletely()
    {
        MovePlayer(Direction.Up, false);
        MovePlayer(Direction.Right, false);
    }
    private void MovePlayer(Direction direction, bool move = true)
    {
        Vector2 moveVector = Vector2.zero;
        if (move)
        {
            switch (direction)
            {
                case Direction.Down:
                    moveVector.y = -1;
                    break;
                case Direction.Up:
                    moveVector.y = 1;
                    break;
                case Direction.Left:
                    moveVector.x = -1;
                    break;
                case Direction.Right:
                    moveVector.x = 1;
                    break;
                default:
                    Debug.Log("CRAZY ERROR REEEEEE");
                    break;
            }
        }
      
        moveVector = moveVector *
            PlayerData.instance.moveSpeed *
            Time.deltaTime;

        if (rigidBody2D.velocity.x > 0 && moveVector.x > 0)
            return;
        if (rigidBody2D.velocity.x < 0 && moveVector.x < 0)
            return;
        if (rigidBody2D.velocity.y > 0 && moveVector.y > 0)
            return;
        if (rigidBody2D.velocity.y < 0 && moveVector.y < 0)
            return;

        if (move)
            rigidBody2D.velocity += moveVector;
        else
        {
            switch (direction)
            {
                case Direction.Down:
                    rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, 0);
                    break;
                case Direction.Up:
                    rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, 0);
                    break;
                case Direction.Left:
                    rigidBody2D.velocity = new Vector2(0, rigidBody2D.velocity.y);
                    break;
                case Direction.Right:
                    rigidBody2D.velocity = new Vector2(0, rigidBody2D.velocity.y);
                    break;
                default:
                    Debug.Log("CRAZY ERROR REEEEEE2");
                    break;
            }
        }
    }
    #endregion
}
