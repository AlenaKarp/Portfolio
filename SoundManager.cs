using System.Collections;
using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine;


//This is part of full code used in the project 
public class SoundManager : MonoBehaviour
{
    [SerializeField] private List<AudioChannel> audioChannels;
    private List<AudioSource> audioSources;


    private void Awake()
    {
        audioSources = new List<AudioSource>();
        for (int i = 0; i < audioChannels.Count; i++)
        {
            audioSources.Add(audioChannels[i].gameObject.GetComponent<AudioSource>());
        }
    }

    private void PlayBackground(AudioChannel audioChannel, List<AudioClip> list, AudioSource audioSource)
    {
        if (audioChannel.isFree)
        {
            if (list.Count != 0)
            {
                int range = Random.Range(0, list.Count);
                audioSource.clip = list[range];
                audioSource.Play();
                audioChannel.PlayCounterTime();
            }
        }
    }

    private void PlaySound(AudioClip nameSound, AudioMixerGroup nameMixerGroup)
    {
        if (nameSound != null)
        {
            for (int i = 0; i < audioChannels.Count; i++)
            {
                if (audioChannels[i].isFree)
                {
                    PlaySoundInSource(nameSound, nameMixerGroup, i);
                    break;
                }
            }
        }
    }

    private void PlaySoundInSource(AudioClip nameSound, AudioMixerGroup nameMixerGroup, int channelNum)
    {
        audioSources[channelNum].clip = nameSound;
        audioSources[channelNum].outputAudioMixerGroup = nameMixerGroup;
        audioSources[channelNum].Play();
        audioChannels[channelNum].PlayCounterTime();
    }

}
