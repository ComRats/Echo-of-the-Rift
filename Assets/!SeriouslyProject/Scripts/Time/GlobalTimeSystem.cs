using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Глобальная система времени, которая сохраняется между сценами
/// Должна быть дочерним объектом GlobalLoader
/// </summary>
public class GlobalTimeSystem : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private DayNightCycle dayNightCycle;
    [SerializeField] private TimeManager timeManager;
    
    [Header("Lighting")]
    [SerializeField] private Light2D globalLight;
    
    public DayNightCycle DayNightCycle => dayNightCycle;
    public TimeManager TimeManager => timeManager;
    
    private void Awake()
    {
        if (transform.parent == null || transform.parent.GetComponent<GlobalLoader>() == null)
        {
            Debug.LogWarning("[GlobalTimeSystem] Должен быть дочерним объектом GlobalLoader!");
        }
        
        InitializeComponents();
    }
    
    private void InitializeComponents()
    {
        if (dayNightCycle == null)
        {
            dayNightCycle = GetComponent<DayNightCycle>();
            if (dayNightCycle == null)
            {
                dayNightCycle = gameObject.AddComponent<DayNightCycle>();
                Debug.Log("[GlobalTimeSystem] DayNightCycle добавлен автоматически");
            }
        }
        
        if (timeManager == null)
        {
            timeManager = GetComponent<TimeManager>();
            if (timeManager == null)
            {
                timeManager = gameObject.AddComponent<TimeManager>();
                Debug.Log("[GlobalTimeSystem] TimeManager добавлен автоматически");
            }
        }
        
        if (globalLight == null)
        {
            globalLight = FindObjectOfType<Light2D>();
            if (globalLight == null)
            {
                Debug.LogWarning("[GlobalTimeSystem] Global Light2D не найден! Создайте Light2D (Global) в сцене.");
            }
        }
        
        if (dayNightCycle != null && globalLight != null)
        {
            dayNightCycle.SetGlobalLight(globalLight);
        }
        
        if (timeManager != null && dayNightCycle != null)
        {
            timeManager.SetDayNightCycle(dayNightCycle);
        }
    }
    

}
