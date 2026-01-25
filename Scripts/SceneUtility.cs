using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class SceneUtility : MonoBehaviour
    {
        public static SceneUtility instance;
        public List<string> SceneBacklog;
        void Awake()
        {
            if (instance != null)
            {
                Debug.Log("Game has multiple Scene Utilities!");
                Destroy(this.gameObject);
            }
            
            else if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
        }
        void OnEnable()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }
        void OnSceneUnloaded(Scene current)
        {
            if (current.name.Contains("_"))
            {
                SceneBacklog.Add(current.name);
            }
            if (current == SceneManager.GetActiveScene())
            {
              //  LoaderUtility.Deinitialize();
               // LoaderUtility.Initialize();
            }
        }

        void OnDisable()
        {
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
    }