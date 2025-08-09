using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    [SerializeField] private float startTime = 90f; // Время в секундах
    [SerializeField] private TMP_Text timerText;    // TextMeshPro UI

    private float currentTime;
    private bool isRunning = false;

    private void Start()
    {
        StartTimer();
    }

    public void StartTimer()
    {
        currentTime = startTime;
        isRunning = true;
        StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateUI();
            yield return null;
        }

        currentTime = 0;
        UpdateUI();
        isRunning = false;

        // Сообщаем, что время вышло
        Debug.LogWarning("TIMER_COMPLETE");
        try
        {
            Messenger.Broadcast(GameEvent.TIMER_COMPLETE);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Error: {ex.Message}");
        }
    }

    private void UpdateUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
