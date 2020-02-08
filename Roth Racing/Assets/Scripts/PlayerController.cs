using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private AudioManager audioManager;
    private bool initialized = false;
    private Rigidbody2D rigidBody2D;
    private bool isStopping = false;
    public Racer racerInfo;

    #region COMBAT DATA
    [Header("COMBAT")]
    [SerializeField]
    GameObject playerProjectile;
    float projectileSpeed = 700f;
    float projectileDuration = 0.9f;
    private bool canShoot = true;
    #endregion

    #region ANIMATION DATA
    [Header("ANIMATIONS")]
    [SerializeField]
    private SpriteRenderer playerSprite;
    [SerializeField]
    private Sprite sideSprite;
    [SerializeField]
    private Sprite upSprite;
    [SerializeField]
    private Sprite downSprite;
    private Direction moveDirection = Direction.Right;
    #endregion

    private void Start()
    {
        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
        PlayerData.instance.currLife = PlayerData.instance.maxLife;
    }

    public void InitializePlayer(Racer playerInfo)
    {
        racerInfo = playerInfo;
        audioManager = GameManager.instance.gameObject.GetComponent<AudioManager>();
        GameManager.instance.UpdatePlayerAmmoHUD();
        GameManager.instance.UpdatePlayerLifeHUD();
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

    private void LateUpdate()
    {
        UpdatePlayerSprite();
    }

    #region ANIMATION METHODS
    private void UpdatePlayerSprite()
    {
        switch (moveDirection)
        {
            case Direction.Right:
                playerSprite.flipX = false;
                playerSprite.sprite = sideSprite;
                break;
            case Direction.Left:
                playerSprite.flipX = true;
                playerSprite.sprite = sideSprite;
                break;
            case Direction.Up:
                playerSprite.sprite = upSprite;
                break;
            case Direction.Down:
                playerSprite.sprite = downSprite;
                break;
        }
    }
    #endregion

    #region INPUT
    private void CheckPlayerInput()
    {
        // SHOOT
        if (Input.GetKey(KeyCode.Space))
        {
            ManageProjectileShooting();
        }

        // MOVE
        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            moveDirection = Direction.Left;
            MovePlayer(Direction.Left);
        }
        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            moveDirection = Direction.Right;
            MovePlayer(Direction.Right);
        }
        if (Input.GetAxisRaw("Vertical") < 0)
        {
            moveDirection = Direction.Down;
            MovePlayer(Direction.Down);
        }
        else if (Input.GetAxisRaw("Vertical") > 0)
        {
            moveDirection = Direction.Up;
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

    #region COMBAT METHODS
    private void ManageProjectileShooting()
    {
        if (PlayerData.instance.currAmmo <= 0)
            return;
        if (!canShoot)
            return;
        canShoot = false;
        StartCoroutine(AllowShootingAfterCooldown());
        PlayerData.instance.currAmmo--;
        GameManager.instance.UpdatePlayerAmmoHUD();
        if (moveDirection == Direction.Up || moveDirection == Direction.Down)//(playerSprite == upSprite || playerSprite == downSprite)
        {
            ShootProjectile(Direction.Left);
            ShootProjectile(Direction.Right);
        }
        else
        {
            ShootProjectile(Direction.Up);
            ShootProjectile(Direction.Down);
        }
    }

    private IEnumerator AllowShootingAfterCooldown()
    {
        yield return new WaitForSeconds(PlayerData.instance.attackCooldown);
        canShoot = true;
    }

    private void ShootProjectile(Direction shootDirection)
    {
        GameObject newProjectileObject = Instantiate(playerProjectile, transform);
        newProjectileObject.transform.parent = null;

        Projectile newProjectile = newProjectileObject.GetComponent<Projectile>();
        newProjectile.InitializeProjectile(true, shootDirection, projectileDuration, projectileSpeed, PlayerData.instance.projectileDamage);
    }

    public void TakeDamage(int amount)
    {
        GameManager.instance.UpdatePlayerLifeHUD();
        PlayerData.instance.currLife -= amount;
        if (PlayerData.instance.currLife <= 0)
        {
            Die();
            return;
        }
        audioManager.PlayShipDamagedSFX();
    }

    private void Die()
    {
        GameManager.instance.ShowGameResults();
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
