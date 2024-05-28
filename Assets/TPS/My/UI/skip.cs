using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class skip : MonoBehaviour
{
    public VideoPlayer W;
    public CanvasGroup regame;
    private bool ispress=false;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown&&!ispress)
        {
            //W.Stop();
            regame.DOFade(1f, 1.5f).SetEase(Ease.InQuad);//µ≠»Î
            regame.transform.DOMoveY(Screen.height/6, 1.5f).SetEase(Ease.OutBounce);
            ispress=true;
        }
    }
    public void ReGame()
    {
        SceneManager.LoadScene(0);
    }


}
