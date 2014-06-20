using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;

public class StationLoader : MonoBehaviour {

	// Constant URI for retrieving all stations
	const string STATIONS_URI = "http://localhost:8080/api/stations";

	public static JSONObject[] stations;

	public static string[] stationNames;

	public static GUIContent[] stationContent;

	IEnumerator DownloadStations() {

		WWW w = new WWW (STATIONS_URI);
		yield return w;

		Debug.Log ("Waiting for stations\n");

		// Add a wait to make sure we have the stations

		yield return new WaitForSeconds(1f);

		Debug.Log ("Recieved Stations\n");

		ExtractStations (w.text);
	}

	// Use this for initialization
	void Start () {
		Debug.Log ("Start Stations download\n");

		StartCoroutine (DownloadStations());
	}

	// Extract all the Stations from JSON
	void ExtractStations(string json)
	{
		JSONObject jo = new JSONObject (json);

		// Our outer object is an array
		
		if (jo.type != JSONObject.Type.ARRAY)
			return;

		// Initialize the array of 
		stations = new JSONObject[jo.list.Count];
		stationNames = new string[jo.list.Count];
		stationContent = new GUIContent[jo.list.Count];
		int index = 0;
		foreach (JSONObject item in jo.list) {
			stations[index] = item;
			stationNames[index] = item.GetField("name").str;
			stationContent[index] = new GUIContent(stationNames[index]);
			index++;
		}
	}

	public static JSONObject[] getStations() {
		return stations;
	}

	public static string[] getStationNames() {
		return stationNames;
	}

	public static GUIContent[] getStationGUIContent() {
		return stationContent;
	}

	// Update is called once per frame
	void Update () {
	
	}
			
}
