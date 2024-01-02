using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class PlayerController : MonoBehaviour
{   
    public static PlayerController Instance;
    [HideInInspector]
    public Vector2 inputMov;
    public Camera cam;
    public LayerMask wallLayer;
    float speedDecrease = 5;
    [HideInInspector]
    public Rigidbody2D rb;
    public GameObject pointer;
    public PjBase character;
    public PjBase backCharacter1;
    public PjBase backCharacter2;
    bool lockPointer;
    public GameObject targetBoss;
    float maxViewportDistance = 16;
    public List<PjBase> team = new List<PjBase>();

    public void LockPointer(bool value)
    {
        lockPointer = value;
    }

    void Awake()
    {
        if (Instance == null || Instance == this)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(cam != Camera.main)
        {
            cam = Camera.main;
        }

        if (character != null)
        {
            transform.position = character.transform.position;


            if (character.stunTime <= 0)
            {
                HandlePointer();

                HandleHabilities();

                HandleMovement();
            }

            HandleCamera();
        }



        //if (unit.pointer != null) { unit.pointer.transform.position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z - cam.transform.position.z)); }




        /*foreach (Unit unit_ in unit.manager.units)
        {
            if (unit_ != null)
            {
                if (unit_.team != unit.team)
                {
                    var dir = unit_.transform.position - transform.position;
                    if (!Physics2D.Raycast(transform.position, dir, dir.magnitude, wallLayer))
                    {
                        unit_.oculto = false;
                    }
                    else
                    {
                        unit_.oculto = true;
                    }
                }
            }
        }*/


        /*if (unit.aim)
        {
            transform.GetChild(0).up = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z - cam.transform.position.z)) - transform.position;
        }
        transform.GetChild(0).eulerAngles = new Vector3(0, 0, transform.GetChild(0).eulerAngles.z);
        unit.pointer.transform.position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z - cam.transform.position.z));*/
    }


    public virtual void FixedUpdate()
    {
        if (character != null)
        {
            if (!character.dashing)
            {
                if (!character.casting && character.stunTime <= 0)
                {
                    if (!character.softCasting)
                    {
                        rb.velocity = transform.right * character.stats.spd * inputMov.x + transform.up * character.stats.spd * inputMov.y;
                    }
                    else
                    {
                        rb.velocity = transform.right * (character.stats.spd / 1.5f) * inputMov.x + transform.up * (character.stats.spd / 1.5f) * inputMov.y;
                    }
                }
                else
                {
                    rb.velocity = Vector3.zero;
                }
            }
        }

    }

    void HandlePointer()
    {
        if (!lockPointer)
        {
            Vector2 dir = UtilsClass.GetMouseWorldPosition() - pointer.transform.position;
            pointer.transform.up = dir;
        }
    }
    void HandleCamera()
    {
        cam.transform.position = new Vector3(transform.position.x, transform.position.y, cam.transform.position.z);
        if (targetBoss == null)
        {
            Camera.main.orthographicSize = maxViewportDistance;
        }
        else
        {
            Vector3 dist = targetBoss.transform.position - character.transform.position;
            dist = character.transform.position + (dist * 0.5f);
            cam.transform.position = new Vector3(dist.x, dist.y , cam.transform.position.z);

            dist = targetBoss.transform.position - character.transform.position;
            if (dist.magnitude > maxViewportDistance)
            {
                Camera.main.orthographicSize = dist.magnitude;
            }
        }
    }

    void HandleHabilities()
    {
        if (Input.GetMouseButton(0))
        {
            character.MainAttack();
        }
        if (Input.GetMouseButton(1))
        {
            character.StrongAttack();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            character.Hab1();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            character.Hab2();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            character.BasicDash();
        }
    }

    void HandleMovement()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            inputMov.x = Input.GetAxisRaw("Horizontal");
        }
        else if (inputMov.x != 0)
        {
            if (inputMov.x <= 0.2f && inputMov.x >= -0.2f /*|| unit.casting*/)
            {
                inputMov.x = 0;
            }
            if (inputMov.x != 0 && inputMov.x > 0)
            {
                inputMov.x -= speedDecrease * Time.deltaTime;
            }
            if (inputMov.x != 0 && inputMov.x < 0)
            {
                inputMov.x += speedDecrease * Time.deltaTime;
            }
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            inputMov.y = Input.GetAxisRaw("Vertical");
        }
        else if (inputMov.y != 0)
        {
            if (inputMov.y <= 0.2f && inputMov.y >= -0.2f /*|| unit.casting*/)
            {
                inputMov.y = 0;
            }
            if (inputMov.y != 0 && inputMov.y > 0)
            {
                inputMov.y -= speedDecrease * Time.deltaTime;
            }
            if (inputMov.y != 0 && inputMov.y < 0)
            {
                inputMov.y += speedDecrease * Time.deltaTime;
            }
        }

    }
}
