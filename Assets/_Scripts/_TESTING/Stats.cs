using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats
{
    public enum Item
    {
        A, B, C, D, E
    }

    public const int STAT_MIN = 0;
    public const int STAT_MAX = 20;

    private SingleStat A, B, C, D, E;


    public Stats(int aAmount, int bAmount, int cAmount, int dAmount, int eAmount)
    {
        A = new SingleStat(aAmount);
        B = new SingleStat(bAmount);
        C = new SingleStat(cAmount);
        D = new SingleStat(dAmount);
        E = new SingleStat(eAmount);
    }
    
    public class SingleStat
    {
        private int _mStat;

        public SingleStat(int statAmount)
        {
            Stat = statAmount;
        }

        public int Stat
        {
            get { return _mStat; }

            set { _mStat = Mathf.Clamp(value, STAT_MIN, STAT_MAX); }
        }

        public float GetStatNormalized()
        {
            return (float)_mStat / STAT_MAX;
        }
    }
}
