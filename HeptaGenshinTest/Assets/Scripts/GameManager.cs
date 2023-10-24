using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject character1;
    public GameObject character2;
    public GameObject character3;

    int charaterDisplayed;

    public List<PjBase> pjList = new List<PjBase>();

    public LayerMask wallLayer;
    public LayerMask enemyLayer;



    public GameObject damageText;

    public Color32 iceColor;
    public Color32 fireColor;
    public Color32 waterColor;
    public Color32 desertColor;
    public Color32 natureColor;
    public Color32 windColor;
    public Color32 lightningColor;
    // Start is called before the first frame update
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


         character1 = Instantiate(character1, transform.position, transform.rotation);
        PlayerController.Instance.team.Add(character1.GetComponent<PjBase>());
        if (character2 != null)
        {
            character2 = Instantiate(character2, transform.position, transform.rotation);
            PlayerController.Instance.team.Add(character2.GetComponent<PjBase>());
        }

        if (character3 != null)
        {
            character3 = Instantiate(character3, transform.position, transform.rotation);
            PlayerController.Instance.team.Add(character3.GetComponent<PjBase>());
        }

        DisplayCharacter(1);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DisplayCharacter(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DisplayCharacter(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            DisplayCharacter(3);
        }
        foreach (PjBase pj in PlayerController.Instance.team)
        {
            if (!pj.isActive)
            {
                pj.transform.position = PlayerController.Instance.character.transform.position;
            }
        }
    }

    void DisplayCharacter(int character)
    {
        switch (character)
        {
            case 1:
                if (PlayerController.Instance.character != character1)
                {
                    character1.GetComponent<PjBase>().Activate(true);
                    if (character2 != null)
                    {
                        character2.GetComponent<PjBase>().Activate(false);
                    }
                    if (character3 != null)
                    {
                        character3.GetComponent<PjBase>().Activate(false);
                    }
                    PlayerController.Instance.character = character1.GetComponent<PjBase>();
                    PlayerController.Instance.rb = character1.GetComponent<Rigidbody2D>();
                }
                break;
            case 2:
                if (PlayerController.Instance.character != character1)
                {
                    character1.GetComponent<PjBase>().Activate(false);
                    character2.GetComponent<PjBase>().Activate(true);
                    if (character3 != null)
                    {
                        character3.GetComponent<PjBase>().Activate(false);
                    }
                    PlayerController.Instance.rb = character2.GetComponent<Rigidbody2D>().GetComponent<Rigidbody2D>();
                    PlayerController.Instance.character = character2.GetComponent<PjBase>();
                }
                break;
            case 3:
                if (PlayerController.Instance.character != character1)
                {
                    character1.GetComponent<PjBase>().Activate(false); 
                    if (character2 != null)
                    {
                        character2.GetComponent<PjBase>().Activate(false);
                    }
                    character3.GetComponent<PjBase>().Activate(true);
                    PlayerController.Instance.rb = character3.GetComponent<Rigidbody2D>().GetComponent<Rigidbody2D>();
                    PlayerController.Instance.character = character3.GetComponent<PjBase>();
                }
                break;
        }
    }
}
