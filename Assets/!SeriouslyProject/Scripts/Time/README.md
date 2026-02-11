# Система День/Ночь - Инструкция по настройке

## Созданные компоненты:

1. **GameTimer.cs** - Базовая система времени (уже был)
2. **DayNightCycle.cs** - Контроллер цикла день/ночь с освещением
3. **TimeUI.cs** - UI для отображения времени и периода суток
4. **TimeManager.cs** - Менеджер для управления временем
5. **GlobalTimeSystem.cs** - Глобальная система для DontDestroyOnLoad

## Настройка в Unity:

### Вариант A: Интеграция с GlobalLoader (рекомендуется)

Этот вариант сохраняет систему времени между сценами через GlobalLoader.

#### Шаг 1: Настройка в префабе GlobalLoader

1. Откройте префаб **GlobalLoader** (Assets/!SeriouslyProject/Prefabs/GlobalLoader.prefab)
2. Создайте дочерний GameObject и назовите его "TimeSystem"
3. Добавьте компонент **GlobalTimeSystem** на "TimeSystem"
4. Компоненты DayNightCycle и TimeManager добавятся автоматически

#### Шаг 2: Создание Global Light 2D

Важно: Light2D должен быть в каждой игровой сцене (не в GlobalLoader)!

1. В каждой игровой сцене создайте GameObject "Global Light"
2. Добавьте компонент **Light 2D** (Component → Rendering → Light 2D)
3. Настройте Light 2D:
   - Light Type: **Global**
   - Intensity: **1**
   - Color: **White**
   - Blend Style: **Default**

GlobalTimeSystem автоматически найдёт Light2D при загрузке сцены.

### Вариант B: Отдельная система в каждой сцене

Если не нужно сохранять время между сценами:

### Вариант B: Отдельная система в каждой сцене

Если не нужно сохранять время между сценами:

### Шаг 1: Настройка освещения (2D Light)

1. В сцене создайте пустой GameObject и назовите его "DayNightSystem"
2. Добавьте компонент **Light 2D** (Component → Rendering → Light 2D)
3. Настройте Light 2D:
   - Light Type: **Global**
   - Intensity: **1**
   - Color: **White**
   - Blend Style: **Default**

### Шаг 2: Добавление DayNightCycle

1. На GameObject "DayNightSystem" добавьте компонент **DayNightCycle**
2. В инспекторе:
   - Перетащите созданный Light 2D в поле **Global Light**
   - Можно оставить **Time Of Day Settings** пустым (будут использованы дефолтные настройки)
   - Или настроить свои периоды времени

### Шаг 3: Настройка UI времени

#### Вариант A: Обновить существующий TimeWeatherQuest

Замените компонент `TimeWeatherQuest` на `TimeUI` в вашем UI:

1. Найдите GameObject с компонентом TimeWeatherQuest
2. Удалите компонент TimeWeatherQuest
3. Добавьте компонент **TimeUI**
4. Назначьте поля:
   - **Time Text** - TextMeshProUGUI для времени
   - **Day Text** - TextMeshProUGUI для дня
   - **Period Text** - TextMeshProUGUI для периода суток (новое)
   - **Period Icon** - Image для иконки (опционально)

#### Вариант B: Использовать оба компонента

Можно оставить TimeWeatherQuest и добавить TimeUI на другой GameObject для расширенного отображения.

### Шаг 4: Добавление TimeManager (опционально)

1. На GameObject "DayNightSystem" добавьте компонент **TimeManager**
2. Назначьте **Day Night Cycle** (перетащите компонент DayNightCycle)
3. Настройте скорости времени:
   - Normal Time Scale: 1 (по умолчанию)
   - Fast Time Scale: 5 (для ускорения)

## Использование в коде:

### Получение ссылки на TimeManager:
```csharp
[SerializeField] private TimeManager timeManager;

// Или через Zenject:
[Inject] private TimeManager timeManager;
```

### Получение текущего времени:
```csharp
int hour = timeManager.GetCurrentHour();
int minute = timeManager.GetCurrentMinute();
int day = timeManager.GetCurrentDay();
```

### Проверка периода суток:
```csharp
if (timeManager.IsNight())
{
    // Логика для ночи
}

DayPeriod period = timeManager.GetCurrentPeriod();
```

### Управление временем:
```csharp
// Установить время на 18:30
timeManager.SetTime(18, 30);

// Пропустить 8 часов (сон)
timeManager.SkipTime(8);

// Пропустить до утра
timeManager.SkipToMorning();

// Ускорить время
timeManager.SetFastTime();
```

### Подписка на события:
```csharp
private void OnEnable()
{
    DayNightCycle.OnDayPeriodChanged += OnPeriodChanged;
}

private void OnDisable()
{
    DayNightCycle.OnDayPeriodChanged -= OnPeriodChanged;
}

private void OnPeriodChanged(DayPeriod newPeriod)
{
    Debug.Log($"Период изменился на: {newPeriod}");
}
```

## Настройка периодов времени:

В инспекторе DayNightCycle можно настроить массив **Time Of Day Settings**:

- **Period Name** - Название периода (для отладки)
- **Start Hour** - Час начала (0-23)
- **Light Color** - Цвет освещения
- **Light Intensity** - Яркость (0-2)

### Рекомендуемые настройки:

1. **Ночь** (0:00)
   - Color: RGB(51, 51, 102) - тёмно-синий
   - Intensity: 0.3

2. **Рассвет** (5:00)
   - Color: RGB(255, 179, 128) - оранжево-розовый
   - Intensity: 0.6

3. **Утро** (7:00)
   - Color: RGB(255, 242, 204) - светло-жёлтый
   - Intensity: 0.9

4. **День** (12:00)
   - Color: RGB(255, 255, 255) - белый
   - Intensity: 1.0

5. **Вечер** (18:00)
   - Color: RGB(255, 153, 77) - оранжевый
   - Intensity: 0.7

6. **Сумерки** (20:00)
   - Color: RGB(102, 77, 153) - фиолетовый
   - Intensity: 0.4

## Важные замечания:

1. **URP (Universal Render Pipeline)** должен быть настроен в проекте
2. Все спрайты должны использовать материал с поддержкой 2D освещения
3. Время останавливается при открытии меню/инвентаря/магазина (уже реализовано)
4. Время сохраняется и загружается автоматически (уже реализовано)

## Отладка:

В DayNightCycle включите **Show Debug Info** для вывода информации в консоль:
- Текущее время
- Период суток
- Прогресс перехода
- Цвет и яркость освещения
