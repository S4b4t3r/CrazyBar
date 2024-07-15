using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class UIController : MonoBehaviour
{
    [SerializeField] GameObject retryPrompt;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] GameObject tutorialPrompt;
    [SerializeField] Animation blackFadeAnim;
    [SerializeField] HighscoreToast highscoreToast;
    [SerializeField] GameObject tickerPrefab;
    public bool PopupShown = false;

    void Awake() {
        PopupShown = false;
        blackFadeAnim.Play("FadeIn");
        switch (SceneManager.GetActiveScene().name)
        {
            case "MainMenuScene":
                Anatidae.HighscoreManager.ShowHighscores();
                break;
            case "Game":
                retryPrompt.SetActive(false);
                tutorialPrompt.SetActive(false);
                break;

        }
    }

    void Update()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Game":
                if (PopupShown && !Anatidae.HighscoreManager.IsHighscoreInputScreenShown){
                    if (Input.GetButtonDown("P1_B1")){
                        RetryBtn();
                        PopupShown = false;
                    }
                }
                break;
            case "MainMenuScene":
                if (Input.GetButtonDown("P1_B1"))
                    RetryBtn();
                break;
        }
    }

    public void ShowPopup(int score, int hiScore, bool isHighscore)
    {
        if (isHighscore && Anatidae.HighscoreManager.IsHighscore(score))
        {
            if (Anatidae.HighscoreManager.PlayerName == null) {
                Anatidae.HighscoreManager.ShowHighscoreInput(score);
            }
            else {
                Anatidae.HighscoreManager.SetHighscore(Anatidae.HighscoreManager.PlayerName, score).ContinueWith(task => {
                    if (task.IsFaulted)
                        Debug.LogError(task.Exception);
                    else {
                        highscoreToast.Name = Anatidae.HighscoreManager.PlayerName;
                        highscoreToast.ShowPopup();
                    }
                });
            }
        }

        scoreText.text = "Score: " + score.ToString() + "\n" + "HiScore: " + hiScore.ToString();

        retryPrompt.SetActive(true);
        StartCoroutine(RetryBtnDebounce());
    }

    public void SpawnTicker(Vector3 worldSpacePosition, int value)
    {
        Vector3 uiSpacePosition = new Vector3(worldSpacePosition.x * 54f, worldSpacePosition.y * 54f, 0);
        Ticker ticker = Instantiate(tickerPrefab, gameObject.transform).GetComponent<Ticker>();
        ticker.SetPosition(uiSpacePosition);
        ticker.SetValue(value);
    }

    public void ShowTutorial()
    {
        tutorialPrompt.SetActive(true);
    }

    public void RetryBtn()
    {
        blackFadeAnim.Play("FadeOut");
        StartCoroutine(RestartLevelCoroutine());
    }

    IEnumerator RetryBtnDebounce()
    {
        yield return new WaitForSeconds(1.5f);
        PopupShown = true;
    }

    IEnumerator RestartLevelCoroutine()
    {
        yield return new WaitForSeconds(0.6f);
        LoadLevel("Game");
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
}
