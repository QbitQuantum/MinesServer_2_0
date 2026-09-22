using System.Collections.Generic;
using System.IO;
using UnityEngine;
public static class ResourcesManager
{
    // ========== НАСТРОЙКИ ==========
    /// <summary>
    /// Использовать папку mods для загрузки спрайтов (по умолчанию false)
    /// </summary>
    public static bool UseModsFolder = false;
    // ================================

    // Кэш для загруженных спрайтов
    private static Dictionary<string, Sprite> _spriteCache = new Dictionary<string, Sprite>();

    // Кэш для массивов спрайтов
    private static Dictionary<string, Sprite[]> _spriteArrayCache = new Dictionary<string, Sprite[]>();

    public static T[] LoadAll<T>(string path) where T : UnityEngine.Object
    {
        // Если это спрайты - загружаем из папки mods
        if (typeof(T) == typeof(Sprite) && path == "skills" && UseModsFolder)
        {
            // Проверяем кэш
            if (_spriteArrayCache.ContainsKey(path))
                return _spriteArrayCache[path] as T[];

            // Полный путь к папке
            string folderPath = GetFullPath(path);

            if (!Directory.Exists(folderPath))
                return new T[0];

            // Получаем все PNG файлы в папке
            string[] files = Directory.GetFiles(folderPath, "*.png");
            List<Sprite> sprites = new List<Sprite>();

            foreach (string file in files)
            {
                Sprite sprite = LoadPNGFromFullPath(file);
                sprites?.Add(sprite);
            }

            Sprite[] result = sprites.ToArray();
            _spriteArrayCache[path] = result;
            return result as T[];
        }

        // Для остальных типов - стандартный Resources.LoadAll
        return Resources.LoadAll<T>(path);
    }

    public static T Load<T>(string path) where T : UnityEngine.Object
    {
        // Если это спрайт - загружаем из папки mods
        if (typeof(T) == typeof(Sprite) && UseModsFolder)
            return LoadPNG(path) as T;
        return Resources.Load<T>(path);
    }

    public static T GetBuiltinResource<T>(string path) where T : UnityEngine.Object
        => Resources.GetBuiltinResource<T>(path);

    /// <summary>
    /// Загружает PNG из относительного пути
    /// </summary>
    public static Sprite LoadPNG(string localPath)
    {
        // Нормализуем путь
        localPath = localPath.Replace('\\', '/');

        // Проверяем кэш
        if (_spriteCache.ContainsKey(localPath))
        {
            Debug.Log($"[ResourcesManager] Возвращаю из кэша спрайт: {localPath}");
            return _spriteCache[localPath];
        }

        // Формируем полный путь к файлу
        string fullPath = GetFullPath(localPath);
        Sprite sprite = LoadPNGFromFullPath(fullPath);

        if (sprite != null)
        {
            _spriteCache[localPath] = sprite;
        }

        return sprite;
    }

    /// <summary>
    /// Возвращает полный путь к файлу/папке в зависимости от UseModsFolder
    /// </summary>
    private static string GetFullPath(string relativePath)
    {
        return Path.Combine(Application.dataPath, "../mods", relativePath);
    }

    /// <summary>
    /// Загружает PNG из полного пути (внутренний метод)
    /// </summary>
    private static Sprite LoadPNGFromFullPath(string fullPath)
    {
        if (!File.Exists(fullPath))
        {
            Debug.LogError($"[ResourcesManager] Файл не найден: {fullPath}");
            return null;
        }

        string url = "file:///" + fullPath;

        using (WWW www = new WWW(url))
        {
            while (!www.isDone) { }

            if (string.IsNullOrEmpty(www.error))
            {
                Texture2D tex = www.texture;
                Sprite sprite = Sprite.Create(tex,
                    new Rect(0, 0, tex.width, tex.height),
                    new Vector2(0.5f, 0.5f));

                Debug.Log($"[ResourcesManager] Загружен спрайт: {Path.GetFileName(fullPath)} ({tex.width}x{tex.height})");
                return sprite;
            }

            Debug.LogError($"[ResourcesManager] Ошибка загрузки: {www.error}");
            return null;
        }
    }

    /// <summary>
    /// Очищает кэш спрайтов
    /// </summary>
    public static void ClearCache()
    {
        _spriteCache.Clear();
        _spriteArrayCache.Clear();
        Debug.Log("[ResourcesManager] Кэш спрайтов очищен");
    }

    /// <summary>
    /// Удаляет конкретный спрайт из кэша
    /// </summary>
    public static void RemoveFromCache(string localPath)
    {
        if (_spriteCache.ContainsKey(localPath))
        {
            _spriteCache.Remove(localPath);
            Debug.Log($"[ResourcesManager] Удален из кэша: {localPath}");
        }
    }
}