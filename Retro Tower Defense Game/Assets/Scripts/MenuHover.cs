using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MenuHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI popupText;
    public GameObject currentTower;
    public GameObject textBack;

    public void OnPointerEnter(PointerEventData eventData)
    {
        popupText.text = currentTower.GetComponent<Tower>().towerDescription;
        textBack.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        popupText.text = currentTower.GetComponent<Tower>().towerDescription;
        textBack.gameObject.SetActive(false);
    }
}