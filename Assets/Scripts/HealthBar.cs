using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Image img;
    [SerializeField]
    private float animSpeed = 2f;
    public Transform lookPoint;
    [SerializeField]
    private Gradient gradient;
    //int hp = 100;
    //float maxHp;

    private Coroutine animCoroutine;

    private void Awake()
    {
        GetComponentInParent<BossHealth>().OnChange += BarUpdate;
    }

    public void BarUpdate(float deltaHp)
    {
        // Debug.Log("deltaHp: " + deltaHp);
        if (deltaHp < 0 || deltaHp > 1)
        {
            deltaHp = Mathf.Clamp01(deltaHp);
        }
        if (deltaHp != img.fillAmount)
        {
            if (animCoroutine != null)
            {
                StopCoroutine(animCoroutine);
            }
            animCoroutine = StartCoroutine(healthUpdate(deltaHp, animSpeed));
        }
    }

    private IEnumerator healthUpdate(float deltaHp, float speed)
    {
        float time = 0f;
        float currentHp = img.fillAmount;
        
        while (time < 1)
        {
            img.fillAmount = Mathf.Lerp(currentHp, deltaHp, time);
            img.color = gradient.Evaluate(img.fillAmount);
            time += Time.deltaTime * speed;
            yield return null;
        }

        img.fillAmount = deltaHp;
        img.color = gradient.Evaluate(img.fillAmount);
    }

    private void LateUpdate()
    {
        // transform.LookAt(Camera.main.transform);
        transform.LookAt(lookPoint);
        transform.Rotate(0, 180, 0);
    }
}
