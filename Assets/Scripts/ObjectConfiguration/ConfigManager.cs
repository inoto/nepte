﻿using System.Collections;
using System.IO;
using UnityEngine;

public class ConfigManager : MonoBehaviour
{
	private static ConfigManager _instance;

	public static ConfigManager Instance { get { return _instance; } }

	public delegate void Config();
	public event Config OnConfigsLoaded = delegate { };
	
	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(this.gameObject);
		}
		else
		{
			_instance = this;
		}

//		savesPath = Application.persistentDataPath + System.IO.Path.DirectorySeparatorChar + "Saves" + System.IO.Path.DirectorySeparatorChar;
		configPath = Application.persistentDataPath + System.IO.Path.DirectorySeparatorChar + "Configs" + System.IO.Path.DirectorySeparatorChar;
		
		LoadConfigs();
	}

	public string configFileName = "tightspace-config.json";
	public string configURL = "https://raw.githubusercontent.com/inoto/inoto.github.io/master/";
	public string configPath;

	public string jsonData;
	
	public ConfigBase Base;
	public ConfigBase BaseTransit;
	public ConfigDrone Drone;
	public ConfigMothership Mothership;

	public ConfigBase GetBaseConfig(Planet bas)
	{
		switch (bas.Type)
		{
			case global::Planet.PlanetType.Normal:
				return Base;
			case global::Planet.PlanetType.Main:
				return Base;
			case global::Planet.PlanetType.Transit:
				return BaseTransit;
			default:
				return null;
		}
	}
	
	private void Start()
	{
		StartCoroutine(DownloadConfigFromWWW());
	}
	
	public bool LoadConfigs()
	{
		return LoadConfigsFromCacheFile(configFileName);
	}
	
	public bool LoadConfigsFromCacheFile(string fileName)
	{
		if (!File.Exists(configPath + configFileName))
			return false;
		string jsonFile = File.ReadAllText(configPath + configFileName);
		if (jsonFile != null)
		{
			JsonUtility.FromJsonOverwrite(jsonFile, this);
			jsonData = jsonFile;
//			Base = JsonUtility.FromJson<ConfigBase>(jsonFile);
//			BaseTransit = JsonUtility.FromJsonOverwrite(jsonFile);
//			Drone = JsonUtility.FromJson<ConfigDrone>(jsonFile);
			return true;
		}
		else
		{
			Debug.LogError("Config file was not found in cache");
			return false;
		}
	}
	
	public bool LoadConfigsFromString(string text)
	{
		if (text != null)
		{
//			Debug.Log(text);
			JsonUtility.FromJsonOverwrite(text, this);
			jsonData = text;
//			Base = JsonUtility.FromJson<ConfigBase>(text);
//			BaseTransit = JsonUtility.FromJson<ConfigBase>(text);
//			Drone = JsonUtility.FromJson<ConfigDrone>(text);
			return true;
		}
		else
		{
			Debug.LogError("Config file was not found in string");
			return false;
		}
	}

	private IEnumerator DownloadConfigFromWWW()
	{
		WWW www = new WWW(configURL + configFileName);
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			// www.text to config
			Debug.Log("Config has overwritten from WWW");
			LoadConfigsFromString(www.text);
			// save downloaded file as cache
			if (!Directory.Exists(configPath))
				Directory.CreateDirectory(configPath);
			File.WriteAllText(configPath + configFileName, www.text);
		}
		else
		{
			Debug.LogError(www.error);
		}
		OnConfigsLoaded();
		Debug.Log(jsonData);
	}
}