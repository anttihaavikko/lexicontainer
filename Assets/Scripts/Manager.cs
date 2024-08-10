using System;
using System.Globalization;
using UnityEngine;

public class Manager : MonoBehaviour {
	private static Manager instance = null;
	public static Manager Instance => instance;
	
	public int Seed { get; set; }
	public DateTime Day { get; set; } = DateTime.Today;
	
	public string DailyString => Day.ToString("MMM dd yyyy", new CultureInfo("en-US"));

	void Awake()
	{
		if (instance != null && instance != this) {
			Destroy (this.gameObject);
			return;
		}

		instance = this;
		DontDestroyOnLoad(this);
	}
}
