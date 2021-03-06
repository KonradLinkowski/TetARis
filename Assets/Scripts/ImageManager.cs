﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

namespace TetARis.Core {
  public class ImageManager : MonoBehaviour, ITrackableEventHandler
  {

    private TrackableBehaviour mTrackableBehaviour;
 
    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }
    public void OnTrackableStateChanged(
      TrackableBehaviour.Status previousStatus,
      TrackableBehaviour.Status newStatus)
    {
      if (newStatus == TrackableBehaviour.Status.DETECTED ||
        newStatus == TrackableBehaviour.Status.TRACKED ||
        newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) {
          BoardManager.Instance.gameObject.SetActive(true);
        BoardManager.Instance.undetected = false;
      } else {  
        BoardManager.Instance.undetected = true;
        BoardManager.Instance.gameObject.SetActive(false);
      }
    }
  }
}
