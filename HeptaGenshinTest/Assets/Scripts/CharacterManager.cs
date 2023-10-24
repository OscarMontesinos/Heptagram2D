using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;
    public List<CharacterData> data = new List<CharacterData>();
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (PlayerPrefs.HasKey("FirstSafe"))
        {
            UploadInfo();
        }
        else
        {
            PlayerPrefs.SetInt("FirstSafe", 0);
            SafeInfo();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SafeInfo();
        }
    }

    void UploadInfo()
    {
        foreach (CharacterData data in data)
        {
            if( PlayerPrefs.GetInt(data.name + "Unlocked") == 0)
            {
                data.unlocked = false;
            }
            else
            {
                data.unlocked = true;
                data.level = PlayerPrefs.GetInt(data.name + "Level");
                data.convergence = PlayerPrefs.GetInt(data.name + "Convergence");
            }
        }
    }
    void SafeInfo()
    {
        foreach (CharacterData data in data)
        {
            if(!data.unlocked)
            {
                PlayerPrefs.SetInt(data.name + "Unlocked", 0);
            }
            else
            {
                PlayerPrefs.SetInt(data.name + "Unlocked", 1);
            }
            PlayerPrefs.SetInt(data.name + "Level",data.level);
            PlayerPrefs.SetInt(data.name + "Convergence",data.convergence);
        }
    }
}
