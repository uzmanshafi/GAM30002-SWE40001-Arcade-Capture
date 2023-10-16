using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    private Slider health_slider;

    private void Awake()
    {
        health_slider = GetComponent<Slider>();
    }

    public void UpdateHealthBar(Enemy enemy)
    {
        float currentHealth = enemy.GetCurrentHealth(); // Assuming you have this function in the Enemy class
        float maxHealth = enemy.GetMaxHealth(); // Assuming you have this function in the Enemy class
        
        health_slider.value = currentHealth / maxHealth;
    }
}
