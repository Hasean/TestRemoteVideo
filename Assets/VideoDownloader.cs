using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoDownloader : MonoBehaviour
{
    [SerializeField] private string videoURL; // Ссылка на видео
    [SerializeField] private VideoPlayer videoPlayer;
    public RawImage rawTexture;// Компонент VideoPlayer

    private void Start()
    {
        // Начинаем загрузку видео
        StartCoroutine(DownloadAndPlayVideo());
    }

    private IEnumerator DownloadAndPlayVideo()
    {
        // Отправляем запрос на скачивание видео
        UnityWebRequest request = UnityWebRequest.Get(videoURL);
        request.SendWebRequest();

        // Ожидаем завершения загрузки
        while (!request.isDone)
        {
            Debug.Log($"Загрузка: {request.downloadProgress * 100}%");
            yield return null;
        }

        // Проверяем наличие ошибок
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Ошибка загрузки видео: {request.error}");
            yield break;
        }

        // Загружаем данные в память
        byte[] videoData = request.downloadHandler.data;

        // Сохраняем данные как временный файл
        string tempPath = System.IO.Path.Combine(Application.persistentDataPath, "tempVideo.mp4");
        System.IO.File.WriteAllBytes(tempPath, videoData);

        Debug.Log($"Видео загружено в {tempPath}");

        // Настраиваем VideoPlayer
        videoPlayer.url = tempPath;

        // Подготавливаем и воспроизводим видео
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += (source) =>
        {
            Debug.Log("Видео готово к воспроизведению.");
            videoPlayer.Play();
            rawTexture.texture = videoPlayer.texture;
        };
    }
}