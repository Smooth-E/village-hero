using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ScriptableObjects
{
    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
    {

        private static T _instance;

        public static T Instance
        {
            get 
            {
                if (_instance == null)
                {
                    var assets = Resources.LoadAll<T>("");
                    
                    if (assets == null || assets.Length < 1)
                        throw new ApplicationException($"Cannot find any {typeof(T).Name} assets!");

                    if (assets.Length > 1)
                    {
                        var message = $"More than one SingletonScriptableObject asset created for {typeof(T).Name}";
                        throw new ApplicationException(message);
                    }

                    _instance = assets[0];
                }
                
                return _instance;                 
            }
        }
        
    }
}
