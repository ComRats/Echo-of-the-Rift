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
    
    private static GlobalTimeSystem instance;
    public static GlobalTimeSystem Instance => instance;
    
    public DayNightCycle DayNightCycle => dayNightCycle;
    public TimeManager TimeManager => timeManager;
    
    private void Awake()
    {
        // Проверяем, что мы дочерний объект GlobalLoader
        if (transform.parent == null || transform.parent.GetComponent<GlobalLoader>() == null)
        {
            Debug.LogWarning("[GlobalTimeSystem] Должен быть дочерним объектом GlobalLoader!");
        }
        
        if (instance != null && instance != this)
        {
            Debug.LogWarning("[GlobalTimeSystem] Дубликат обнаружен и будет уничтожен");
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        
        // Инициализация компонентов
        InitializeComponents();
    }
    
    private void InitializeComponents()
    {
        // Если компоненты не назначены, пытаемся найти их
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
        
        // Настройка Light2D если не назначен
        if (globalLight == null)
        {
            globalLight = FindObjectOfType<Light2D>();
            if (globalLight == null)
            {
                Debug.LogWarning("[GlobalTimeSystem] Global Light2D не найден! Создайте Light2D (Global) в сцене.");
            }
        }
        
        // Передаём ссылку на свет в DayNightCycle
        if (dayNightCycle != null && globalLight != null)
        {
            dayNightCycle.SetGlobalLight(globalLight);
        }
        
        // Связываем TimeManager с DayNightCycle
        if (timeManager != null && dayNightCycle != null)
        {
            timeManager.SetDayNightCycle(dayNightCycle);
        }
    }
    
    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
