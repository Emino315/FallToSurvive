using UnityEngine;
public class LifePickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.AddHealth(1f);
            }
            Destroy(gameObject);
        }
    }
}
