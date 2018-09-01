using UnityEngine;

namespace JReact
{
    /// <summary>
    /// this is a singleton used to debug elements in a specific way
    /// </summary>
    public static class HelperConsole
    {
        #region MAIN LOGGERS
        /// <summary>
        /// this message display a message in the console
        /// </summary>
        /// <param name="message">the message to be logged</param>
        public static void DisplayMessage(string message, string consoleTag = "")
        {
#if UNITY_EDITOR
            Debug.Log(consoleTag + message);
#endif
#if !UNITY_EDITOR
			Debug.Log(consoleTag + message);
#endif
        }

        /// <summary>
        /// this message display a warning in the console, it may differ based on the platform
        /// </summary>
        /// <param name="message">the message to be logged</param>
        public static void DisplayWarning(string message, string consoleTag = "")
        {
#if UNITY_EDITOR
            Debug.LogWarning(consoleTag + message);
#endif
#if !UNITY_EDITOR
			Debug.LogWarning(consoleTag + message);
#endif
        }

        /// <summary>
        /// this message display an error in the console, it may differ based on the platform
        /// </summary>
        /// <param name="message">the message to be logged</param>
        public static void DisplayError(string message)
        {
#if UNITY_EDITOR
            Debug.LogError(message);
#endif
#if !UNITY_EDITOR
			Debug.LogWarning(message);
#endif
        }
        #endregion

        #region REMINDERS
        /// <summary>
        /// Used to remember the to do list
        /// </summary>
        /// <param name="workOnThis">To find the class that needs this.</param>
        /// <param name="message">A comment of description.</param>
        public static void RememberToDo(object workOnThis, string message)
        {
#if UNITY_EDITOR
            Debug.Log(string.Format("#TO DO#\n{0} needs to be completed.\n Task: {1}", workOnThis.GetType(), message));
#endif
        }
        #endregion

        #region HELPER LOGS
        /// <summary>
        /// Used to store quick reminders
        /// </summary>
        /// <param name="message">A comment of description.</param>
        public static void QuickLog(string message = "")
        {
            DisplayMessage(message, J_DebugConstants.QuickCheckLogsTag);
        }

        /// <summary>
        /// this is used to display important messages on the console
        /// </summary>
        /// <param name="message">the message we want to display</param>
        public static void DisplayImportantMessage(string message) { DisplayMessage(message, J_DebugConstants.HighlightLogTag); }
        #endregion
    }
}
