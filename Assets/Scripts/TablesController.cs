using System.Collections;
using UnityEngine;

public class TablesController : MonoBehaviour
{
    [SerializeField] float minTimeBeforeServe;
    [SerializeField] float maxTimeBeforeServe;
    float currentTimeOffset;
    float timeBeforeServe;
    [SerializeField] bool canServe;
    int numberOfItems;
    new Collider2D collider;
    [SerializeField] GameObject serveSprite;
    [SerializeField] Sprite[] stuffArray;
    [SerializeField] SpriteRenderer stuffRenderer; 
    [SerializeField] PlayerController playerController;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] UIController uIController;
    
    [SerializeField] AudioSource sfx;
    [SerializeField] AudioSource dishes;
    GameManager gameManager;
    
    float exponentialScore;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider2D>();
        SetNewTimeBeforeServe();
        collider.enabled = false; 
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!canServe && !gameManager.getIsGameOver() && Time.realtimeSinceStartup - currentTimeOffset > timeBeforeServe)
        {
            serveSprite.SetActive(true);
            canServe = true;
            collider.enabled = true;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            playerController.SetCanServe(true);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            playerController.SetCanServe(false);
        }
    }
    public void AddNumberOfItems(int amount)
    {
        sfx.Play();
        StartCoroutine(AddNumberOfItemsDelay(amount));
    }

    IEnumerator AddNumberOfItemsDelay(int amount)
    {
        yield return new WaitForSeconds(1.7f); // Wait for objects to come to table
        if (!gameManager.getIsGameOver())
        {
            numberOfItems += amount;

            // Show sprites on table
            Mathf.Clamp(numberOfItems/4, 0, 8);

            Sprite stuffToRender = stuffArray[Mathf.Clamp((numberOfItems+3)/6, 0, stuffArray.Length-1)];
            stuffRenderer.sprite = stuffToRender;
            exponentialScore = amount + Mathf.Pow((int)Mathf.Round(amount / 6) + 1, 2);
            scoreManager.AddToScore((int)exponentialScore, false); 
            dishes.Play();
            uIController.SpawnTicker(transform.position + new Vector3(0f,1f), (int)exponentialScore);

            SetNewTimeBeforeServe();
        }
    }

    void SetNewTimeBeforeServe()
    {
        serveSprite.SetActive(false);
        canServe = false;
        collider.enabled = false;
        timeBeforeServe = Random.Range(minTimeBeforeServe, maxTimeBeforeServe);
        currentTimeOffset = Time.realtimeSinceStartup;
    }
}
