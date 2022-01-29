using System;
using Gateway;
using UnityEngine;
using Zenject;

namespace Interactor
{
    public class DownloadInteractor
    {
        [Inject] private DownloadGateway downloadGateway;
        
        public void GetImage(string url, Action<Texture2D> onSuccess)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("No url for downloading image.");
                return;
            }

            if (!url.StartsWith("http"))
            {
                Debug.LogError("Wrong url format given for downloading image");
                return;
            }
            
            downloadGateway.GetPicture(url, 100000, onSuccess);
        }
    }
}
