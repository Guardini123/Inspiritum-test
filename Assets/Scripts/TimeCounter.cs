using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text _counterText;
    private float _time = 0.0f;
    private bool _count = false;


	private void Start()
	{
		Player.Instance.OnPlayerStart.AddListener(() => { _count = true; });
	}


	void Update()
    {
		if (_count)
		{
            _time += Time.deltaTime;
		}
		_counterText.text = _time.ToString("0.00") + " sec";
    }
}
