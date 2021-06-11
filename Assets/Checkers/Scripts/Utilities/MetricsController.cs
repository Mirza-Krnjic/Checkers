using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.Profiling;
using Text = UnityEngine.UI.Text;

public class MetricsController : MonoBehaviour
{
    public GameObject MetricsContainer;

    public Text FpsText;
    public Text MomentalFpsText;
    public Text AverageFpsText;
    public Text MinMaxFpsText;
    public Text BadFramesText;
    public Text MemoryText;

    private float _accum = 0.0f; // FPS accumulated over the interval
    private int _frames = 0; // Frames drawn over the interval
    private int _avgFrames = 0;
    public float UpdateInterval = 0.5f;
    public float AverageInterval = 1f;
    public float MemoryInterval = 1f;
    public float MinMaxFpsInterval = 5f;

    private int _currentFps = 0;
    private int _momentalFps = 0;
    private int _minFps = 0;
    private int _maxFps = 0;
    private int _accumMomentalFps = 0;

    private float _timer;
    private float _nextUpdateTime;
    private float _nextAverageUpdateTime;
    private float _nextMemoryUpdateTime;
    private float _nextMinMaxFpsUpdateTime;

    private Dictionary<int, int> _badMs = new Dictionary<int, int>()
    {
        { 50,0 },
        { 100,0 },
        { 250,0 },
        { 500,0 },
    };

    public bool IsFpsActive = false;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        _timer = Time.time;

        _minFps = (int)(1 / Time.deltaTime);

        _nextUpdateTime = _timer + UpdateInterval;
        _nextAverageUpdateTime = _timer + AverageInterval;
        _nextMemoryUpdateTime = _timer + MemoryInterval;
        _nextMinMaxFpsUpdateTime = _timer + MinMaxFpsInterval;
    }

    private void Update()
    {
        if (!IsFpsActive)
        {
            return;
        }

        _timer += Time.deltaTime;
        _accum += Time.timeScale / Time.deltaTime;
        ++_frames;
        ++_avgFrames;
        CalculateBadFrames(Time.deltaTime);

        _momentalFps = (int)(1 / Time.deltaTime);
        _accumMomentalFps += _momentalFps;

        if (_momentalFps < _minFps)
        {
            _minFps = _momentalFps;
        }

        if (_momentalFps > _maxFps)
        {
            _maxFps = _momentalFps;
        }

        UpdateMomentalFPSView();

        if (_timer >= _nextUpdateTime)
        {
            _nextUpdateTime = _timer + UpdateInterval;

            _currentFps = (int)(_accum / _frames);

            UpdateFPSView();
            _accum = 0.0f;
            _frames = 0;
        }

        if (_timer >= _nextAverageUpdateTime)
        {
            _nextAverageUpdateTime = _timer + AverageInterval;

            UpdateAverageFPSView();
            _avgFrames = 0;
            _accumMomentalFps = 0;
        }

        if (_timer >= _nextMinMaxFpsUpdateTime)
        {
            _nextMinMaxFpsUpdateTime = _timer + MinMaxFpsInterval;

            UpdateMinMaxFpsView();

            _minFps = _momentalFps;
            _maxFps = _momentalFps;
        }

        if (_timer >= _nextMemoryUpdateTime)
        {
            _nextMemoryUpdateTime = _timer + MemoryInterval;

            UpdateMemoryView();
        }
    }

    private void CalculateBadFrames(float lastTimeFrame)
    {
        var timeInMs = TimeSpan.FromSeconds(lastTimeFrame).TotalMilliseconds;

        var key = -1;

        for (int i = 0; i < _badMs.Count; i++)
        {
            var element = _badMs.ElementAt(i);

            if (element.Key <= timeInMs)
            {
                key = element.Key;
            }
            else
            {
                break;
            }
        }

        if (key != -1)
        {
            _badMs[key]++;
        }

        UpdateBadFramesView();
    }

    private void UpdateBadFramesView()
    {
        string badFramesStr = string.Empty;

        for (int i = 0; i < _badMs.Count; i++)
        {
            var element = _badMs.ElementAt(i);

            badFramesStr += $"BF{element.Key} ms count: {element.Value}\n";
        }
        BadFramesText.text = badFramesStr;
    }

    private void UpdateFPSView()
    {
        var curFps = GetStringColor(_currentFps);
        FpsText.text = $"{curFps} FPS";
    }

    private void UpdateAverageFPSView()
    {
        var avgFrames = GetStringColor(_accumMomentalFps / _avgFrames);
        AverageFpsText.text = $"Average: {avgFrames} FPS";
    }

    private void UpdateMomentalFPSView()
    {
        var momentalFps = GetStringColor(_momentalFps);
        MomentalFpsText.text = $"Momental: {momentalFps} FPS";
    }

    private void UpdateMinMaxFpsView()
    {
        var min = GetStringColor(_minFps);
        var max = GetStringColor(_maxFps);
        MinMaxFpsText.text = $"Min {min} FPS Max {max} FPS";
    }

    private void UpdateMemoryView()
    {
        var reserved = Profiler.GetTotalAllocatedMemoryLong() / 1048576f;
        var total = SystemInfo.systemMemorySize;
        MemoryText.text = $"Memory:{reserved}/{total} MB";
    }

    private string GetStringColor(float value)
    {
        if (value < 30)
        {
            return $"<color=\"red\">{value:f0}</color>";
        }
        else if (_currentFps < 40)
        {
            return $"<color=\"yellow\">{value:f0}</color>";
        }
        else
        {
            return $"<color=\"white\">{value:f0}</color>";
        }
    }

    public void ActivateMetrics(bool state)
    {
        IsFpsActive = state;
        MetricsContainer.SetActive(state);
    }

    public void Reset()
    {
        _badMs = new Dictionary<int, int>()
                {
                    { 50,0 },
                    { 100,0 },
                    { 250,0 },
                    { 500,0 },
                };
    }
}
