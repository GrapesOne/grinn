using Delivery.Views;
using Interactor;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ChangeViewButtonHandler : MonoBehaviour
{
    [ValueDropdown(nameof(DropdownList))]
    public int NewView;
    private ValueDropdownList<int> DropdownList => ViewsController.ViewsDropdownList;
    
    [Inject] private ChangeViewInteractor changeViewInteractor;
    
    void Awake()
    {
        changeViewInteractor ??= ChangeViewDucttape.Interactor;
        var button = GetComponent<Button>();
        if (button == null) button = gameObject.AddComponent<Button>();
        button.onClick.AddListener(Call);
    }

    void Call() => changeViewInteractor.ChangeView(NewView);
}
