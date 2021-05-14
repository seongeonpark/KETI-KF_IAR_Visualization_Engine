using System.Collections;
using UnityEngine;

[System.Serializable]
public struct IoT
{
    public float Min, Max;

    public IoT(float min = 0, float max = 100)
    {
        this.Min = min;
        this.Max = max;
    }
}

public class VirtualServer : MonoBehaviour
{
    #region Inspector Window

    //public List<IoT> IoTs = new List<IoT>();
    [Space]
    [Header("Input Data")]
    [SerializeField] private int RepeatTime;
    [SerializeField] private IoT[] IoTs;

    [Space]
    [Header("Output Data")]
    public float[] IoTData;
    #endregion

    private void Start()
    {
        IoTData = new float[IoTs.Length];
        
        StartCoroutine("GetRandomData");
    }

    private IEnumerator GetRandomData()
    {
        while (true)
        {
            for (int i = 0; i < IoTs.Length; i++)
            {
                IoTData[i] = Random.Range(IoTs[i].Min, IoTs[i].Max);
            }

            yield return new WaitForSeconds(RepeatTime);
        }
    }
}
