using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class EndingSequence : MonoBehaviour
{
    [Header("Cutscene (Video)")]
    public VideoPlayer videoPlayer;

    [Header("Credits")]
    public GameObject creditsPanel;
    public RectTransform creditsText;
    public float scrollSpeed = 80f;

    [Header("End Buttons (ปุ่มตอนจบ)")]
    public GameObject endButtonsPanel;

    [Header("After")]
    public string menuSceneName = "MainMenu";

    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true; // ต้องเปิดเมาส์ให้มองเห็นเพื่อกดปุ่มได้

        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (endButtonsPanel != null) endButtonsPanel.SetActive(false);

        StartCoroutine(Run());
    }

    bool SkipPressed()
    {
        bool key = Keyboard.current != null &&
            (Keyboard.current.spaceKey.wasPressedThisFrame ||
             Keyboard.current.enterKey.wasPressedThisFrame);
        bool click = Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame;
        return key || click;
    }

    IEnumerator Run()
    {
        // 1) เล่นวิดีโอคัตซีน
        if (videoPlayer != null)
        {
            // ลบคำสั่งปิด/เปิด GameObject ออก เพื่อป้องกันสคริปต์ปิดตัวเอง
            videoPlayer.Play();

            while (!videoPlayer.isPlaying)
            {
                yield return null;
            }

            while (videoPlayer.isPlaying)
            {
                if (SkipPressed())
                {
                    videoPlayer.Stop();
                    break;
                }
                yield return null;
            }
            // เมื่อเล่นจบ วิดีโอจะหยุดภาพค้างไว้ที่เฟรมสุดท้ายเป็นพื้นหลังให้ปุ่ม
        }

        // 2) เครดิตเลื่อนขึ้น
        if (creditsPanel != null && creditsText != null)
        {
            creditsPanel.SetActive(true);
            yield return null;
            LayoutRebuilder.ForceRebuildLayoutImmediate(creditsText);

            RectTransform parent = creditsText.parent as RectTransform;
            float startY = -(parent.rect.height / 2f + creditsText.rect.height / 2f);
            float endY = parent.rect.height / 2f + creditsText.rect.height / 2f;

            Vector2 pos = creditsText.anchoredPosition;
            pos.y = startY;
            creditsText.anchoredPosition = pos;

            while (pos.y < endY)
            {
                pos.y += scrollSpeed * Time.deltaTime;
                creditsText.anchoredPosition = pos;

                if (Keyboard.current != null &&
                    Keyboard.current.escapeKey.wasPressedThisFrame)
                    break;

                yield return null;
            }
            creditsPanel.SetActive(false);
        }

        // 3) โชว์แผงปุ่ม
        if (endButtonsPanel != null)
        {
            endButtonsPanel.SetActive(true);
        }
    }

    public void OnClick_ReplayVideo()
    {
        if (endButtonsPanel != null) endButtonsPanel.SetActive(false);
        
        if (videoPlayer != null) 
        {
            videoPlayer.time = 0; // รีเซ็ตเวลาวิดีโอ
        }
        
        StartCoroutine(Run());
    }

    public void OnClick_ReturnToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}