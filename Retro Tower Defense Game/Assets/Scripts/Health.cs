using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHP;
    private int currentHP;


    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if(currentHP <= 0)
        {
            Debug.Log("You are dead!?");
        }
    }
}
