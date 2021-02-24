using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TImeProgress : MonoBehaviour
{
    private Slider slider;
	private float targetProcess = 10;
	
	private void Awake() {
		slider = gameObject.GetComponent<Slider>();
	}
	

	public void IncrementProgress(float duration) {
		targetProcess = slider.value - duration;
	}
}
