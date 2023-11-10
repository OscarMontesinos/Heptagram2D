using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Slider hpSlider;
    public TextMeshProUGUI hpText;
    public Slider hp2Slider;
    public TextMeshProUGUI hp2Text;
    public Slider hp3Slider;
    public TextMeshProUGUI hp3Text;
    public Slider shieldSlider;
    public TextMeshProUGUI shieldText;
    public Slider stunSlider;
    public List<HabilityUIIndicator> habIndicators = new List<HabilityUIIndicator>();

    public GameObject pauseMenu;

    public PjBase ch1;
    public PjBase ch2;
    public PjBase ch3;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {

    }
    private void Update()
    {
        UpdateHpBars();

        UpdateHabIndicators();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(Time.timeScale == 0)
            {
                Time.timeScale = 1;
                pauseMenu.SetActive(false);
            }
            else
            {
                Time.timeScale = 0;
                pauseMenu.SetActive(true);
            }
        }
    }

    void UpdateHpBars()
    {
        hpSlider.value = PlayerController.Instance.character.stats.hp;
        hpSlider.maxValue = PlayerController.Instance.character.stats.mHp;
        hpText.text = PlayerController.Instance.character.stats.hp.ToString("F0");
        
        hp2Slider.value = PlayerController.Instance.backCharacter1.stats.hp;
        hp2Slider.maxValue = PlayerController.Instance.backCharacter1.stats.mHp;
        hp2Text.text = PlayerController.Instance.backCharacter1.stats.hp.ToString("F0");
        
        hp3Slider.value = PlayerController.Instance.backCharacter2.stats.hp;
        hp3Slider.maxValue = PlayerController.Instance.backCharacter2.stats.mHp;
        hp3Text.text = PlayerController.Instance.backCharacter2.stats.hp.ToString("F0");

        PlayerController.Instance.character.stunnBar = stunSlider;
        PlayerController.Instance.backCharacter1.stunnBar = stunSlider;
        PlayerController.Instance.backCharacter2.stunnBar = stunSlider;

        shieldSlider.value = Shield.shieldAmount;
        shieldSlider.maxValue = PlayerController.Instance.character.stats.mHp*1.5f;
        if (Shield.shieldAmount > 0)
        {
            shieldText.text = Shield.shieldAmount.ToString("F0");
        }
        else
        {
            shieldText.text = "";
        }
    }


    public void UpdateHabIndicatorsImages()
    {
        habIndicators[0].UpdateImage(ch1.hab1Image);
        habIndicators[1].UpdateImage(ch1.hab2Image);
        habIndicators[2].UpdateImage(ch2.hab1Image);
        habIndicators[3].UpdateImage(ch2.hab2Image);
        habIndicators[4].UpdateImage(ch3.hab1Image);
        habIndicators[5].UpdateImage(ch3.hab2Image);
    }


    void UpdateHabIndicators()
    {
        if (ch1 != null && ch1.stats.hp > 0)
        {
            habIndicators[0].Activate(true);
            habIndicators[1].Activate(true);
            habIndicators[0].UIUpdate(ch1.hab1Cd, ch1.currentHab1Cd, ch1.isActive);
            habIndicators[1].UIUpdate(ch1.hab2Cd, ch1.currentHab2Cd, ch1.isActive);
        }
        else
        {
            habIndicators[0].Activate(false);
            habIndicators[1].Activate(false);
        }

        if (ch2 != null && ch2.stats.hp > 0)
        {
            habIndicators[2].Activate(true);
            habIndicators[3].Activate(true);
            habIndicators[2].UIUpdate(ch2.hab1Cd, ch2.currentHab1Cd, ch2.isActive);
            habIndicators[3].UIUpdate(ch2.hab2Cd, ch2.currentHab2Cd, ch2.isActive);
        }
        else
        {
            habIndicators[2].Activate(false);
            habIndicators[3].Activate(false);
        }

        if (ch3 != null && ch3.stats.hp > 0)
        {
            habIndicators[4].Activate(true);
            habIndicators[5].Activate(true);
            habIndicators[4].UIUpdate(ch3.hab1Cd, ch3.currentHab1Cd, ch3.isActive);
            habIndicators[5].UIUpdate(ch3.hab2Cd, ch3.currentHab2Cd, ch3.isActive);
        }
        else
        {
            habIndicators[4].Activate(false);
            habIndicators[5].Activate(false);
        }
    }
}
