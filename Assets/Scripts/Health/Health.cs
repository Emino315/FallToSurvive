using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    private float startingHealth = 3;
    private GameOverScript GameOver;
    public float currentHealth { get; private set; }
    private Animator anim;
    private bool dead;
    private bool invincible = false;
    
    public void Awake()  {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        GameOver = GetComponent<GameOverScript>();
        dead = false;
    }
    public void TakeDamage(float damage)
    {
        if (invincible) return;
        currentHealth = Mathf.Clamp(currentHealth-damage, 0, startingHealth);
        if (currentHealth > 0)  {
            anim.SetTrigger("hurt");
            StartCoroutine(Blink());
            StartCoroutine(InvincibilityTimer());
        }
        else
        {
            if (!dead)
            {
                anim.SetTrigger("die");
                GetComponent<playerScript>().enabled = false;
                gameObject.layer = LayerMask.NameToLayer("DeadPlayer"); 
                dead = true;
                StartCoroutine(GameOverSequence()); 
            }
           
        }
    }

    private IEnumerator GameOverSequence()
    {
        yield return new WaitForSeconds(2f);
        Time.timeScale = 0f; 
        GameOver.Setup();
    }

    private IEnumerator Blink()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        float duration = 0.25f;
        float blinkSpeed = 0.06f; 

        for (float t = 0; t < duration; t += blinkSpeed) {
            sprite.enabled = !sprite.enabled;
            yield return new WaitForSeconds(blinkSpeed);
        }
        sprite.enabled = true;
    }
    public void AddHealth(float amount)
    {
          currentHealth = Mathf.Clamp(currentHealth + amount, 0, startingHealth+1);
    }


    private IEnumerator InvincibilityTimer()
    {
        invincible = true; 
        yield return new WaitForSeconds(1f); 
        invincible = false; 
    }
}
