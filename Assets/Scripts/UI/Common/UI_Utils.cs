/*
 * Date: November 29th, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters
{
	public class UI_Utils
	{
		public static string GetFormatTime(int secs)
		{
			var hrs = Mathf.FloorToInt(secs / 60 / 60);

			var mins = Mathf.FloorToInt(secs / 60);
			mins = mins - hrs * 60;

			secs = Mathf.CeilToInt(secs - mins * 60 - hrs * 60 * 60);

			secs = Mathf.Max(0, secs);

			// Return only seconds
			if (hrs <= 0 && mins <= 0) return $"{secs:00}s";

			// Return mins and seconds
			if (hrs <= 0) return $"{mins:00}m {secs:00}s";

			return $"{hrs:00}h {mins:00}m {secs:00}s";
		}
	}
}