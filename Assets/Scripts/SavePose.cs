using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class SavePose : MonoBehaviour
{
    public Transform gunParent;
    public Transform gunLeftGrip;
    public Transform gunRightGrip;

    Gun equipGun;

    [ContextMenu("Save gun pose")]
    void SaveGunPose()
    {
        equipGun = GetComponentInChildren<Gun>();

        GameObjectRecorder recorder = new GameObjectRecorder(gameObject);
        recorder.BindComponentsOfType<Transform>(gunParent.gameObject, false);
        recorder.BindComponentsOfType<Transform>(gunLeftGrip.gameObject, false);
        recorder.BindComponentsOfType<Transform>(gunRightGrip.gameObject, false);
        recorder.TakeSnapshot(0.0f);
        recorder.SaveToClip(equipGun.gunAnimation);
        UnityEditor.AssetDatabase.SaveAssets();
    }
}
