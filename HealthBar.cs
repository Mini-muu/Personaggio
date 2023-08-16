using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health h;
    [SerializeField] private Image totalHealthBar;
    [SerializeField] private Image currentHealtBar;

    //Importante: notare bene che il metodo sottostante è sempre attivo nell'editor del gioco e non solo durante la simulazione.

    private void OnDrawGizmos()
    {
        totalHealthBar.fillAmount = h.getMaxHealth() / 10;
        currentHealtBar.fillAmount = h.getHealth() / 10;
    }
}
