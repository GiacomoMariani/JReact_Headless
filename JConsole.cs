using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JReact
{
    /// <summary>
    /// displays a message in the console
    /// </summary>
    public static class JConsole
    {
        #region MAIN LOGGERS
        /// <summary>
        /// displays a message in the console
        /// </summary>
        /// <param name="message">the message to be logged</param>
        /// <param name="context"> the related context</param>
        /// <param name="consoleTag">a tag useful for console pro</param>
        public static void Log(string message, string consoleTag = "", Object context = null)
        {
#if UNITY_EDITOR
            Debug.Log($"{DateTime.Now:[HH:mm:ss]} {consoleTag} {message}", context);
#endif
#if !UNITY_EDITOR
			Debug.Log(DateTime.Now.ToString("[HH:mm:ss]") + "[" + consoleTag + "] - " + message);
#endif
        }

        /// <summary>
        /// displays a warning in the console, it may differ based on the platform
        /// </summary>
        /// <param name="message">the message to be logged</param>
        /// <param name="context"> the related context</param>
        /// <param name="consoleTag">a tag useful for console pro</param>
        public static void Warning(string message, string consoleTag = "", Object context = null)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"{DateTime.Now:[HH:mm:ss]} {consoleTag} {message}", context);
#endif
#if !UNITY_EDITOR
			Debug.LogWarning(DateTime.Now.ToString("[HH:mm:ss]") + "[" + consoleTag + "] - " + message);
#endif
        }

        /// <summary>
        /// displays an error in the console, it may differ based on the platform
        /// </summary>
        /// <param name="message">the message to be logged</param>
        /// <param name="context"> the related context</param>
        /// <param name="consoleTag">a tag useful for console pro</param>
        public static void Error(string message, string consoleTag = "", Object context = null)
        {
#if UNITY_EDITOR
            Debug.LogError($"{DateTime.Now:[HH:mm:ss]} {consoleTag} {message}", context);
#endif
#if !UNITY_EDITOR
			Debug.LogError(DateTime.Now.ToString("[HH:mm:ss]") + "[" + consoleTag + "] - " + message);
#endif
        }
        
        /// <summary>
        /// an error message with a breacn
        /// </summary>
        /// <param name="message">the message to be logged</param>
        /// <param name="context"> the related context</param>
        /// <param name="consoleTag">a tag useful for console pro</param>
        public static void Break(string message, string consoleTag = "", Object context = null)
        {
#if UNITY_EDITOR
            Debug.LogError($"{DateTime.Now:[HH:mm:ss]} {consoleTag} {message}", context);
            Debug.Break();
#endif
#if !UNITY_EDITOR
			Debug.LogError(DateTime.Now.ToString("[HH:mm:ss]") + "[" + consoleTag + "] - " + message);
            Debug.Break();
#endif
        }
        #endregion

        #region REMINDERS
        /// <summary>
        /// Used to remember the to do list
        /// </summary>
        /// <param name="workOnThis">To find the class that needs this.</param>
        /// <param name="message">A comment of description.</param>
        public static void RememberToDo(string message, object workOnThis)
        {
#if UNITY_EDITOR
            Debug.Log($"#TO DO#\n{workOnThis.GetType()} needs to be completed.\n Task: {message}");
#endif
        }
        #endregion

        #region HELPER LOGS
        /// <summary>
        /// Used to store quick reminders
        /// </summary>
        /// <param name="message">A comment of description.</param>
        /// <param name="context"> the related context</param>
        public static void QuickLog(string message, Object context = null) { Log(message, JLogTags.QuickLogTag, context); }
        #endregion
    }
}
