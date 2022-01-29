using System.Collections;
using System.Collections.Generic;
using Interactor;
using UnityEngine;
using Zenject;

public class ChangeViewDucttape : MonoBehaviour
{
    [Inject] private ChangeViewInteractor changeViewInteractor;
    public static ChangeViewInteractor Interactor;
    void Awake() => Interactor = changeViewInteractor;
}
