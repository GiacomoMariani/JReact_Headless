﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace JReact
{
    public static class JExtensions
    {
        #region CONSTANT VALUES
        private const string ScriptableObjectSuffix = "_ScriptableObject";

        private const string DaysHoursMinutesSeconds = "{0:D2}d:{1:D2}h:{2:D2}m:{3:D2}s";
        private const string HoursMinutesSeconds = "{0:D2}h:{1:D2}m:{2:D2}s";
        private const string MinutesSeconds = "{0:D2}m:{1:D2}s";
        private const string Seconds = "{0:D2}s";

        private const string DaysHoursMinutesSecondsWithoutFormat = "{0:D2}:{1:D2}:{2:D2}:{3:D2}";
        private const string HoursMinutesSecondsWithoutFormat = "{0:D2}:{1:D2}:{2:D2}";
        private const string MinutesSecondsWithoutFormat = "{0:D2}:{1:D2}";
        #endregion

        #region INT
        //an extension that tries to convert a string into int
        public static int ToInt(this string stringToConvert)
        {
            //try getting the value
            int valueToReturn;
            if (int.TryParse(stringToConvert, out valueToReturn)) return valueToReturn;

            //otherwise send a warning and return 0
            Debug.LogWarning(string.Format("The string '{0}' cannot be converted into integere. Returning 0.", stringToConvert));
            return 0;
        }
        #endregion

        #region FLOAT
        //used to convert a float value into time string
        public static string ToTimeString(this float valueToConvert, bool withFormat = true)
        {
            //get the time span from the value
            TimeSpan time = TimeSpan.FromSeconds(valueToConvert);
            //initiate the string
            string timeWithFormat = "";
            //TIME WITH DAY
            if (time.Days > 0)
                timeWithFormat = string.Format(withFormat
                                                   ? DaysHoursMinutesSeconds
                                                   : DaysHoursMinutesSecondsWithoutFormat, time.Days, time.Hours, time.Minutes,
                                               time.Seconds);
            //TIME WITH HOURS
            else if (time.Hours > 0)
                timeWithFormat = string.Format(withFormat
                                                   ? HoursMinutesSeconds
                                                   : HoursMinutesSecondsWithoutFormat, time.Hours, time.Minutes, time.Seconds);
            //TIME WITH MINUTES
            else
                timeWithFormat = string.Format(withFormat
                                                   ? MinutesSeconds
                                                   : MinutesSecondsWithoutFormat, time.Minutes, time.Seconds);
            //returns the given string
            return timeWithFormat;
        }
        #endregion

        #region COLLECTIONS
        /// <summary>
        /// a way to debug all elements in an enumerable
        /// </summary>
        /// <param name="collection">the sequence to check</param>
        /// <typeparam name="T">the type of element in the collection</typeparam>
        /// <returns>returns the collection as a string</returns>
        public static string PrintAll<T>(this ICollection<T> collection)
        {
            string printedElements = "Elements: - ";
            foreach (var element in collection) { printedElements += (element + " - "); }

            return printedElements;
        }

        /// <summary>
        /// get a random element in the collection
        /// </summary>
        /// <param name="collection">the sequence to check</param>
        /// <typeparam name="T">the type of element in the collection</typeparam>
        /// <returns>returns a random element</returns>
        public static T GetRandomElement<T>(this ICollection<T> collection)
        {
            return collection.ElementAt(Random.Range(0, collection.Count));
        }

        /// <summary>
        /// get a random element in the collection
        /// </summary>
        /// <param name="collection">the sequence to check</param>
        /// <typeparam name="T">the type of element in the collection</typeparam>
        /// <returns>returns a random element</returns>
        public static void SubscribeToAll<T>(this ICollection<T> collection, JAction actionToPerform)
            where T : iBaseObservable
        {
            foreach (var element in collection) { element.Subscribe(actionToPerform); }
        }

        public static void UnSubscribeToAll<T>(this ICollection<T> collection, JAction actionToPerform)
            where T : iBaseObservable
        {
            foreach (var element in collection) { element.UnSubscribe(actionToPerform); }
        }

        public static void SubscribeToAll<T>(this ICollection<iObservable<T>> collection, JGenericDelegate<T> actionToPerform)
        {
            foreach (var element in collection) { element.Subscribe(actionToPerform); }
        }

        public static void UnSubscribeToAll<T>(this ICollection<iObservable<T>> collection, JGenericDelegate<T> actionToPerform)
        {
            foreach (var element in collection) { element.UnSubscribe(actionToPerform); }
        }
        
        //reset 
        public static void ResetAll(ICollection<iResettable> collection)
        {
            foreach (var element in collection) { element.ResetThis(); }
        }
        #endregion

        #region SCRIPTABLE OBJECTS
        //a way to set the names of scriptable object
        public static void SetName(this ScriptableObject itemToRename, string newName)
        {
            itemToRename.name = newName + ScriptableObjectSuffix;
        }
        #endregion

        #region TRANSFORMS
        //used to clear a transform from all of its children
        public static void ClearTransform(this Transform transform)
        {
            foreach (Transform child in transform) { GameObject.Destroy(child.gameObject); }
        }
        #endregion

        #region GAMEOBJECTS
        /// <summary>
        /// a method to check if a gameobject has a component
        /// </summary>
        /// <param name="component"></param>
        /// <param name="gameObjectToCheck"></param>
        public static void CheckComponent(this GameObject gameObjectToCheck, Type component)
        {
            //check one component ont he weapon
            var elementSearched = gameObjectToCheck.GetComponents(component);

            //check if we have at least a component
            Assert.IsFalse(elementSearched.Length == 0,
                           string.Format("There is no {0} components on {1}", component, gameObjectToCheck.name));

            //check that we have just one component
            Assert.IsFalse(elementSearched.Length > 1,
                           string.Format("There are too many components of {0} on {1}", component, gameObjectToCheck.name));
            //check that the component is of the specified class
            if (elementSearched.Length > 0)
            {
                Assert.IsTrue(elementSearched[0].GetType() == component.GetElementType(),
                              string.Format("The class requested is of a parent class. Weapon {0}, class found {1}, class requested {2}. Player {3}",
                                            gameObjectToCheck, elementSearched[0].GetType(), component.GetElementType(),
                                            gameObjectToCheck.transform.root.gameObject));
            }
        }
        #endregion

        #region VECTORS
        /// <summary>
        /// used to have a random float value between 2 data given in a Vector2
        /// </summary>
        /// <param name="range">x is the min and y is the max</param>
        /// <returns></returns>
        public static float GetRandomValue(this Vector2 range)
        {
            Assert.IsTrue(range.x <= range.y,
                          string.Format("The y value of the given range needs to be higher than x. X = {0}, Y = {1}", range.x,
                                        range.y));
            return Random.Range(range.x, range.y);
        }

        /// <summary>
        /// Gets a random value between vector.x and vector.y
        /// </summary>
        /// <param name="rangeInt">the vector with the min and max</param>
        public static int RandomValue(this Vector2Int rangeInt)
        {
            Assert.IsTrue(rangeInt.x <= rangeInt.y,
                          string.Format("The y value of the given range needs to be higher than x. X = {0}, Y = {1}", rangeInt.x,
                                        rangeInt.y));
            return UnityEngine.Random.Range(rangeInt.x, rangeInt.y);
        }
        #endregion

        #region STRING
        /// <summary>
        /// this is used to encode a string into a hex string
        /// </summary>
        /// <param name="str">the string to encode</param>
        /// <returns>returns the hex string</returns>
        public static string ToHexString(this string str)
        {
            var sb = new StringBuilder();

            var bytes = Encoding.Unicode.GetBytes(str);
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString();
        }
        #endregion

        #region DATE TIME
        /// <summary>
        /// this is used to calculate the seconds passed between 2 date times
        /// </summary>
        /// <param name="currentTime">the current date time</param>
        /// <param name="previousTime">the time passed from the previous time</param>
        /// <returns>returns the seconds passed in this time interval</returns>
        public static double CalculateSecondsFrom(this DateTime currentTime, DateTime previousTime)
        {
            //calculate the time passed
            var passedTime = currentTime.Subtract(previousTime);
            //return the seconds passed
            return passedTime.TotalSeconds;
        }
        #endregion

        #region 2D
        /// <summary>
        /// used to set a transparency on a given sprite renderer
        /// </summary>
        /// <param name="spriteRenderer">the sprite renderer to adjust</param>
        /// <param name="transparency">the transparency we want to set</param>
        public static void SetTransparency(this SpriteRenderer spriteRenderer, float transparency)
        {
            //sanity check
            Assert.IsTrue(transparency >= 0f && transparency <= 1.0f,
                          string.Format("The transparency to be set on {0} should be between 0 and 1. Received value: {1}",
                                        spriteRenderer.gameObject.name, transparency));
            //clamp the value
            transparency = Mathf.Clamp(transparency, 0f, 1f);
            //get the color
            var fullColor = spriteRenderer.color;
            //set the color with transparency
            spriteRenderer.color = new Color(fullColor.r, fullColor.g, fullColor.b, transparency);
        }

        /// <summary>
        /// used to set a transparency on a given image
        /// </summary>
        /// <param name="image">the image to adjust</param>
        /// <param name="transparency">the transparency we want to set</param>
        public static void SetTransparency(this Image image, float transparency)
        {
            //sanity check
            Assert.IsTrue(transparency >= 0f && transparency <= 1.0f,
                          string.Format("The transparency to be set on {0} should be between 0 and 1. Received value: {1}",
                                        image.gameObject.name, transparency));
            //clamp the value
            transparency = Mathf.Clamp(transparency, 0f, 1f);
            //get the color
            var fullColor = image.color;
            //set the color with transparency
            image.color = new Color(fullColor.r, fullColor.g, fullColor.b, transparency);
        }
        #endregion
    }
}
