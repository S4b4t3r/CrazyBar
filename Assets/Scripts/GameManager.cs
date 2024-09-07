using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class GameManager : MonoBehaviour
{
    int retryCount = -1;
    [SerializeField] ObjectSpawner objectSpawner;
    [SerializeField] PlateauController plateauController;
    [SerializeField] PlayerController playerController;
    [SerializeField] UIController uIController;
    [SerializeField] AudioManager audioManager;
    [SerializeField] ScoreManager scoreManager;
    bool isGameOver = false;
    float sceneLoadedTime;
    float gameOverTime;
    int hiScore;

    [DllImport("__Internal")]
    private static extern void BackToMenu();
    
    // called first
    void OnEnable()
    {
        // Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        retryCount++;
        sceneLoadedTime = Time.realtimeSinceStartup;

        objectSpawner = GameObject.FindGameObjectWithTag("ObjectSpawner").GetComponent<ObjectSpawner>();
        plateauController = GameObject.FindGameObjectWithTag("Plateau").GetComponent<PlateauController>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        uIController = GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>();
        audioManager = GameObject.FindGameObjectWithTag("Jukebox").GetComponent<AudioManager>();
        scoreManager = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager>();
        isGameOver = false;

        if (retryCount == 0)
        {
            uIController.ShowTutorial();
        }
    }

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameController");

        if (objs.Length > 1)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        Anatidae.HighscoreManager.FetchHighscores();
    }

    float pitch;
    void Update()
    {
        if (!isGameOver)
        {
            pitch = (Time.realtimeSinceStartup-sceneLoadedTime)/200f+0.9f;
            audioManager.SetMusicPitch(Mathf.Max(1f,pitch));
        } else {
            audioManager.SetMusicPitch(Mathf.Max(1f,pitch - (Time.realtimeSinceStartup-gameOverTime/10f)));
        }
    }

    public void GameOver()
    {
        if (!isGameOver)
        {            
            objectSpawner.StopSpawning();
            playerController.GameOver();
            plateauController.Fall();

            // Explode objects on plateau
            GameObject[] objects;
            objects = GameObject.FindGameObjectsWithTag("Object");
            
            foreach(GameObject obj in objects)
            {
                Destroy(obj.GetComponent<FixedJoint2D>());

                obj.GetComponent<Rigidbody2D>().AddExplosionForce(
                    3f,
                    plateauController.transform.position+Vector3.down*2f,
                    10f
                );
            }

            isGameOver = true;
            gameOverTime = Time.realtimeSinceStartup;

            int score = scoreManager.GetScore();
            bool isHighScore = false;
            if (score > hiScore)
            {
                hiScore = score;
                isHighScore = true;
            }

            uIController.ShowPopup(score, hiScore, isHighScore);
        }
    }

    public bool getIsGameOver()
    {
        return isGameOver;
    }

    public void OnApplicationQuit()
    {
        BackToMenu();
    }
}
