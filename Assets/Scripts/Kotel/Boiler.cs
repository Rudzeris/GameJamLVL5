using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Recipe
{
    public string potionName;
    public List<IngredientAmount> ingredients = new();
}

[Serializable]
public class IngredientAmount
{
    public ItemType itemType;
    public int amount;
}


public class Boiler : MonoBehaviour, IInteractable
{
    private Dictionary<ItemType, int> currentIngredients = new();

    // Пример рецептов: ключ — название зелья, значение — рецепт (ингредиенты и их количество)
    private Dictionary<string, Dictionary<ItemType, int>> recipesDict;
    public List<Recipe> recipes = new();

    public GameObject KotelUI;

    private void Awake()
    {
        recipesDict = new Dictionary<string, Dictionary<ItemType, int>>();

        foreach (var recipe in recipes)
        {
            var dict = new Dictionary<ItemType, int>();
            foreach (var ingredient in recipe.ingredients)
            {
                dict[ingredient.itemType] = ingredient.amount;
            }
            recipesDict[recipe.potionName] = dict;
        }
    }


    public bool AddIngredient(ItemType type, int amount)
    {
        if (amount <= 0) return false;

        if (!currentIngredients.ContainsKey(type))
            currentIngredients[type] = 0;

        currentIngredients[type] += amount;
        Debug.Log($"Добавлено {amount}x {type} в котел. Всего: {currentIngredients[type]}");
        return true;
    }

    public void Activate()
    {
        KotelUI.SetActive(!KotelUI.activeSelf);
    }

    public void ClearIngredients()
    {
        currentIngredients.Clear();
        Debug.Log("Ингредиенты котла очищены.");
    }

    public void BrewPotion()
    {
        foreach (var recipe in recipesDict)
        {
            if (IsMatch(recipe.Value))
            {
                Debug.Log($"Зелье сварено: {recipe.Key}");
                ClearIngredients();
                return;
            }
        }

        Debug.Log("Зелье не распознано. Варка не удалась.");
        ClearIngredients();
    }

    private bool IsMatch(Dictionary<ItemType, int> recipe)
    {
        // Проверка, что все ингредиенты в рецепте совпадают с текущими ингредиентами (и наоборот)
        if (recipe.Count != currentIngredients.Count) return false;

        foreach (var pair in recipe)
        {
            if (!currentIngredients.TryGetValue(pair.Key, out var amount)) return false;
            if (amount != pair.Value) return false;
        }

        return true;
    }

    public int GetIngredientAmount(ItemType type)
    {
        return currentIngredients.TryGetValue(type, out var amount) ? amount : 0;
    }
    public Dictionary<ItemType, int> GetAllIngredients()
    {
        return new Dictionary<ItemType, int>(currentIngredients);
    }
}
