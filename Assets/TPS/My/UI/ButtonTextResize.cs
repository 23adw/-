using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonTextResize : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform offlineButton;
    public RectTransform startButton;
    public RectTransform introButton;
    public RectTransform exitButton;
    public CanvasGroup startText;
    private bool buttonsShown = false;
    private RectTransform currentButton; // 用于记录当前按钮
    private AudioSource buttonAudioSource;
    public AudioClip enterSound;
    public AudioClip clickSound;
    void Awake()
    {
        StartCoroutine(BlinkStartText());
    }
    private void Update()
    {
        CheckForKeyPress();
    }
    private IEnumerator BlinkStartText()
    {
        while (!buttonsShown)
        {
            yield return new WaitForSeconds(2f);
            startText.DOFade(0f, 2f).SetEase(Ease.OutQuad);
            yield return new WaitForSeconds(2f);
          startText.DOFade(1f, 2f).SetEase(Ease.InQuad);
        }
    }
    void CheckForKeyPress()
    {
        if (Input.anyKeyDown && !buttonsShown)
        {
            //StopCoroutine(BlinkStartText());
            startText.DOKill();
            // 使用DoTween逐个显示按钮
            Sequence sequence = DOTween.Sequence();
            sequence.AppendCallback(() =>
            {
                ShowButtonWithAnimation(offlineButton, 0.5f);
            });
            sequence.AppendCallback(() =>
            {
                ShowButtonWithAnimation(startButton,1f);
            });
            sequence.AppendCallback(() =>
            {
                ShowButtonWithAnimation(introButton,1.5f);
            });
            sequence.AppendCallback(() =>
            {
                ShowButtonWithAnimation(exitButton,2f);
            });
            buttonsShown = true;
        }
    }
    void ShowButtonWithAnimation(RectTransform button,float timer)
    {
        button.DOAnchorPosX(-600f, timer).SetEase(Ease.OutBack);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        RectTransform buttonTransform = eventData.pointerEnter.GetComponent<RectTransform>();
            buttonTransform.DOScale(1.2f, 0.2f);
            currentButton = buttonTransform;
            buttonAudioSource = buttonTransform.GetComponentInParent<AudioSource>();
            if (buttonAudioSource.clip != enterSound)
            {
                buttonAudioSource.clip = enterSound;
            }
            buttonAudioSource.Play();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        RectTransform buttonTransform = eventData.pointerEnter.GetComponent<RectTransform>();
            buttonTransform.DOScale(1f, 0.2f);
    }
    public void ReNewScale()
    {
        if (currentButton != null)
        {
            // 复原当前按钮
            currentButton.DOScale(1f, 0.2f);
            currentButton = null; // 重置当前按钮
            buttonAudioSource.clip = clickSound;
            buttonAudioSource.Play();
        }
    }
    public void StartGame()
    {
        //SceneManager.LoadScene("Pun2");
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);
    }
    public void Offline()
    {
        SceneManager.LoadScene(5);
    }
    public void IntroGame()
    {
        ShowIntroUI();
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public GameObject SetUI;
    private void ShowIntroUI()
    {
        SetUI.SetActive(true);
        gameObject.SetActive(false);
    }
}