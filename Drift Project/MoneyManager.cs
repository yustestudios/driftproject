using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{
    public Text moneyText; // Asigna el Texto de UI desde el Inspector
    private int money = 0; // Dinero actual

    private void Start() {
        
    }
    // Función para añadir dinero
    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoneyText();
    }

    // Función para restar dinero
    public void SubtractMoney(int amount)
    {
        money -= amount;

        // Asegurarse de que el dinero no sea negativo
        money = Mathf.Max(0, money);

        UpdateMoneyText();
    }

    // Función para actualizar el texto de UI con la cantidad actual de dinero
    private void UpdateMoneyText()
    {
        if (moneyText != null)
        {
            moneyText.text = money.ToString();
        }
    }
}
