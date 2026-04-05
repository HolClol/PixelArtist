using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public GameObject audioPlayerPrefab;
    public List<AudioTypeTable> AudioList;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayAudio(AudioTypeEnum audioType, Transform spawnTransform)
    {
        if (AudioList.Exists(x => x.AudioType == audioType))
        {
            AudioTypeTable audioTable = AudioList.Find(x => x.AudioType == audioType);
            AudioFile audioFile = audioTable.AudioFiles[Random.Range(0, audioTable.AudioFiles.Count)];
            
            GameObject audioPlayer = Pooling.Spawn("AudioPlayer", audioPlayerPrefab, "");
            audioPlayer.transform.position = spawnTransform.position;
            audioPlayer.SetActive(true);
            
            AudioSource audioSource = audioPlayer.GetComponent<AudioSource>();
            audioSource.clip = audioFile.clip;
            audioSource.volume = audioFile.setting.volume;
            audioSource.pitch = audioFile.setting.pitch;
            audioSource.loop = audioFile.setting.loop;
            audioSource.Play();

            if (audioFile.setting.loop) return;

            float length = audioSource.clip.length;
            FunctionManager.Instance.DelayFunction(length, () =>
            { 
                audioSource.Stop();
                audioPlayer.SetActive(false);
                Pooling.Despawn("AudioPlayer", audioPlayer);
            });

        }
    }
}
