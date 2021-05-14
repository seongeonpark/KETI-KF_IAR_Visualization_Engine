using TMPro;
using UnityEngine;

public class LoadingAnimation : MonoBehaviour
{
    [SerializeField] private GetTimeSeries_v0_2 WindowGraphManager;
    private Transform LoadingCircle;
    public TextMeshProUGUI TextLoading;
    
    [SerializeField] private float _speed = 150f;
    private float _mRepeatTime = 5f;
    private float _mZAngle = 0f;
    private float _mElapsedTime = 0f;

    private void Awake()
    {
        _mRepeatTime = WindowGraphManager.repeatTime;
        LoadingCircle = gameObject.transform;
    }

    void Update()
    {
        if (_mElapsedTime <= _mRepeatTime)
        {
            _mElapsedTime += Time.deltaTime;
            _mZAngle += _speed * Time.deltaTime;
            LoadingCircle.eulerAngles = new Vector3(0, 0, _mZAngle);
        }
        else
        {
            LoadingCircle.gameObject.SetActive(false);
            TextLoading.gameObject.SetActive(false);
        }
    }
}
