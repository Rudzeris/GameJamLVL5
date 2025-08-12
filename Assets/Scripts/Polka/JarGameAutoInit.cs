using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JarGameAutoInit : MonoBehaviour
{
    public GameObject jarButtonPrefab;  // Префаб кнопки с TextMeshProUGUI
    public Transform jarsPanel;          // Контейнер с GridLayoutGroup
    public TextMeshProUGUI attemptsText;
    public TextMeshProUGUI hintText;

    private Button[] jarButtons;
    private string[] jarsContents = new string[9] {
        "Пусто", "Пусто", "Мёд",
        "Пусто", "Лаванда", "Пусто",
        "Молоко", "Пусто", "Пусто"
    };

    private bool[] openedJars;
    public int attemptsLeft = 3;
    private int ingredientsFound = 0;
    private string[] neededIngredients = new string[] { "Мёд", "Лаванда", "Молоко" };

    void Start()
    {
        InitializeJars();
    }

    void InitializeJars()
    {
        // Очистка контейнера
        foreach (Transform child in jarsPanel)
            Destroy(child.gameObject);

        jarButtons = new Button[jarsContents.Length];
        openedJars = new bool[jarsContents.Length];
        ingredientsFound = 0;
        hintText.text = "";
        UpdateAttemptsText();

        for (int i = 0; i < jarsContents.Length; i++)
        {
            GameObject jarObj = Instantiate(jarButtonPrefab, jarsPanel);
            Button jarBtn = jarObj.GetComponent<Button>();
            TextMeshProUGUI jarText = jarBtn.GetComponentInChildren<TextMeshProUGUI>();

            jarText.text = i.ToString();  // Скрываем содержимое

            int index = i; // Локальная копия для замыкания
            jarBtn.onClick.RemoveAllListeners();
            jarBtn.onClick.AddListener(() => OnJarClicked(index));
            jarBtn.interactable = true;

            jarButtons[i] = jarBtn;
            openedJars[i] = false;
        }
    }

    void OnJarClicked(int index)
    {
        if (openedJars[index] || attemptsLeft <= 0)
            return;

        openedJars[index] = true;

        UpdateAttemptsText();

        string content = jarsContents[index];
        TextMeshProUGUI jarText = jarButtons[index].GetComponentInChildren<TextMeshProUGUI>();
        jarText.text = content;

        if (content == "Пусто")
        {
            attemptsLeft--;

            // Ищем индекс одного из ещё не найденных ингредиентов
            int targetIndex = -1;
            for (int i = 0; i < jarsContents.Length; i++)
            {
                if (!openedJars[i] && System.Array.Exists(neededIngredients, item => item == jarsContents[i]))
                {
                    targetIndex = i;
                    break;
                }
            }

            // Если нашли, даём подсказку по нему, иначе пустое сообщение
            if (targetIndex != -1)
                hintText.text = GetHint(targetIndex);
            else
                hintText.text = "Хм... странно, нужного ингредиента больше нигде нет...";
        }
        else if (System.Array.Exists(neededIngredients, item => item == content))
        {
            ingredientsFound++;
            hintText.text = $"Нашли ингредиент: {content}!";
        }

        jarButtons[index].interactable = false;

        if (ingredientsFound == neededIngredients.Length)
        {
            hintText.text = "Все ингредиенты собраны! Отличная работа!";
            EndGame(true);
        }
        else if (attemptsLeft == 0)
        {
            hintText.text = "Попытки закончились. Попробуйте снова!";
            EndGame(false);
        }
    }

    void UpdateAttemptsText()
    {
        attemptsText.text = $"Попыток: {attemptsLeft}";
    }

    string GetHint(int lastIndex)
    {
        int row = lastIndex / 3;    // 0 = верхний, 1 = средний, 2 = нижний
        int col = lastIndex % 3;    // 0 = слева, 1 = центр, 2 = справа

        string rowHint = row switch
        {
            0 => "Верхняя полка будто шепчет тебе...",
            1 => "Где-то посередине полки есть то, что нужно...",
            2 => "Смотри внимательнее у самого пола...",
            _ => ""
        };

        string colHint = col switch
        {
            0 => "кажется, ближе к левому краю.",
            1 => "прямо по центру.",
            2 => "в правой стороне, почти в углу.",
            _ => ""
        };

        return $"{rowHint} {colHint}";
    }

    void EndGame(bool success)
    {
        for (int i = 0; i < jarButtons.Length; i++)
            jarButtons[i].interactable = false;

        Messenger<bool>.Broadcast(GameEvent.JAR_GAME_COMPLETE, success);
        // Дополнительная логика конца игры
    }
}
