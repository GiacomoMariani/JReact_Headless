using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.CloudBuild
{
    public static class J_CloudBuildData
    {
        #region FIELDS AND PROPERTIES
        //the file manifest
        private static TextAsset _manifest;
        //the dictionary of the manifest
        private static Dictionary<string, object> _manifestDictionary;

        // --------------- CONSTANTS --------------- //
        private const string ManifestNameTxt = "UnityCloudBuildManifest.json";

        private const string VersionNumber = "buildNumber";
        #endregion

        #region SETUP
        //setup and check manifest and dictionary
        private static void GetManifest()
        {
            _manifest = (TextAsset) Resources.Load(ManifestNameTxt);
            Assert.IsNotNull(_manifest, $"Retrieved no manifest file from resource file: {ManifestNameTxt}");
            _manifestDictionary = JsonUtility.FromJson<Dictionary<string, object>>(_manifest.text);
            Assert.IsNotNull(_manifestDictionary, "no Dictionary found for manifest");
        }

        //gets a key
        private static string GetKeyFromManifest(string key)
        {
            //key without a value sends a warning
            if (_manifestDictionary.ContainsKey(key))
            {
                JLog.Warning($"No key -{key}- found on manifest", JLogTags.Build);
                return "";
            }

            //return the given key
            return _manifestDictionary[key].ToString();
        }
        #endregion

        #region PRINT ALL COMMAND
        //print all the values of the manifest
        public static void PrintAllManifestValues()
        {
            if (_manifest == null) GetManifest();
            if (_manifest == null) return;

            JLog.Log($"Build Manifest with {_manifestDictionary.Count} kvp", JLogTags.Build);
            foreach (KeyValuePair<string, object> kvp in _manifestDictionary)
            {
                string value = kvp.Value != null
                                   ? kvp.Value.ToString()
                                   : "";

                JLog.Log($"Key: {kvp.Key}, Value: {value}", JLogTags.Build);
            }
        }
        #endregion

        #region DATA GETTERS
        /// <summary>
        /// gets the cloud build data
        /// </summary>
        /// <returns>returns the version of the cloud build</returns>
        public static string GetCloudBuildVersion()
        {
            if (_manifest == null) GetManifest();
            if (_manifest == null) return "";

            return GetKeyFromManifest(VersionNumber);
        }
        #endregion
    }
}
