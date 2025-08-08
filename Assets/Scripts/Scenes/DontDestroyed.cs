using System.Collections.Generic;
using UnityEngine;

public enum TypeDontDestroyed
{
    None,
    Audio
}
public class DontDestroyed : MonoBehaviour
{
    [SerializeField] private TypeDontDestroyed type;
    private static List<TypeDontDestroyed> list = new List<TypeDontDestroyed>();
    private void Start()
    {
        if (type == TypeDontDestroyed.None)
        {
            Debug.LogError("TypeDontDestroyed is None. Please set a valid type.");
            return;
        }

        if (list.IndexOf(type) == -1)
        {
            list.Add(type);
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
