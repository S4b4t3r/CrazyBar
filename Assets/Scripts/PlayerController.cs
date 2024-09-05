using UnityEngine;

public class PlayerController : MonoBehaviour
{
    HingeJoint2D hand;
    new Rigidbody2D rigidbody2D;
    float inputHorizontal = 0f;
    // float inputVertical = 0f;
    bool freezeMovement = false;
    [SerializeField] float normalSpeed = 10f;
    [SerializeField] float sprintSpeed = 20f;
    [SerializeField] float sprintReloadRate = 30f;
    [SerializeField] float sprintUsageRate = 100f;
    [SerializeField] AudioSource fail;
    float speedMultiplier;
    [SerializeField] GameObject staminaBar;
    [SerializeField] Transform staminaInsideBar;
    float stamina = 100f;
    bool canServe = false;

    [SerializeField] GameObject spriteHolder;
    [SerializeField] TablesController tablesController;

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        hand = GetComponent<HingeJoint2D>();
        speedMultiplier = normalSpeed;
    }

    void Update()
    {
        staminaBar.gameObject.SetActive(stamina != 100f);
    }

    // Update is called once per frame
    void FixedUpdate()
    { 
        if (freezeMovement) 
        {
            inputHorizontal = 0f;
        } else {

            inputHorizontal = Input.GetAxisRaw("P1_Horizontal");
            
            // Sprint System
            staminaInsideBar.localScale = new Vector3(stamina/100f, 1f, 1f);

            if(Input.GetButton("P1_B1"))
            {
                if(stamina > 0f) {
                    speedMultiplier = sprintSpeed;
                    if (inputHorizontal != 0)
                        stamina = stamina - sprintUsageRate * Time.deltaTime;
                } else {
                    speedMultiplier = normalSpeed;
                    stamina = 0f;
                }
            } else {
                speedMultiplier = normalSpeed;
                if(stamina < 100f) {
                    stamina = stamina + sprintReloadRate * Time.deltaTime;
                } else {
                    stamina = 100f;
                }
            }

            // Serve Input
            if(canServe)
            {
                if (Input.GetButton("P1_B2"))
                {
                    // Move objects to Table
                    GameObject[] objects;
                    int objectsOnPlateauCount = 0;
                    objects = GameObject.FindGameObjectsWithTag("Object");
                    
                    foreach(GameObject obj in objects)
                    {
                        Object objectScript = obj.GetComponent<Object>();
                        if (objectScript.isOnPlateau)
                        {
                            objectScript.FlyToTable();
                            objectsOnPlateauCount++;
                        }
                    }

                    tablesController.AddNumberOfItems(objectsOnPlateauCount);

                    canServe = false;
                }
            }

            // Movement Animation
            if(inputHorizontal > 0f) {
                spriteHolder.GetComponent<SpriteRenderer>().flipX = false;
            } else if(inputHorizontal < 0f) {
                spriteHolder.GetComponent<SpriteRenderer>().flipX = true;
            }

            if(inputHorizontal != 0f) {
                spriteHolder.GetComponent<Animator>().SetBool("Walking", true);
            } else {
                spriteHolder.GetComponent<Animator>().SetBool("Walking", false);
            }

            // Movement Rigidbody
            rigidbody2D.MovePosition(
                new Vector2(
                    rigidbody2D.position.x + inputHorizontal*Time.deltaTime*speedMultiplier,
                    rigidbody2D.position.y
                )
            );
        }
    }

    public void GameOver()
    {
        freezeMovement = true;
        fail.Play();
        Destroy(hand);
        spriteHolder.GetComponent<Animator>().SetTrigger("Failed");
    }

    public void SetCanServe(bool value)
    {
        canServe = value;
    }
}
