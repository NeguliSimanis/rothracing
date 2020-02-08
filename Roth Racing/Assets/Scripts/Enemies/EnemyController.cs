using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private AudioManager audioManager;

    #region ENEMY STATE
    private EnemyState currentState = EnemyState.Patrolling;
    [HideInInspector]
    public bool hasNoticedPlayer = false;
    private bool isDamaged = false;
    #endregion

    private float currentMoveSpeed;
    private float defaultMoveSpeed;
    private float patrolMoveSpeed;

    private Transform targetTransform;
    private Vector2 dirNormalized;
    private Rigidbody2D rigidBody2D;

    public Racer racerInfo;

    #region COMBAT
    [Header("COMBAT")]
    [SerializeField]
    private GameObject enemyProjectile;
    private int attackDamage = 1;
    private float projectileFlyDistance = 2f;
    private float projectileFlyDuration = 0.7f;
    private float projectileSpeed = 500f;
    private float attackCooldown = 2f;
    #endregion

    #region ANIMATIONS
    [Header("ANIMATIONS")]
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private SpriteRenderer enemySprite;
    [SerializeField]
    private Sprite sideSprite;
    [SerializeField]
    private Sprite upSprite;
    [SerializeField]
    private Sprite downSprite;
    private Direction moveDirection = Direction.Right;
    #endregion

    #region PATROLLING
    Transform[] patrolWaypoints;
    #endregion

    #region DEBUG METH
    private void RaycastDebug()
    {
        Vector3 forward = transform.TransformDirection(new Vector3(projectileFlyDistance, 0));
        Debug.DrawRay(transform.position, forward, Color.green);
    }
    #endregion

    private void Start()
    {
        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
        audioManager = GameManager.instance.gameObject.GetComponent<AudioManager>();
    }

    public void InitializeEnemy(Racer enemyInfo, Transform[] moveWaypoints)
    {
        racerInfo = enemyInfo;
        
        currentMoveSpeed = racerInfo.moveSpeed;
        defaultMoveSpeed = currentMoveSpeed;
        patrolMoveSpeed = 0.8f * defaultMoveSpeed;

        targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
        patrolWaypoints = moveWaypoints;
        InitializeEnemyState();
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.currentGameState == GameState.MainMenu ||
           GameManager.instance.currentGameState == GameState.StartingRace ||
           GameManager.instance.currentGameState == GameState.GameOver)
            return;
        ManageMovement();
        UpdateEnemySprite();
        RaycastDebug();
    }

    #region STATE MANAGEMENT
    private void InitializeEnemyState()
    {
        currentState = EnemyState.Patrolling;
        ChooseNextPatrolLocation();
    }
    #endregion

    #region PATROLLING METHODS
    private void ChooseNextPatrolLocation()
    {
        int nextLocationID = Random.Range(0, patrolWaypoints.Length);
        Transform nextLocation = patrolWaypoints[nextLocationID];
        while (targetTransform == nextLocation)
        {
            nextLocationID = Random.Range(0, patrolWaypoints.Length);
            nextLocation = patrolWaypoints[nextLocationID];
        }
        targetTransform = nextLocation;
    }
    #endregion

    #region ANIMATION METHODS
    private void UpdateEnemySprite()
    {
        if (isDamaged)
        {
            UpdateEnemyHurtSprite();
            return;
        }
        switch (moveDirection)
        {
            case Direction.Right:
                transform.localScale = new Vector3(-1, 1, 1);
                enemySprite.sprite = sideSprite;
                break;
            case Direction.Left:
                transform.localScale = new Vector3(1, 1, 1);
                enemySprite.sprite = sideSprite;
                break;
            case Direction.Up:
                enemySprite.sprite = upSprite;
                break;
            case Direction.Down:
                enemySprite.sprite = downSprite;
                break;
        }
    }

    private void UpdateEnemyHurtSprite()
    {
        switch (moveDirection)
        {
            case Direction.Right:
                transform.localScale = new Vector3(-1, 1, 1);
                animator.SetFloat("hurtDirection", 0f);
                break;
            case Direction.Left:
                transform.localScale = new Vector3(1, 1, 1);
                animator.SetFloat("hurtDirection", 0f);
                break;
            case Direction.Up:
                animator.SetFloat("hurtDirection", 1f);
                break;
            case Direction.Down:
                animator.SetFloat("hurtDirection", 0.5f);
                break;
        }
    }
    public void NoticePlayer()
    {
        if (hasNoticedPlayer)
            return;
        hasNoticedPlayer = true;
        currentState = EnemyState.ChasingPlayer;
        targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(ShootProjectileManager());
    }
    #endregion

    #region COMBAT METHODS
    public void TakeDamage(int amount)
    {
        if (!isDamaged)
        {
            isDamaged = true;
            animator.SetBool("hurt", true);
        }
        if (!hasNoticedPlayer)
        {
            NoticePlayer();
        }
        racerInfo.racerCurrentHP -= amount;
        if (racerInfo.racerCurrentHP <= 0)
        {
            Die();
        }
        audioManager.PlayShipDamagedSFX();
    }


    private IEnumerator ShootProjectileManager()
    {
        if (moveDirection == Direction.Up || moveDirection == Direction.Down)
        {
            ShootProjectile(Direction.Left);
            ShootProjectile(Direction.Right);
        }
        else
        {
            ShootProjectile(Direction.Up);
            ShootProjectile(Direction.Down);
        }
        yield return new WaitForSeconds(attackCooldown);
        StartCoroutine(ShootProjectileManager());
    }

    private void ShootProjectile(Direction shootDirection)
    {
        GameObject newProjectileObject = Instantiate(enemyProjectile, transform);
        newProjectileObject.transform.parent = null;

        Projectile newProjectile = newProjectileObject.GetComponent<Projectile>();
        newProjectile.InitializeProjectile(false, shootDirection, projectileFlyDuration, projectileSpeed, attackDamage);
    }
    private void Die()
    {
        GameManager.instance.gameObject.GetComponent<AudioManager>().PlayEnemyKilledSFX();
        Destroy(gameObject);
    }
    #endregion

    #region MOVEMENT METHODS
    private void ManageMovement()
    {
        SetDirNormalized();
        SetMoveSpeed();
        MoveEnemy();

        if (currentState == EnemyState.Patrolling && IsTargetPositionReached())
        {
            ChooseNextPatrolLocation();
        }
    }

    private void SetMoveSpeed()
    {
        switch (currentState)
        {
            case (EnemyState.Patrolling):
                currentMoveSpeed = patrolMoveSpeed;
                break;
            default:
                currentMoveSpeed = defaultMoveSpeed;
                break;
        }
    }

    private bool IsTargetPositionReached()
    {
        float approxError = 0.82f;

        bool isNearTargetX = false;
        bool isNearTargetY = false;

        if (MathUtils.RoughlyEqual(transform.position.x, targetTransform.position.x, approxError))
            isNearTargetX = true;
        if (MathUtils.RoughlyEqual(transform.position.y, targetTransform.position.y, approxError))
            isNearTargetY = true;

        if (isNearTargetX && isNearTargetY)
            return true;

        return false;
    }

    private void SetDirNormalized()
    {
        dirNormalized = new Vector2(targetTransform.position.x - transform.position.x, targetTransform.position.y - transform.position.y);
        dirNormalized = dirNormalized.normalized;

        bool isXBigger = false;
        if (Mathf.Abs(dirNormalized.x) >= Mathf.Abs(dirNormalized.y))
            isXBigger = true;

        if (isXBigger)
        {
            if (dirNormalized.x > 0)
                moveDirection = Direction.Left;
            else
                moveDirection = Direction.Right;
        }
        else
        {
            if (dirNormalized.y > 0)
                moveDirection = Direction.Up;
            else
                moveDirection = Direction.Down;
        }
    }

    private void MoveEnemy()
    {
        Vector2 moveVector = dirNormalized *
            currentMoveSpeed *
            Time.deltaTime;

        rigidBody2D.velocity = moveVector;
    }

    #endregion

}
