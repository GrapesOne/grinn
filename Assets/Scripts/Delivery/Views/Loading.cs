using System.Collections;
using System.Threading;
using Interactor;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

public class Loading : MonoBehaviour
{
    [Inject] private LoadingInteractor loadingInteractor;
    [SerializeField] private GameObject LoadingTapHandler;
    [SerializeField] private TextMeshProUGUI text;
    private Animator animator;
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    void Awake()
    {
        animator = GetComponent<Animator>();
        loadingInteractor.OnStartLoad.Subscribe(SendStart);
        loadingInteractor.OnEndLoad.Subscribe(SendEnd);
        loadingInteractor.WhenLongLoad.Subscribe(LongLoading);
    }

    public const string BaseText = "Секундочку...";
    public void SetText(string val)
    {
        text.text = val;
    }
    void OnEnable()
    {
        StartCoroutine(Close(0));
    }

    private void LongLoading(LoadingTrigger trigger)
    {
        _cancellationTokenSource.Cancel();
        SendTrigger(trigger.ToString(), false);
    }
    IEnumerator Close(float seconds)
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        var token = _cancellationTokenSource.Token;
        yield return new WaitForSeconds(seconds);
        if(token.IsCancellationRequested) yield break;
        loadingInteractor?.ClosedLoad();
        LoadingTapHandler.SetActive(false);
        SendTrigger(LoadingTrigger.Closed.ToString(), false);
    }
    private void SendEnd(LoadingTrigger trigger)
    {
        SendTrigger(trigger.ToString(), false);
        StartCoroutine(Close(0.6f));
    }
    
    public void SendStart(LoadingTrigger trigger)
    {
        SetText(BaseText);
        SendTrigger(trigger.ToString(),true);
        StartCoroutine(Close(8));
    }

    private void SendTrigger(string name, bool val)
    {
        LoadingTapHandler.SetActive(val);
        animator.SetTrigger(name);
    }
}
