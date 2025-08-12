using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JarGameAutoInit : MonoBehaviour
{
    public GameObject jarButtonPrefab;  // ������ ������ � TextMeshProUGUI
    public Transform jarsPanel;          // ��������� � GridLayoutGroup
    public TextMeshProUGUI attemptsText;
    public TextMeshProUGUI hintText;

    private Button[] jarButtons;
    private string[] jarsContents = new string[9] {
        "�����", "�����", "̸�",
        "�����", "�������", "�����",
        "������", "�����", "�����"
    };

    private bool[] openedJars;
    public int attemptsLeft = 3;
    private int ingredientsFound = 0;
    private string[] neededIngredients = new string[] { "̸�", "�������", "������" };

    void Start()
    {
        InitializeJars();
    }

    void InitializeJars()
    {
        // ������� ����������
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

            jarText.text = i.ToString();  // �������� ����������

            int index = i; // ��������� ����� ��� ���������
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

        if (content == "�����")
        {
            attemptsLeft--;

            // ���� ������ ������ �� ��� �� ��������� ������������
            int targetIndex = -1;
            for (int i = 0; i < jarsContents.Length; i++)
            {
                if (!openedJars[i] && System.Array.Exists(neededIngredients, item => item == jarsContents[i]))
                {
                    targetIndex = i;
                    break;
                }
            }

            // ���� �����, ��� ��������� �� ����, ����� ������ ���������
            if (targetIndex != -1)
                hintText.text = GetHint(targetIndex);
            else
                hintText.text = "��... �������, ������� ����������� ������ ����� ���...";
        }
        else if (System.Array.Exists(neededIngredients, item => item == content))
        {
            ingredientsFound++;
            hintText.text = $"����� ����������: {content}!";
        }

        jarButtons[index].interactable = false;

        if (ingredientsFound == neededIngredients.Length)
        {
            hintText.text = "��� ����������� �������! �������� ������!";
            EndGame(true);
        }
        else if (attemptsLeft == 0)
        {
            hintText.text = "������� �����������. ���������� �����!";
            EndGame(false);
        }
    }

    void UpdateAttemptsText()
    {
        attemptsText.text = $"�������: {attemptsLeft}";
    }

    string GetHint(int lastIndex)
    {
        int row = lastIndex / 3;    // 0 = �������, 1 = �������, 2 = ������
        int col = lastIndex % 3;    // 0 = �����, 1 = �����, 2 = ������

        string rowHint = row switch
        {
            0 => "������� ����� ����� ������ ����...",
            1 => "���-�� ���������� ����� ���� ��, ��� �����...",
            2 => "������ ������������ � ������ ����...",
            _ => ""
        };

        string colHint = col switch
        {
            0 => "�������, ����� � ������ ����.",
            1 => "����� �� ������.",
            2 => "� ������ �������, ����� � ����.",
            _ => ""
        };

        return $"{rowHint} {colHint}";
    }

    void EndGame(bool success)
    {
        for (int i = 0; i < jarButtons.Length; i++)
            jarButtons[i].interactable = false;

        Messenger<bool>.Broadcast(GameEvent.JAR_GAME_COMPLETE, success);
        // �������������� ������ ����� ����
    }
}
