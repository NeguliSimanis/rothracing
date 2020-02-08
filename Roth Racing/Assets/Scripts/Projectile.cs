using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private GameObject projectileSpriteObject;
    private Rigidbody2D rigidBody2D;

    private bool isInitialized = false;
    private bool isPlayerProjectile;
    private Direction shootDirection;
    private float projectileDuration;
    private float projectileSpeed;
    private Vector3 moveVector;
    private int projectileDamage;

    [SerializeField]
    Sprite enemyProjectileSprite;
    [SerializeField]
    SpriteRenderer projectileSpriteRenderer;

    [SerializeField]
    GameObject plunchAnimation;
    AudioManager audioManager;

    Vector2 initialVelocity;
    Vector2 deleteProjVelocity;

    private void Start()
    {
        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
    }

    public void InitializeProjectile(bool isPlayer, Direction direction, float duration, float speed, int damage)
    {
        audioManager = GameManager.instance.gameObject.GetComponent<AudioManager>();
        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
        projectileSpriteObject = transform.GetChild(0).gameObject;
        isInitialized = true;
        isPlayerProjectile = isPlayer;
        shootDirection = direction;
        projectileDuration = duration;
        projectileSpeed = speed;
        projectileDamage = damage;

        switch (shootDirection)
        {
            case Direction.Right:
                moveVector = new Vector3(speed, 0);
                break;
            case Direction.Up:
                moveVector = new Vector3(0, speed);
                break;
            case Direction.Down:
                moveVector = new Vector3(0, -speed);
                break;
            case Direction.Left:
                moveVector = new Vector3(-speed, 0);
                break;
        }
        AdjustSpriteRotation();
        PlayShootSFX();
        SetupProjectileRendering();
        ShootProjectile();


    }

    private void PlayShootSFX()
    {
        if (isPlayerProjectile)
        GameManager.instance.gameObject.GetComponent<AudioManager>().PlayPlayerCannonSFX();
    }

    private void SetupProjectileRendering()
    {
        if (isPlayerProjectile)
            gameObject.layer = 8;
        else
        {
            gameObject.layer = 9;
            projectileSpriteRenderer.sprite = enemyProjectileSprite;
        }
    }

    private void Update()
    {
        //if (rigidBody2D.velocity.x < 0 && deleteProjVelocity.x < rigidBody2D.velocity.x)

        //{
        //    Debug.Log("yay");
        //    SinkProjectile();
        //}
        //if (rigidBody2D.velocity.x > 0 && deleteProjVelocity.x > rigidBody2D.velocity.x)
        //{
        //    Debug.Log("yay");
        //    SinkProjectile();
        //}
        //if (rigidBody2D.velocity.y < 0 && deleteProjVelocity.y < rigidBody2D.velocity.y)
        //{
        //    Debug.Log("yay");
        //    SinkProjectile();
        //}
        //if (rigidBody2D.velocity.y > 0 && deleteProjVelocity.y > rigidBody2D.velocity.y)
        //{
        //    Debug.Log("yay");
        //    SinkProjectile();
        //}
     }

    private void AdjustSpriteRotation()
    {
        switch (shootDirection)
        {
            case Direction.Right:
                projectileSpriteObject.transform.eulerAngles = new Vector3(0, 0, -45);
                break;
            case Direction.Up:
                projectileSpriteObject.transform.eulerAngles = new Vector3(0, 0, 45);
                break;
            case Direction.Down:
                projectileSpriteObject.transform.eulerAngles = new Vector3(0, 0, -135);
                break;
            case Direction.Left:
                projectileSpriteObject.transform.eulerAngles = new Vector3(0, 0, 135);
                break;
        }
    }

    private void ShootProjectile()
    {
        rigidBody2D.AddForce(moveVector);
        initialVelocity = rigidBody2D.velocity;
        deleteProjVelocity = initialVelocity * 0.9f;

        Debug.Log(initialVelocity);
        Debug.Log(deleteProjVelocity);
        StartCoroutine(DestroyAfterDuration());
    }

    private IEnumerator DestroyAfterDuration()
    {
        yield return new WaitForSeconds(projectileDuration);
        SinkProjectile();
    }

    private void SinkProjectile()
    {
        GameObject newPlunch = Instantiate(plunchAnimation, transform);
        newPlunch.transform.parent = null;
        audioManager.PlayPlunchSFX(isPlayerProjectile);
        DestroyProjectile();
    }

    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPlayerProjectile && collision.gameObject.tag == "Enemy")
        {
            EnemyController enemyController = collision.gameObject.GetComponent<EnemyController>();
            enemyController.TakeDamage(projectileDamage);
            DestroyProjectile();
        }
        else if (!isPlayerProjectile && collision.gameObject.tag == "Player")
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            playerController.TakeDamage(projectileDamage);
            DestroyProjectile();
        }
    }
}
