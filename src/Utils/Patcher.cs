using System;
using System.Reflection;
using HarmonyLib;
using TerrariaModder.Core.Logging;

namespace Utils;

public class Patcher(Harmony harmony, ILogger logger = null)
{
    private const BindingFlags DefaultFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    private readonly Harmony _harmony = harmony ?? throw new ArgumentNullException(nameof(harmony));
    private readonly ILogger _logger = logger;

    public bool Patch(Type type, string methodName, BindingFlags flags = DefaultFlags, Type[] paramTypes = null, HarmonyMethod prefix = null, HarmonyMethod postfix = null)
    {
        if (type is null || string.IsNullOrEmpty(methodName))
        {
            _logger?.Error($"Patcher: Patch called with invalid arguments: type={type}, methodName={methodName}");
            return false;
        }

        var method = FindMethod(type, methodName, flags, paramTypes);
        if (method == null)
        {
            _logger?.Error($"Patcher: could not find method '{methodName}' on type '{type?.FullName}'");
            return false;
        }

        try
        {
            _harmony.Patch(method, prefix, postfix);
            _logger?.Info($"Patcher: successfully patched '{type.FullName}.{methodName}'");
            return true;
        }
        catch (Exception ex)
        {
            _logger?.Error($"Patcher: failed to patch '{type.FullName}.{methodName}': {ex.Message}");
            return false;
        }
    }

    public static HarmonyMethod GetHarmonyMethod(Type type, string methodName)
    {
        if (string.IsNullOrEmpty(methodName))
            return null;

        var methodInfo = type.GetMethod(methodName, DefaultFlags);
        return methodInfo is null ? null : new HarmonyMethod(methodInfo);
    }

    private MethodInfo FindMethod(Type type, string methodName, BindingFlags flags = DefaultFlags, Type[] paramTypes = null)
    {
        try
        {
            return paramTypes == null
                ? type.GetMethod(methodName, flags)
                : type.GetMethod(methodName, flags, null, paramTypes, null);
        }
        catch (Exception ex)
        {
            _logger?.Error($"Error while finding method '{methodName}' on type '{type?.FullName}': {ex.Message}");
            return null;
        }
    }
}
