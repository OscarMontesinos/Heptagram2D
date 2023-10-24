using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
