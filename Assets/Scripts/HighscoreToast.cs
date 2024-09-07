using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighscoreToast : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    Animation animation;
    public string Name {get; set;}
    bool showAtUpdateBecauseAsyncBs;

    void Start()
    {
        animation = GetComponent<Animation>();
    }

    public void ShowPopup()
    {
        showAtUpdateBecauseAsyncBs = true;
    }

    void Update()
    {
        if (showAtUpdateBecauseAsyncBs){
            text.text = $"Record enregistre sur {Name} !";
            animation.Play("Highscore_Popup");
            showAtUpdateBecauseAsyncBs = false;
        }
    }
}
