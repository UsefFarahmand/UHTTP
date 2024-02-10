using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace UHTTP
{
    public static class SpriteDownloader
    {
        private static readonly DownloaderBehaviour Downloader;

        static SpriteDownloader() =>
            Downloader = new GameObject("DownloaderBehaviour").AddComponent<DownloaderBehaviour>();

        public static void DownloadSprite(string url, Action<Sprite> onComplete)
        {
            Downloader.StartCoroutine(Download(url, onComplete));

            static IEnumerator Download(string url, Action<Sprite> onComplete)
            {
                if (string.IsNullOrEmpty(url))
                {
                    Debug.LogWarning("Sprite URL is empty.");
                    onComplete?.Invoke(null);
                    yield break;
                }

                using UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);

                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError($"Failed to download image from {url}: {www.error}");
                    onComplete?.Invoke(null);
                }
                else
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(www);
                    if (texture != null)
                    {
                        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                        onComplete?.Invoke(sprite);
                    }
                    else
                    {
                        Debug.LogError($"Failed to create sprite from texture downloaded from {url}");
                        onComplete?.Invoke(null);
                    }
                }
            }
        }

        internal class DownloaderBehaviour : MonoBehaviour
        {

        }
    }
}
