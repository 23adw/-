using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonPause : MonoBehaviour
{
    public GameObject ingameMenu;

    void Update()
    {
        // ���� ESC ����ͣ��Ϸ
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        // �л���ͣ�˵�����ʾ������
        bool isPaused = Time.timeScale == 0;
        ingameMenu.SetActive(!isPaused);
        RectTransform buttonTransform = ingameMenu.GetComponent<RectTransform>();
        // �����ͣ�˵���ʾ������ͣ��Ϸ������������
        if (!isPaused)
        {
           // buttonTransform.DOScale(1,0.5f);
            //StartCoroutine(WaitScale(0,CursorLockMode.None,true));
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            //buttonTransform.DOScale(0.5f, 0.5f);
            //StartCoroutine(WaitScale(0, CursorLockMode.Locked, false));
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    public IEnumerator WaitScale(float value,CursorLockMode mode,bool visible)
    {
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = value;
        Cursor.lockState = mode;
        Cursor.visible = visible;
    }
    public void OnResume()
    {
        TogglePauseMenu();
    }

    public void OnRestart()
    {
        SceneManager.LoadScene(5);
        Time.timeScale = 1f;
    }

    public void OnReMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }
}
