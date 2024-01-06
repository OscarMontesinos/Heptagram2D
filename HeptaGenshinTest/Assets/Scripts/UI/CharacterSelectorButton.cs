using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectorButton : MonoBehaviour
{
    public TeamCard card;
    public PjData pj;
    public TextMeshProUGUI nameText;
    public Image image;

    private void Start()
    {
        nameText.text = pj.unit.name.ToUpper();

        switch (pj.unit.element)
        {
            case HitData.Element.ice:
                image.color = GameManager.Instance.iceColor;
                break;
            case HitData.Element.fire:
                image.color = GameManager.Instance.fireColor;
                break;
            case HitData.Element.water:
                image.color = GameManager.Instance.waterColor;
                break;
            case HitData.Element.desert:
                image.color = GameManager.Instance.desertColor;
                break;
            case HitData.Element.nature:
                image.color = GameManager.Instance.natureColor;
                break;
            case HitData.Element.wind:
                image.color = GameManager.Instance.windColor;
                break;
            case HitData.Element.lightning:
                image.color = GameManager.Instance.lightningColor;
                break;
        }
    }

    public void SelectPj()
    {
        card.SelectPj(pj);
    }
}
