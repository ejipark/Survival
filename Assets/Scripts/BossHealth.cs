using System;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public int maxHp = 2000;
    public float hp;
    public float health;

    public event Action<float> OnChange = delegate { };

    private void OnEnable()
    {
        hp = (float)maxHp;
    }

    public void UpdateHealth(float deltaHp)
    {
        hp += deltaHp;
        health = hp / (float)maxHp;
        OnChange(health);
    }
}
