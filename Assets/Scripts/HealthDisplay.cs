using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    public Image[] heartImages; 
    public Sprite fullHeartSprite; 
    public Sprite emptyHeartSprite; 

    private int currentHealth; 
    internal int maxHealth;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthDisplay();
    }

    public void TakeDamage()
    {
        if (currentHealth > 0)
        {
            currentHealth--; 
            UpdateHealthDisplay();
        }
    }

    public void Kill()
    {
        currentHealth = 0;
        UpdateHealthDisplay();
    }

    private void UpdateHealthDisplay()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < currentHealth)
            {
                heartImages[i].sprite = fullHeartSprite;
            }
            else
            {
                heartImages[i].sprite = emptyHeartSprite; 
            }
        }
    }
}
