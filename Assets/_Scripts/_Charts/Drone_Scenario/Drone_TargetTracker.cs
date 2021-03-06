using UnityEngine;
using System;

namespace Vuforia
{

    public class Drone_TargetTracker : MonoBehaviour, ITrackableEventHandler
    {
        private TrackableBehaviour mTrackableBehaviour;
        public event Action OnTrackingFound = () => { };
        public event Action OnTrackingLost = () => { };

        void Start()
        {
            mTrackableBehaviour = GetComponent<TrackableBehaviour>();
            if (mTrackableBehaviour)
            {
                mTrackableBehaviour.RegisterTrackableEventHandler(this);
            }
        }

        public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
        {
            print("Test");
            if (newStatus == TrackableBehaviour.Status.DETECTED || newStatus == TrackableBehaviour.Status.TRACKED ||
                       newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
            {
                print("TargetTracker : onTrackingFound");
                OnTrackingFound();
            }
            else
            {
                print("TargetTracker : onTrackingLost");
                OnTrackingLost();
            }
        }
    }
}

