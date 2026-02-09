# Система магазина - Инструкция по настройке

## 📦 Что создано

### Скрипты:
1. **IShopkeeper.cs** - Интерфейс для NPC-торговцев
2. **ShopData.cs** - ScriptableObject с конфигурацией магазина
3. **ShopManager.cs** - Менеджер транзакций купли-продажи
4. **ShopUI.cs** - UI панель с двумя инвентарями
5. **ShopInventorySlot.cs** - Компонент слота для магазина
6. **MerchantNPC.cs** - NPC-торговец

### Обновлённые скрипты:
- **InventoryContextMenu.cs** - Добавлены кнопки покупки/продажи для режима магазина
- **GameInstaller.cs** - Добавлена регистрация ShopManager в Zenject

## 🔧 Настройка (Шаг за шагом)

### Шаг 1: Создание ShopData (конфигурация товаров)

1. В Unity: ПКМ в Project → Create → Shop → Shop Data
2. Назовите файл (например, "GeneralStoreShop")
3. Настройте:
   - **Название магазина**: "Общий магазин"
   - **Описание**: "Здесь можно купить всё необходимое"
   - **Товары**: Добавьте предметы из Resources/Items
     - Для каждого товара укажите:
       - Item (ItemData)
       - Quantity (количество)
       - Buy Price (0 = использовать цену из ItemData)
       - Infinite Stock (бесконечный запас)
   - **Торговец покупает предметы**: включить/выключить
   - **Процент выкупа**: 50% (игрок продаёт за половину цены)

### Шаг 2: Настройка InventoryContextMenu

1. Найдите объект с компонентом **InventoryContextMenu** в вашей сцене
2. В инспекторе добавьте новые ссылки:
   - **Shop Manager** (будет назначен через Zenject или найдите в сцене)
   - **Shop UI** (ссылка на компонент ShopUI)

### Шаг 3: Создание UI магазина

1. Создайте Canvas для магазина (если нет):
   - GameObject → UI → Canvas
   - Назовите "ShopCanvas"

2. Создайте панель магазина:
   ```
   ShopCanvas
   └── ShopPanel (Panel)
       ├── Background (Image - затемнение)
       ├── ShopWindow (Panel)
       │   ├── Header
       │   │   ├── ShopNameText (TextMeshPro)
       │   │   ├── ShopDescriptionText (TextMeshPro)
       │   │   └── CloseButton (Button)
       │   ├── Content
       │   │   ├── MerchantPanel
       │   │   │   ├── Title: "Товары торговца"
       │   │   │   └── MerchantInventoryGrid (Grid Layout Group)
       │   │   │       └── MerchantSlot (x20) - InventorySlot + ShopInventorySlot
       │   │   └── PlayerPanel
       │   │       ├── Title: "Ваш инвентарь"
       │   │       ├── CoinsDisplay: "Монеты: [PlayerCoinsText]"
       │   │       └── PlayerInventoryGrid (Grid Layout Group)
       │   │           └── PlayerSlot (x20) - InventorySlot + ShopInventorySlot
   ```

3. Добавьте компонент **ShopUI** на ShopPanel:
   - Назначьте все ссылки:
     - Shop Panel
     - Merchant Inventory Panel
     - Player Inventory Panel
     - Merchant Slots (массив)
     - Merchant Item Prefab (ваш DraggableItem prefab)
     - Player Slots (массив)
     - Shop Name Text
     - Shop Description Text
     - Player Coins Text
     - Shop Manager (будет назначен автоматически через Zenject)
     - Inventory Manager (ссылка на MainUI.inventoryManager)
     - Player Wallet (ссылка на InventoryManager.Wallet)

4. На каждый слот (Merchant и Player) добавьте:
   - **InventorySlot** (базовый компонент)
   - **ShopInventorySlot** (для контекстного меню)
     - Назначьте Context Menu (ваш существующий InventoryContextMenu)
     - Назначьте Item Description Display (если есть)

### Шаг 4: Настройка ShopManager

1. Создайте пустой GameObject в сцене: "ShopManager"
2. Добавьте компонент **ShopManager**
3. Этот объект будет автоматически зарегистрирован через Zenject

### Шаг 5: Настройка GameInstaller

