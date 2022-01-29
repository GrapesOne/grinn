using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

namespace Gateway
{
    public class DownloadGateway
    {
        public void GetPicture(string url, int maxBytes, Action<Texture2D> callback)
        {
            if (!url.StartsWith("http"))
            {
                Debug.LogWarning("Invalid url for avatar: " + url);
                callback(null);
                return;
            }

            Observable.FromCoroutine<Texture2D>(observer => GetTexture(url, observer, maxBytes)).Subscribe(
                callback, error =>
                {
                    if(error.ToString() != "System.Exception: HTTP/1.1 404 Not Found")
                        Debug.LogWarning(url + "--->" + error);
                    else Debug.Log("404 Not Found ---> in loading image");
                    callback(null);
                });
        }

        private IEnumerator GetTexture(string url, IObserver<Texture2D> observer, int maxBytes)
        {
            for (int i = 0; i < 3; i++) //retrying up to 3 times
            {
                using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
                {
                    www.timeout = 20;
                    www.SendWebRequest();
                    while (!www.isDone)
                    {
                        if (www.downloadedBytes > (ulong) maxBytes)
                        {
                            Debug.LogError($"Download image too big: {url}");
                            observer.OnNext(null);
                            observer.OnCompleted();
                            yield break;
                        }

                        yield return null;
                    }

                    if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                    {
                        if (i == 2)
                        {
                            observer.OnError(new Exception(www.error));
                            observer.OnCompleted();
                        }
                    }
                    else
                    {
                        if (www.downloadedBytes >= 512)
                        {
                            Texture2D texture = ((DownloadHandlerTexture) www.downloadHandler).texture;
                            observer.OnNext(texture);
                            observer.OnCompleted();
                        }
                        else
                        {
                            observer.OnNext(null);
                            observer.OnCompleted();
                        }

                        break;
                    }
                }
            }
        }
    }

    public interface IUrlDownload
    {
        void GetPicture(string url,  int maxBytes, Action<Texture2D> callback);
    }
}