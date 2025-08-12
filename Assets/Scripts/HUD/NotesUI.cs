using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class NotesUI : MonoBehaviour
{
    public Transform notesContainer;  // Контейнер UI
    public GameObject notePrefab;     // Префаб UI-элемента заметки

    private void OnEnable()
    {
        Messenger<string, System.Collections.Generic.List<IngredientAmount>>.AddListener(GameEvent.RECIPE_DISCOVERED, OnRecipeDiscovered);
        LoadSavedRecipes();
    }

    private void OnDisable()
    {
        Messenger<string, System.Collections.Generic.List<IngredientAmount>>.RemoveListener(GameEvent.RECIPE_DISCOVERED, OnRecipeDiscovered);
    }

    private void LoadSavedRecipes()
    {
        foreach (var recipe in SessionData.Instance.discoveredRecipes)
        {
            CreateNoteUI(recipe.RecipeName, recipe.Ingredients);
        }
    }

    private void OnRecipeDiscovered(string recipeName, System.Collections.Generic.List<IngredientAmount> ingredients)
    {
        // Проверяем, не сохранен ли уже такой рецепт, чтобы не дублировать
        bool alreadyExists = SessionData.Instance.discoveredRecipes.Exists(r => r.RecipeName == recipeName);
        if (alreadyExists) return;

        // Сохраняем рецепт в сессии
        var newRecipe = new SessionData.RecipeNote(recipeName, ingredients);
        SessionData.Instance.discoveredRecipes.Add(newRecipe);

        // Создаем UI заметку
        CreateNoteUI(recipeName, ingredients);
    }

    private void CreateNoteUI(string recipeName, System.Collections.Generic.List<IngredientAmount> ingredients)
    {
        var noteGO = Instantiate(notePrefab, notesContainer);
        var textComp = noteGO.GetComponentInChildren<TextMeshProUGUI>();
        if (textComp != null)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"📜 {recipeName}");
            sb.AppendLine("Ингредиенты:");

            foreach (var ing in ingredients)
                sb.AppendLine($"- {ing.itemType} x{ing.amount}");

            textComp.text = sb.ToString();
        }
    }
}

public class SessionData
{
    private static SessionData _instance;
    public static SessionData Instance => _instance ??= new SessionData();

    // Здесь храним уже обнаруженные рецепты
    public List<RecipeNote> discoveredRecipes = new();

    private SessionData() { }

    public class RecipeNote
    {
        public string RecipeName;
        public List<IngredientAmount> Ingredients;

        public RecipeNote(string name, List<IngredientAmount> ingredients)
        {
            RecipeName = name;
            Ingredients = ingredients;
        }
    }
}