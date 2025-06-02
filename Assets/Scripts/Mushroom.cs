using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    private float moveSpeed = 1.5f;
    private float wallDetectionDistance = 0.5f;
    private float attackRange = 0.5f;
    private float jumpForce = 4.3f;
    private float attackCooldown = 1f;
    private float scale = 2.5f;

    private bool canAttack = true;
    private bool canMove = true;
    private bool waitingToTurn = false;
    private bool isGrounded = false;
    private bool movingRight = true;
    private bool isDead = false;
    private bool wasHit = false;
    private bool counted = false;
    private bool PlayerInvincible = false;
    private float damage = 1f;

    private Rigidbody2D body;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
   

    [SerializeField] public GameVictory gameVictory;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask groundLayer;
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(JumpRoutine());
        StartCoroutine(ChangeDirectionRoutine());
    }

    private void Update()
    {
        if (!canMove || isDead || anim.GetCurrentAnimatorStateInfo(0).IsName("attack")) return;

        DetectPlayer();

        if (isGrounded && IsWallAhead() && !waitingToTurn)
        {
            StartCoroutine(TryJumpThenTurn());
            //StartCoroutine(ChangeDirectionAfterDelay());
            return;
        }

        body.velocity = new Vector2(movingRight ? moveSpeed : -moveSpeed, body.velocity.y);

        if (isGrounded)
        {
            bool isMoving = Mathf.Abs(body.velocity.x) > 0.01f;
            anim.SetBool("idle", !isMoving);
            anim.SetBool("run", isMoving);
        }
        else
        {
            anim.SetBool("idle", false);
            anim.SetBool("run", false);
        }
        if (body.velocity.y < -6f)
        {
            body.velocity = new Vector2(body.velocity.x, -6f);
        }
    }

    private IEnumerator TryJumpThenTurn()
    {
        waitingToTurn = true;
        Jump();
        float waiting = Random.Range(0.7f, 1.7f);
        yield return new WaitForSeconds(waiting);
        FlipDirection();    
        waitingToTurn = false;
    }
    private void DetectPlayer()
    {
        Vector2 horizontalOffset = (movingRight ? Vector2.right : Vector2.left) * attackRange;
        Vector2 verticalOffset = Vector2.up * 0.3f;
        Vector2 detectionPosition = (Vector2)transform.position + (horizontalOffset + verticalOffset);

        Collider2D playerCollider = Physics2D.OverlapCircle(detectionPosition, attackRange, playerLayer);

        if (playerCollider != null && canAttack && isGrounded)
        {
            StartCoroutine(Attack(playerCollider.gameObject));
        }
    }


    private IEnumerator Attack(GameObject player)
    {
        canAttack = false; 
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f); // Wait for hit timing
        Vector2 direction = (player.transform.position - transform.position).normalized;
        float dashDirection = Mathf.Sign(direction.x); // +1 ou -1
        body.velocity = new Vector2(dashDirection * 5f, body.velocity.y);
        yield return new WaitForSeconds(0.27f);
        body.velocity = new Vector2(0, body.velocity.y);

        yield return new WaitForSeconds(attackCooldown-0.2f);
        canAttack = true; canMove = true;
    }

    private IEnumerator ChangeDirectionRoutine()
    {
        while (!isDead)
        {
            float directionChangeInterval = Random.Range(3f, 8f);
            yield return new WaitForSeconds(directionChangeInterval);
            if (canMove && isGrounded)
            {
                FlipDirection();
            }
        }
    }

     private bool IsWallAhead()
    {
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, wallDetectionDistance, groundLayer);
        return hit.collider != null;
    }

    private void FlipDirection()
    {
        movingRight = !movingRight;
        transform.localScale = new Vector3(movingRight ? scale : -scale, scale, scale);
    }

    private void Jump()
    {
        body.velocity = new Vector2(body.velocity.x, jumpForce);
    }

    private IEnumerator JumpRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 6f));
            if (isGrounded)
            {
                Jump();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            isGrounded = true;
            if (isDead) Die();
        }

        if (collision.gameObject.CompareTag("Player") && !isDead)
        {
            if (!PlayerInvincible)
            {
                collision.gameObject.GetComponent<Health>()?.TakeDamage(damage);
            }
            repelPlayer(collision, 5f);
        }
    }

    private void repelPlayer(Collision2D collision, float strength)
    {
        Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            Vector2 repelDirection = (collision.transform.position - transform.position).normalized;
            float repelForce = strength;
            playerRb.velocity = Vector2.zero;
            playerRb.AddForce(repelDirection * repelForce, ForceMode2D.Impulse);
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
        if (collision.gameObject.CompareTag("Player") && !isDead)
        {
            TriggerInvincibility(1f);
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb.velocity.y < 0)
            {
                float signe = Mathf.Sign(playerRb.velocity.x);
                playerRb.velocity = new Vector2(playerRb.velocity.x + 3*signe , 5f);

                if (!wasHit)
                {
                    wasHit = true;
                    anim.SetTrigger("hit"); 
                }
                else
                {
                    isDead = true;
                    canMove = false;
                    if (isGrounded) Die();
                }
                body.velocity = new Vector2(0, body.velocity.y+5);
            }
        }
    }

    public void TriggerInvincibility(float duration)
    {
        StartCoroutine(InvincibilityRoutine(duration));
    }

    private IEnumerator InvincibilityRoutine(float duration)
    {
        PlayerInvincible = true;
        yield return new WaitForSeconds(duration);
        PlayerInvincible = false;
    }

    private void Die()
    {
        anim.SetTrigger("die");
        if (!counted)
        {
            counted = true;
            gameVictory.addOneDead();
        }
        StartCoroutine(Blink());
        StartCoroutine(ShouldDie());
    }

  
    private IEnumerator Blink()
    {
        float duration = 0.25f;
        float blinkSpeed = 0.06f;

        for (float t = 0; t < duration; t += blinkSpeed)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkSpeed);
        }

        spriteRenderer.enabled = true;        
    }

    private IEnumerator ShouldDie()
    {
        yield return new WaitForSeconds(0.7f);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 horizontalOffset = (movingRight ? Vector2.right : Vector2.left) * 0.4f;
        Vector2 verticalOffset = Vector2.up * 0.3f;
        Vector3 detectionPosition = transform.position + (Vector3)(horizontalOffset + verticalOffset);
        Gizmos.DrawWireSphere(detectionPosition, attackRange);

    }
}
