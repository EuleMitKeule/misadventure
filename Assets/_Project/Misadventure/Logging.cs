namespace HotlineHyrule
{
    using System.Diagnostics;

    public static class Logging
    {
        [Conditional("DEBUG")]
        public static void LogInfo(string message, UnityEngine.Object obj = null)
        {
            UnityEngine.Debug.Log(message, obj);
        }

        [Conditional("DEBUG")]
        public static void Log(string message, UnityEngine.Object obj = null)
        {
            UnityEngine.Debug.Log(message, obj);
        }

        [Conditional("DEBUG")]
        public static void LogWarning(string message, UnityEngine.Object obj = null)
        {
            UnityEngine.Debug.LogWarning(message, obj);
        }

        [Conditional("DEBUG")]
        public static void LogError(string message, UnityEngine.Object obj = null)
        {
            UnityEngine.Debug.LogError(message, obj);
        }

        [Conditional("DEBUG")]
        public static void LogException(System.Exception e)
        {
            UnityEngine.Debug.LogError(e.Message);
        }
    }
}