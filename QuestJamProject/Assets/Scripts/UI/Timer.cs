using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private TextMeshProUGUI _timer;
    public int _milli, _second;
    public bool _isTiming = true;

    void Awake()
    {
        _timer = GetComponent<TextMeshProUGUI>();
        StartCoroutine(timing());
    }

    void Update()
    {
        if (_milli < 10)
        {
            _timer.SetText(_second.ToString() + "." + "0" + _milli.ToString());
        }
        else
        {
            _timer.SetText(_second.ToString() + "." + _milli.ToString());
        }
    }

    IEnumerator timing()
    {
        yield return new WaitUntil(() => _isTiming);
        while (_milli != 99 && _isTiming == true)
        {
            yield return new WaitForSeconds(0.01f);
            _milli += 1;
        }
        if (_isTiming)
        {
            _milli = 0;
            _second += 1;
            StartCoroutine(timing());
        }
    }
}
