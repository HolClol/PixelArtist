using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassContainer { }

#region JUNK STUFF
[System.Serializable] public class FillLine
{
    public List<FillHoleController> groupFillHoles;
    public List<Vector3> groupPos;

    public FillLine(List<FillHoleController> hole, List<Vector3> groupPos)
    {
        groupFillHoles = hole;
        this.groupPos = groupPos;
    }
}
#endregion
public class BlockInfo
{
    public MeshCollider Collider;
    public PeopleController Controller;

    public BlockInfo(MeshCollider collider, PeopleController controller)
    {
        Collider = collider;
        Controller = controller;
    }
}

[System.Serializable] public class AudioSetting
{
    public float volume = 1f;
    public float pitch = 1f;
    public bool loop = false;
}

[System.Serializable] public class AudioFile
{
    public AudioClip clip;
    public AudioSetting setting;

    public AudioFile(AudioClip clip, AudioSetting setting)
    {
        this.clip = clip;
        this.setting = setting;
    }
}

[System.Serializable] public class AudioTypeTable
{
    public string AudioTypeName;
    public AudioTypeEnum AudioType;
    public List<AudioFile> AudioFiles;

    public AudioTypeTable(string name, AudioTypeEnum audioType, List<AudioFile> audioFiles)
    {
        this.AudioTypeName = name;
        this.AudioType = audioType;
        this.AudioFiles = audioFiles;
    }
}
