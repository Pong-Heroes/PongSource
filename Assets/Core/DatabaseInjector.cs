using System;
using UnityEngine;

public static class DatabaseInjector
{
    private static bool _isInjected = false;

    [RuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RuntimeInjection()
    {
        if (_isInjected) return;

        Debug.Log("---> Injecting Databases...");
        _isInjected = true;
        Databases.CharactersDatabase = GetCharactersDatabase();
    }
    /// <summary>
    /// Load -- Cache the Characters Database and Get active instance
    /// </summary>
    /// <returns>Characters Database Asset Refernece</returns>
    private static CharactersDatabase GetCharactersDatabase()
    {
        var Database = Resources.Load<CharactersDatabase>(nameof(CharactersDatabase));
        Database.Init();
        return Database;
    }
}
