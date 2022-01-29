using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickupOption : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI name, distance;
    [SerializeField] private Toggle toggle;
    private string currentStore;
    
    public void SetStoreToPrefab(string store, ToggleGroup toggleGroup)
    {
        name.text = store;
        distance.text = Random.Range(0, 11) + "." + Random.Range(1, 10) + " км";
        currentStore = store;
        toggle.group = toggleGroup;
        toggleGroup.RegisterToggle(toggle);
    }

    public string GetStore() => currentStore;
}
