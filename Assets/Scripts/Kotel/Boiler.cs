using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


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

    // ������ ��������: ���� � �������� �����, �������� � ������ (����������� � �� ����������)
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
        Debug.Log($"��������� {amount}x {type} � �����. �����: {currentIngredients[type]}");
        return true;
    }

    public void Activate()
    {
        KotelUI.SetActive(!KotelUI.activeSelf);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        KotelUI.SetActive(false);
    }

    public void ClearIngredients()
    {
        currentIngredients.Clear();
        Debug.Log("����������� ����� �������.");
    }

    public void BrewPotion()
    {
        foreach (var recipe in recipes)
        {
            // ���������� ������ � ������� ��� ��������
            var dict = new Dictionary<ItemType, int>();
            foreach (var ing in recipe.ingredients)
            {
                dict[ing.itemType] = ing.amount;
            }

            if (IsMatch(dict))
            {
                Debug.Log($"����� �������: {recipe.potionName}");

                // ������� ����
                ClearIngredients();
                KotelUI.GetComponent<BoilerUI>().ClearBoilerUI();

                // ���������� ������� � ������������ ������� ������������
                Messenger<string, List<IngredientAmount>>.Broadcast(
                    GameEvent.RECIPE_DISCOVERED,
                    recipe.potionName,
                    recipe.ingredients
                );

                return;
            }
        }

        Debug.Log("����� �� ����������. ����� �� �������.");
        ClearIngredients();
        Managers.Game.RestartCurrent();
    }


    private bool IsMatch(Dictionary<ItemType, int> recipe)
    {
        // ��������, ��� ��� ����������� � ������� ��������� � �������� ������������� (� ��������)
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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ClearIngredients();
    }
}
