using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    // Private values
    Image healthbarFill;
    private float flashCd = .15f;
    private float lastFlash;
    private int flashThreshold = 50;
    private Color red = new Color32(255,0,0,255);
    private Color white = new Color32(255,255,255,255);
    private Color green = new Color32(6,248,36,255);

    public static UIScript occurrence;

    [Header("Health bar")]
    public Slider healthBar;
    public Text healthNumMain;
    public Text healthNumSub;

    [Header("Ammo tracker")]
    public GameObject ammoCounter;
    public Text ammoTotalNumMain;
    public Text ammoTotalNumSub;
    public Text ammoCurrNumMain;
    public Text ammoCurrNumSub;

    [Header("Gun tracker")]
    public GameObject playerGun;
    public GameObject gunCounter;

    [Header("Mag tracker")]
    public GameObject magCounter;
    public Text magNumMain;
    public Text magNumSub;

    [Header("Supply tracker")]
    public GameObject supplyCounter;
    public Text supplyNumMain;
    public Text supplyNumSub;

    private void Awake()
    {
        occurrence = this;
    }

    void Start()
    {
        healthbarFill = GameObject.Find("PlayerUI/HealthBar/HealthBarFiller").GetComponent<Image>();
    }

    void FlashHealthBar(){
        if (healthbarFill.color.g == 1){
            healthbarFill.color = red;
            
        } else {
            healthbarFill.color = white;
        }
    }

    void FixedUpdate(){
        if (healthBar.value <= flashThreshold){
            if (lastFlash > flashCd){
                FlashHealthBar();
                lastFlash = 0;
            } else {
            lastFlash += Time.deltaTime;
            }
        } else {
            healthbarFill.color = green;
        }
    }

    void Update()
    {
        if (playerGun.activeSelf == true)
        {
            ammoCounter.SetActive(true);
            magCounter.SetActive(true);
            gunCounter.SetActive(true);

        }
        else
        {
            ammoCounter.SetActive(false);
            magCounter.SetActive(false);
            gunCounter.SetActive(false);
        }
    }

    public void SetFullHealth(float health)
    {
        // Slider
        healthBar.maxValue = health;
        healthBar.value = health;

        // Text
        healthNumMain.text = NormalizeHealthText(health);
        healthNumSub.text = NormalizeHealthText(health);
    }

    public void UpdateHealth(float health)
    {
        // Slider
        healthBar.value = health;

        // Text
        healthNumMain.text = NormalizeHealthText(health);
        healthNumSub.text = NormalizeHealthText(health);
    }

    public void SetFullAmmo(int ammo)
    {
        ammoTotalNumMain.text = ammo.ToString();
        ammoTotalNumSub.text = ammo.ToString();
        ammoCurrNumMain.text = ammo.ToString();
        ammoCurrNumSub.text = ammo.ToString();
    }

    public void UpdateAmmo(int ammo)
    {
        ammoCurrNumMain.text = ammo.ToString();
        ammoCurrNumSub.text = ammo.ToString();
    }

    public void UpdateMag(int mag)
    {
        magNumMain.text = mag.ToString();
        magNumSub.text = mag.ToString();
    }

    public void UpdateSupply(int supply)
    {
        supplyNumMain.text = supply.ToString();
        supplyNumSub.text = supply.ToString();
    }

    private string NormalizeHealthText(float health)
    {
        if (health <= 0)
        {
            return 0.ToString();
        }
        else
        {
            return Mathf.RoundToInt(health).ToString();
        }
    }
}
