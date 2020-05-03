using UnityEngine;
using System.Collections;
/// <summary>
/// A generic singleton class for creating singleton
/// </summary>
/// <typeparam name="T">The type of MonoBehaviour the singleton needs to be</typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// The instance of the singleton
    /// </summary>
    private static T _instance;

    /// <summary>
    /// Property for accessing the singleton
    /// </summary>
    public static T Instance
    {
        get
        {
            if (!_instance) //If the instance is null then we need to find it
            {
                //Finds the object
                _instance = FindObjectOfType<T>();
            }
            //Returns the instance
            return _instance;
        }
    }
}