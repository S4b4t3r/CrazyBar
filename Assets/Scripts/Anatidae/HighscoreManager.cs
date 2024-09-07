using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Linq;
using UnityEngine.Networking;
using System.Collections;
using System.Data.Common;

namespace Anatidae {

    public class HighscoreManager : MonoBehaviour
    {
        [Serializable]
        public struct HighscoreData
        {
            public List<HighscoreEntry> highscores;
        }

        [Serializable]
        public struct HighscoreEntry
        {
            public string name;
            public int score;
        }

        public static HighscoreManager Instance { get; private set; }
        public static List<HighscoreEntry> Highscores { get; private set;}
        public static bool HasFetchedHighscores { get; private set; }
        public static bool IsHighscoreInputScreenShown { get; private set; }
        public static string PlayerName;

        [SerializeField] HighscoreNameInput highscoreNameInput;
        [SerializeField] HighscoreUI highscoreUi;

        void Awake()
        {
            if (Instance == null){
                Instance = this;
            }
            else{
                Destroy(gameObject);
            }

            if (Instance.highscoreNameInput is null)
                Debug.LogError("HighscoreNameInput de HighscoreManager n'est pas défini.");
            else highscoreNameInput.gameObject.SetActive(false);

            if (Instance.highscoreUi is null)
                Debug.LogError("HighscoreUI de HighscoreManager n'est pas défini.");
            else highscoreUi.gameObject.SetActive(false);
        }

        [DllImport("__Internal")]
        public static extern void BackToMenu();

        float exitTimer = 0f;
        const float exitTime = 1f;
        void Update()
        {
            if (Input.GetButtonDown("Coin"))
                exitTimer += Time.deltaTime;
            else
                exitTimer = 0f;
            
            if (exitTimer >= exitTime)
                Application.Quit();

        }

        public static void ShowHighscores()
        {
            if (Instance.highscoreUi is null){
                Debug.LogError("HighscoreUI de HighscoreManager n'est pas défini.");
                return;
            }

            Instance.highscoreUi.gameObject.SetActive(true);
        }

        public static void HideHighscores()
        {
            if (Instance.highscoreUi is null){
                Debug.LogError("HighscoreUI de HighscoreManager n'est pas défini.");
                return;
            }

            Instance.highscoreUi.gameObject.SetActive(false);
        }
        
        public static void ShowHighscoreInput(int highscore)
        {
            if (Instance.highscoreNameInput is null){
                Debug.LogError("HighscoreNameInput de HighscoreManager n'est pas défini.");
                return;
            }

            Instance.highscoreNameInput.ShowHighscoreInput(highscore);
            IsHighscoreInputScreenShown = true;
        }

        public static void DisableHighscoreInput()
        {
            if (Instance.highscoreNameInput is null){
                Debug.LogError("HighscoreNameInput de HighscoreManager n'est pas défini.");
                return;
            }

            Instance.highscoreNameInput.gameObject.SetActive(false); 
            IsHighscoreInputScreenShown = false;
        }

        public static IEnumerator FetchHighscores()
        {
            UnityWebRequest request = UnityWebRequest.Get("http://localhost:3000/api/?game=" + "WrecklessBar");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                string data = request.downloadHandler.text;
                try {
                    Debug.Log(data);
                    HighscoreData highscoreData = JsonUtility.FromJson<HighscoreData>(data);
                    Debug.Log(highscoreData);
                    Debug.Log(highscoreData.highscores.Count);
                    Highscores = highscoreData.highscores;
                    HasFetchedHighscores = true;
                } catch (Exception e) {
                    Debug.LogError(e);
                }
            }
        }

        public static IEnumerator SetHighscore(string name, int score)
        {
            UnityWebRequest request = UnityWebRequest.Post(
                "http://localhost:3000/api/?game=" + "WrecklessBar",
                JsonUtility.ToJson(new { name, score })
            );
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                Debug.LogError(request.error);
        }

        public static bool IsHighscore(int score)
        {
            return IsHighscore(null, score);
        }
        
        public static bool IsHighscore(string name, int score)
        {
            if (name == null)
            {
                if (Highscores == null)
                    return false;
                if (Highscores.Count < 10)
                    return true;
                if (score > Highscores[Highscores.Count - 1].score)
                    return true;
            } else {
                HighscoreEntry? entry = Highscores.Find(entry => entry.name == name);
                if(entry?.score < score)
                    return true;
            }
            return false;
        }

        public void OnApplicationQuit()
        {
            BackToMenu();
        }
    }
}

