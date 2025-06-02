using System.Collections; 
using UnityEngine;

public class Slime : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float directionChangeInterval = 3f;

  
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float wallDetectionDistance = 0.5f;

    [SerializeField] private float attackRange = 0.3f; 
    [SerializeField] private float attackCooldown = 1.5f; 
    private bool canAttack = true;

    private float scale = 5f;
    private bool canMove = true;
    private bool waitingToTurn = false;
    [SerializeField] private float turnDelay = 0.7f;
    private bool counted = false;

    private Rigidbody2D body;
    private Animator anim;
    private bool movingRight = true;
    private bool isGrounded = false;
    private float damage = 1;
    private bool isDead = false;

    [SerializeField] public GameVictory gameVictory;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        StartCoroutine(ChangeDirectionRoutine());
        StartCoroutine(JumpRoutine());
    }

    private void Update()
    {
        if (!canMove || isDead || anim.GetCurrentAnimatorStateInfo(0).IsName("attack")) return;
        DetectPlayer();
        if (isGrounded && IsWallAhead() && !waitingToTurn)  {
            StartCoroutine(ChangeDirectionAfterDelay());
            return;
        }

        if (isDead) ShouldDie();

        body.velocity = new Vector2(movingRight ? moveSpeed : -moveSpeed, body.velocity.y);

        if (isGrounded)
        {
            anim.SetBool("jump", false);
            anim.SetBool("idle", body.velocity.x == 0);
        }
        else
        {
            anim.SetBool("jump", body.velocity.y > 0);
            anim.SetBool("idle", body.velocity.y <= 0);
        }
        if (body.velocity.y < -6f)
        {
            body.velocity = new Vector2(body.velocity.x, -6f);
        }
    }

    private IEnumerator Attack(GameObject player)
    {
        canAttack = false;
        anim.SetTrigger("attack"); 
        yield return new WaitForSeconds(0.5f); 
        Jump();
        yield return new WaitForSeconds(attackCooldown); 
        canAttack = true;
    }

    private void DetectPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);

        if (playerCollider != null && canAttack && isGrounded)
        {
            StartCoroutine(Attack(playerCollider.gameObject));
        }
    }


    private IEnumerator ChangeDirectionAfterDelay()
    {
        waitingToTurn = true;
        canMove = false;
        body.velocity = Vector2.zero; 
        anim.Play("idle"); 
        anim.SetBool("idle", true);  
        yield return new WaitForSeconds(turnDelay);

        movingRight = !movingRight;
        transform.localScale = new Vector3(movingRight ? scale : -scale, scale, scale);
        canMove = true;
        waitingToTurn = false;
    }
    private bool IsWallAhead() {
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, wallDetectionDistance, groundLayer);
        return hit.collider != null;
    }

    private void Jump() 
    {
        body.velocity = new Vector2(body.velocity.x, jumpForce);
        anim.SetTrigger("jump");
    }

    private IEnumerator ChangeDirectionRoutine()
    {
        while (true) {
            yield return new WaitForSeconds(directionChangeInterval);
            if (isGrounded)  {
                movingRight = !movingRight;
                transform.localScale = new Vector3(movingRight ? scale : -scale, scale, scale);
            }
        }
    }
    private IEnumerator JumpRoutine()
    {
        while (true) {
            yield return new WaitForSeconds(Random.Range(2f, 5f));
            if (isGrounded) {
                Jump();
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)   {
        if (collision.gameObject.CompareTag("ground"))
        {
            isGrounded = true;
            if (isDead){ Die(); }
        }
        if (collision.gameObject.CompareTag("Player") && !isDead)
        {
            collision.gameObject.GetComponent<Health>().TakeDamage(damage);
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 repelDirection = (collision.transform.position - transform.position).normalized;
                float repelForce = 5f; 
                playerRb.velocity = Vector2.zero; 
                playerRb.AddForce(repelDirection * repelForce, ForceMode2D.Impulse);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isDead == false)
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            body.velocity = new Vector2(0,body.velocity.y);
            if (playerRb.velocity.y < 0)
                {
                playerRb.velocity = new Vector2(playerRb.velocity.x, 5f);
                isDead = true;
                canMove = false;
                if (isGrounded)
                {
                   Die();
                }
            }
        }
    }
 
    private void Die()
    {
        anim.SetTrigger("die");
        StartCoroutine(BlinkBeforeDeath());
    }

    private IEnumerator BlinkBeforeDeath()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        float duration = 0.25f;
        float blinkSpeed = 0.06f; 

        for (float t = 0; t < duration; t += blinkSpeed)
        {
            sprite.enabled = !sprite.enabled;
            yield return new WaitForSeconds(blinkSpeed);
        }
        sprite.enabled = true; 
        if (counted == false)
        {
            counted = true;
            gameVictory.addOneDead();
        }
        Destroy(gameObject);
    }

    private IEnumerator ShouldDie()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 detectionOffset = movingRight ? Vector2.right : Vector2.left;
        Vector3 detectionPosition = transform.position + (Vector3)detectionOffset * 0.4f;
        Gizmos.DrawWireSphere(detectionPosition, 0.3f); 
    }

}
