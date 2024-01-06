using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeamCard : MonoBehaviour
{
    public int cardNumber;
    public PjData selectedPj;

    public GameObject cardGO;
    public GameObject addGO;
    public GameObject pjsGO;
    public GameObject buttonGO;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI convText;
    public Image convImage;

    public TextMeshProUGUI hpText;
    public TextMeshProUGUI sinText;
    public TextMeshProUGUI conText;
    public TextMeshProUGUI atSpdText;
    public TextMeshProUGUI cdrText;
    public TextMeshProUGUI spdText;
    public TextMeshProUGUI iceRText;
    public TextMeshProUGUI fireRText;
    public TextMeshProUGUI waterRText;
    public TextMeshProUGUI desertRText;
    public TextMeshProUGUI natureRText;
    public TextMeshProUGUI windRText;
    public TextMeshProUGUI lightningRText;
    // Start is called before the first frame update
    void Start()
    {
        switch (cardNumber)
        {
            case 1:
                selectedPj.unitObject = GameManager.Instance.character1;
                if (selectedPj.unitObject != null)
                {
                    selectedPj.unit = GameManager.Instance.character1.GetComponent<PjBase>();
                }
                break;
            case 2:
                selectedPj.unitObject = GameManager.Instance.character2;
                if (selectedPj.unitObject != null)
                {
                    selectedPj.unit = GameManager.Instance.character2.GetComponent<PjBase>();
                }
                break;
            case 3:
                selectedPj.unitObject = GameManager.Instance.character3;
                if (selectedPj.unitObject != null)
                {
                    selectedPj.unit = GameManager.Instance.character3.GetComponent<PjBase>();
                }
                break;
        }

        UpdateCard();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCard();
    }

    public void Selector(bool value)
    {
        pjsGO.SetActive(value);
        buttonGO.SetActive(!value);
    }

    public void SelectPj(PjData data)
    {
        selectedPj = data;

        Selector(false);
        UpdateCard();
        ChangePj();
    }

    void UpdateCard()
    {
        if (selectedPj.unit != null)
        {
            cardGO.SetActive(true);
            addGO.SetActive(false);

            nameText.text = selectedPj.unit.name.ToUpper();
            convText.text = CharacterManager.Instance.data[selectedPj.unit.id].convergence.ToString("F0");

            hpText.text = (selectedPj.unit.stats.mHp + (selectedPj.unit.statsPerLevel.mHp * (CharacterManager.Instance.data[selectedPj.unit.id].level - 1))).ToString("F0");
            sinText.text = (selectedPj.unit.stats.sinergy + (selectedPj.unit.statsPerLevel.sinergy * (CharacterManager.Instance.data[selectedPj.unit.id].level - 1))).ToString("F0");
            conText.text = (selectedPj.unit.stats.control + (selectedPj.unit.statsPerLevel.control * (CharacterManager.Instance.data[selectedPj.unit.id].level - 1))).ToString("F0");
            atSpdText.text = selectedPj.unit.stats.atSpd.ToString("F1");
            cdrText.text = selectedPj.unit.stats.cdr.ToString("F0");
            spdText.text = selectedPj.unit.stats.spd.ToString("F0");
            iceRText.text = selectedPj.unit.stats.iceResist.ToString("F0");
            fireRText.text = selectedPj.unit.stats.fireResist.ToString("F0");
            waterRText.text = selectedPj.unit.stats.waterResist.ToString("F0");
            desertRText.text = selectedPj.unit.stats.desertResist.ToString("F0");
            natureRText.text = selectedPj.unit.stats.natureResist.ToString("F0");
            windRText.text = selectedPj.unit.stats.windResist.ToString("F0");
            lightningRText.text = selectedPj.unit.stats.lightningResist.ToString("F0");

            switch (selectedPj.unit.element)
            {
                case HitData.Element.ice:
                    convImage.sprite = GameManager.Instance.iceCrystal;
                    break;
                case HitData.Element.fire:
                    convImage.sprite = GameManager.Instance.fireCrystal;
                    break;
                case HitData.Element.water:
                    convImage.sprite = GameManager.Instance.waterCrystal;
                    break;
                case HitData.Element.desert:
                    convImage.sprite = GameManager.Instance.desertCrystal;
                    break;
                case HitData.Element.nature:
                    convImage.sprite = GameManager.Instance.natureCrystal;
                    break;
                case HitData.Element.wind:
                    convImage.sprite = GameManager.Instance.windCrystal;
                    break;
                case HitData.Element.lightning:
                    convImage.sprite = GameManager.Instance.lightningCrystal;
                    break;
            }
        }
        else
        {
            cardGO.SetActive(false);
            addGO.SetActive(true);
        }
    }

    void ChangePj()
    {
        switch (cardNumber)
        {
            case 1:
                GameManager.Instance.character1 = selectedPj.unitObject;
                break;
            case 2:
                GameManager.Instance.character2 = selectedPj.unitObject;
                break;
            case 3:
                GameManager.Instance.character3 = selectedPj.unitObject;
                break;
        }
    }
}
