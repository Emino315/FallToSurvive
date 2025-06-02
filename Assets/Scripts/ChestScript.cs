using System.Collections;
using UnityEngine;

public class ChestScript : MonoBehaviour
{
    [SerializeField] public Sprite openChestSprite;
    [SerializeField] public GameObject itemPrefab;
    private SpriteRenderer spriteRenderer;
    private bool isOpened = false;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (itemPrefab != null)
        {
            itemPrefab.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isOpened)
        {
            isOpened = true;
            StartCoroutine(OpenChestWithDelay()); 
        }
    }

    private IEnumerator OpenChestWithDelay()
    {
        yield return new WaitForSeconds(0.5f); 
        OpenChest(); 
    }


    void OpenChest()
    {
        isOpened = true;
        spriteRenderer.sprite = openChestSprite;

        if (itemPrefab != null)
        {
            itemPrefab.SetActive(true); 
            Rigidbody2D rb = itemPrefab.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = new Vector2(0, 1.5f); 
            }
        }
    }
}