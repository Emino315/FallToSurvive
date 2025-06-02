
using UnityEngine;


public class playerScript : MonoBehaviour
{
    private Rigidbody2D body;
    private float scale = 2.8f;
    private float speed = 3f;
    private float speed_in_air = 2.2f;
    private float jump_speed = 6.5f;
    private Animator anim;
    private bool grounded;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        transform.position = new Vector3(-4.5f, -4.5f, 0);
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        grounded = IsGrounded();

        if (grounded)        {
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
        }
        else        {
            body.velocity = new Vector2(horizontalInput * speed_in_air, body.velocity.y);
        }

        if (body.velocity.y < -3f)
        {
            grounded = false;
        }

        if (horizontalInput > 0.01f)        {
            transform.localScale = new Vector3(scale, scale, scale);
        }

        if (horizontalInput < -0.01f)        {
            transform.localScale = new Vector3(-scale, scale, scale);
        }

        if (Input.GetKey(KeyCode.Space) && grounded)
            Jump();

        anim.SetBool("run", horizontalInput != 0 && body.velocity.y > -1f);
        anim.SetBool("grounded", grounded);

        if (body.velocity.y < -6f)
        {
            body.velocity = new Vector2(body.velocity.x, -6f);
        }
    }

    private void Jump()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(horizontalInput * speed, jump_speed);
        anim.SetTrigger("jump");
        grounded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            grounded = true;
        }
    }
    private bool IsGrounded()
    {
        float extraHeight = 0.1f;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, extraHeight);

        if (hit.collider != null && hit.collider.CompareTag("ground"))
        {
            return true;
        }
        return false;
    }
}
