using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif
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

    private int checkpointCounter;
    
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

        racer = GameManager.Instance.LocalRacers[0];
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
        checkpointCounter++;
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
        checkpointCounter = 0;
        RecordFrame(racerPosition);
        IsRecording = true;
        distanceTraveled = 0;
    }

    private async void SaveRecording()
    {
        List<RacerTelemetryFrame> racerTelemetryCopy = new List<RacerTelemetryFrame>(racerTelemetry);
        
        string trackname = SceneManager.GetActiveScene().name;
        string fileName = $"{trackname}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.json";
#if UNITY_EDITOR
        // In the editor, save straight into the project so the recording can be used as a racing line.
        const string telemetryFolder = "Assets/RacerTelemetry";
        string assetPath = $"{telemetryFolder}/{fileName}";
        string path = Path.Combine(Application.dataPath, "..", assetPath);
        Directory.CreateDirectory(Path.GetDirectoryName(path));
#else
        string path = Path.Combine(Application.persistentDataPath, fileName);
#endif

        await Task.Run(() =>
        {
            RacerTelemetryRecording racerTelemetryRecording = new RacerTelemetryRecording(racerTelemetryCopy);
            string json = JsonUtility.ToJson(racerTelemetryRecording);
            File.WriteAllText(path, json);
        });
#if UNITY_EDITOR
        AssetDatabase.ImportAsset(assetPath);   // back on the main thread after the await
        Debug.Log($"Telemetry saved to: {assetPath}");
#else
        Debug.Log($"Telemetry saved to: {path}");
#endif
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

    private void RecordFrame(Vector3 racerPosition, TrackCheckpoint checkpoint = null)
    {
        RacerTelemetryFrame newFrame = new RacerTelemetryFrame(time, 1, racer.Steering, racer.Throttle, racer.Brake, racerPosition, checkpointCounter);
        distanceTraveled = 0;
        lastRecordedPosition = racerPosition;
        racerTelemetry.Add(newFrame);
    }
}