1. Откройте сцену с GameInstaller
2. Найдите GameObject с компонентом GameInstaller
3. В инспекторе назначьте:
   - Player (если не назначен)
   - Main UI (если не назначен)
   - **Shop Manager** (перетащите префаб или объект ShopManager)

### Шаг 6: Создание NPC-торговца

1. Создайте GameObject для NPC: "Merchant"
2. Добавьте компонент **MerchantNPC**:
   - Shop Data (назначьте созданный ShopData)
   - Dialogue Trigger (опционально, для интеграции с диалогами)
3. Добавьте Collider2D для взаимодействия
4. Добавьте визуал (Sprite Renderer)

### Шаг 7: Открытие магазина

Есть несколько способов:

**Способ 1: Через код**
```csharp
// В скрипте взаимодействия с NPC
MerchantNPC merchant = npc.GetComponent<MerchantNPC>();
if (merchant != null)
{
    merchant.OpenShop();
}
```

**Способ 2: Через Dialogue System**
```csharp
// В Lua команде диалога
DialogueLua.SetVariable("OpenShop", true);
// Затем в C# обработчике
if (DialogueLua.GetVariable("OpenShop").asBool)
{
    merchant.OpenShop();
}
```

**Способ 3: Через кнопку UI**
```csharp
// На кнопке "Торговать" в диалоге
Button tradeButton;
tradeButton.onClick.AddListener(() => merchant.OpenShop());
```

## 🎮 Использование

### Для игрока:

1. **Открыть магазин** - взаимодействовать с NPC-торговцем
2. **Купить предмет**:
   - ПКМ на предмете в инвентаре торговца
   - Выбрать "Купить X" в контекстном меню
3. **Продать предмет**:
   - ПКМ на предмете в своём инвентаре
   - Выбрать "Продать X" в контекстном меню
4. **Закрыть магазин** - кнопка "Закрыть" или ESC

### Особенности:

- ✅ Использует существующее контекстное меню
- ✅ Автоматически добавляет кнопки покупки/продажи в режиме магазина
- ✅ Проверка денег перед покупкой
- ✅ Проверка места в инвентаре
- ✅ Проверка наличия товара
- ✅ Поддержка стакающихся предметов
- ✅ Бесконечный запас товаров (опция)
- ✅ Настраиваемые цены покупки/продажи
- ✅ Процент выкупа для предметов

## 🔍 Отладка

### Проверка в консоли:

```
[ShopManager] Открыт магазин: Общий магазин
[ShopManager] Куплено: axe x1 за 50 монет
[ShopManager] Продано: Turnip x5 за 25 монет
[ShopUI] Магазин открыт: Общий магазин
```

### Частые проблемы:

1. **"ShopManager не инициализирован"**
   - Проверьте, что ShopManager назначен в GameInstaller
   - Убедитесь, что сцена с GameInstaller загружена

2. **"Недостаточно места в инвентаре"**
   - Проверьте метод CanAddItem в InventoryManager
   - Убедитесь, что есть пустые слоты

3. **"Предмет не продаётся в этом магазине"**
   - Добавьте предмет в список items в ShopData

4. **Контекстное меню не показывает кнопки магазина**
   - Проверьте, что ShopManager и ShopUI назначены в InventoryContextMenu
   - Убедитесь, что ShopInventorySlot добавлен на слоты магазина
   - Проверьте, что ShopUI.IsShopMode возвращает true

## 📝 Следующие шаги

- [ ] Создать UI префаб для магазина
- [ ] Добавить звуки покупки/продажи
- [ ] Интегрировать с Dialogue System
- [ ] Добавить анимации открытия/закрытия
- [ ] Создать несколько ShopData для разных торговцев
- [ ] Добавить систему репутации (скидки)
- [ ] Реализовать пополнение товаров со временем

## 🎨 Кастомизация

### Изменение цен:
В ShopData можно настроить:
- Индивидуальную цену для каждого товара
- Процент выкупа (по умолчанию 50%)
- Бесконечный запас

### Добавление новых опций в меню:
В InventoryContextMenu.cs добавьте новые кнопки в методах:
- `CreateBuyButtons()` - для покупки
- `CreateSellButtons()` - для продажи

### Ограничение типов товаров:
В ShopData можно добавить фильтр по ItemType:
```csharp
public ItemType acceptedTypes = ItemType.Food | ItemType.Potion;
```
