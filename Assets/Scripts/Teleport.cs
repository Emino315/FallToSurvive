using UnityEngine;

public class ScreenTeleport : MonoBehaviour
{
    private float teleportX = 13f;  
    private float teleportY = 10.5f;  
    private float maxFallSpeed = -6f;

    private Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (body.velocity.y < maxFallSpeed)
        {
            body.velocity = new Vector2(body.velocity.x, maxFallSpeed);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Vector3 newPosition = transform.position;

        if (other.CompareTag("TriggerL") && body.velocity.x < 0)
        {
            newPosition.x += teleportX;
        }
        else if (other.CompareTag("TriggerR") && body.velocity.x > 0)
        {
            newPosition.x -= teleportX;
        }
        else if (other.CompareTag("TriggerUp") && body.velocity.y > 0)
        {
            newPosition.y -= teleportY;
        }
        else if (other.CompareTag("TriggerDown") && body.velocity.y < 0)
        {
            newPosition.y += teleportY;
        }
        transform.position = newPosition;  
    }
}
