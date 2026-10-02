using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum RecordingMode
{
    None,
    Lap
}

[Serializable]
public class RacerTelemetryRecording
{
    public List<RacerTelemetryFrame> frames;

    public RacerTelemetryRecording(List<RacerTelemetryFrame> frames)
    {
        this.frames = frames;
    }
}

public class RacerRecorder : MonoBehaviour
{
    public RecordingMode mode = RecordingMode.Lap;

    public float recordingDistance = 5.0f;
    
    private RacerController racer;
    
    private RacerState racerState;

    private List<RacerTelemetryFrame> racerTelemetry = new();

    private Vector3 lastRecordedPosition;

    private float distanceTraveled;

    private float time;
    
    bool IsRecording = false;

    private void Start()
    {
        RaceManager.Instance.OnRaceStart += OnRaceStart;
    }

    private void OnRaceStart()
    {
        if (mode == RecordingMode.None)
        {
            gameObject.SetActive(false);
            return;
        }

        racer = GameManager.Instance.LocalRacer;
        racerState = racer.RacerState;
        racerState.OnReachedCheckpoint += ReachedCheckpoint;
        racerState.OnReachedStartFinishCheckpoint += ReachedStartFinishCheckpoint;
    }
    
    private void OnDisable()
    {
        if (RaceManager.Instance != null) RaceManager.Instance.OnRaceStart -= OnRaceStart;

        if (racerState != null)
        {
            racerState.OnReachedCheckpoint -= ReachedCheckpoint;
            racerState.OnReachedStartFinishCheckpoint -= ReachedStartFinishCheckpoint;
        }
    }

    private void ReachedCheckpoint(TrackCheckpoint checkpoint)
    {
        Vector3 racerPosition = racer.GetKartPosition();
        RecordFrame(racerPosition);
    }
    
    private void ReachedStartFinishCheckpoint(StartFinishCheckpoint startFinishCheckpoint)
    {
        IsRecording = false;
        Vector3 racerPosition = racer.GetKartPosition();
        
        if (racerTelemetry.Count > 0)
        {
            RecordFrame(racerPosition);
            SaveRecording();
        }
        racerTelemetry.Clear();

        Debug.Log("Started Recording");
        time = 0;
        RecordFrame(racerPosition);
        IsRecording = true;
        distanceTraveled = 0;
    }

    private async void SaveRecording()
    {
        List<RacerTelemetryFrame> racerTelemetryCopy = new List<RacerTelemetryFrame>(racerTelemetry);
        
        string trackname = SceneManager.GetActiveScene().name;
        string path = Path.Combine(Application.persistentDataPath, $"{trackname}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.json");
        
        await Task.Run(() =>
        {
            RacerTelemetryRecording racerTelemetryRecording = new RacerTelemetryRecording(racerTelemetryCopy);
            string json = JsonUtility.ToJson(racerTelemetryRecording);
            File.WriteAllText(path, json);
        });
        Debug.Log($"Telemetry saved to: {path}");
    }
    
    private void Update()
    {
        if (!IsRecording) return;
        
        time += Time.deltaTime;

        Vector3 racerPosition = racer.GetKartPosition();
        distanceTraveled += Vector3.Distance(lastRecordedPosition, racerPosition);
        lastRecordedPosition = racerPosition;
        
        if (distanceTraveled < recordingDistance) return;
        RecordFrame(racerPosition);
    }

    private void RecordFrame(Vector3 racerPosition)
    {
        RacerTelemetryFrame newFrame = new RacerTelemetryFrame(time, 1, racer.Steering, racer.Throttle, racer.Brake, racerPosition);
        distanceTraveled = 0;
        lastRecordedPosition = racerPosition;
        racerTelemetry.Add(newFrame);
    }
}
