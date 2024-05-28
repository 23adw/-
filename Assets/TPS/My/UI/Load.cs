using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Load : MonoBehaviour
{
    public CanvasGroup regame;
    public CanvasGroup loss;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StartCoroutine(StartShow());
    }
    IEnumerator StartShow()
    {
        yield return new WaitForSeconds(2f);
        regame.DOFade(1f, 1.5f).SetEase(Ease.InQuad);//µ≠»Î
        regame.transform.DOMoveY(Screen.height / 6, 1.5f).SetEase(Ease.OutBounce);
        loss.DOFade(1f, .75f).SetEase(Ease.InQuad);
        loss.transform.DOMoveX(Screen.width / 1.5f, 1.5f).SetEase(Ease.OutBounce);
    }
    public void ReGame()
    {
        SceneManager.LoadScene(0);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }
}
