using System;
using System.Collections.Generic;

/// <summary>
/// このクラスでは、様々なサービスを登録し、取得するためのシンプルなサービスロケーターパターンを実装しています。
/// </summary>
public static class ServiceLocator
{
    public static Dictionary<Type, object> services = new Dictionary<Type, object>();

    public static void Register<T>(T service)
    {
        services[typeof(T)] = service;
    }

    public static T Get<T>()
    {
        if (services.TryGetValue(typeof(T), out var service))
        {
            return (T)service;
        }
        throw new Exception($"Service of type {typeof(T)} not found.");
    }
}