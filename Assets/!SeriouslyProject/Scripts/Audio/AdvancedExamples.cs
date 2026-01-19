using AudioManager.Core;
using AudioManager.Helper;
using AudioManager.Locator;
using AudioManager.Service;
using AudioManager.Settings;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AdvancedExamples : MonoBehaviour
{
    [SerializeField]
    private AdvancedExample advancedExample;

    [Header("Initialization:")]
    [SerializeField]
    private AudioSourceSetting[] allSettingsFiles;

    [Header("Names of sounds in the settings files:")]
    [SerializeField] private string sound3D_Name;
    [SerializeField] private Vector3 position3D;
    [SerializeField] private float fadeInTime3D;

    [Header("Loop song sub-section:")]
    [SerializeField] private string loopSound_Name;
    [SerializeField] private float loopStart;
    [SerializeField] private float loopEnd;

    [Header("Reverse Loop song sub-section:")]
    [SerializeField] private string reverseLoopSound_Name;
    [SerializeField] private float reverseLoopStart;
    [SerializeField] private float reverseLoopEnd;

    [Header("Play song after another example:")]
    [SerializeField] private string firstSound_Name;
    [SerializeField] private string secondSound_Name;

    [Header("Play song in reverse:")]
    [SerializeField] private string reverseSound_Name;
    [SerializeField] private float directionPitch;

    [Header("Fade in song:")]
    [SerializeField] private string fadeInSong_Name;
    [SerializeField] private float clipFadeInTime;
    [SerializeField] private float fadeInEndVolume;

    [Header("Fade out song:")]
    [SerializeField] private string fadeOutSong_Name;
    [SerializeField] private float clipFadeOutTime;

    [Header("Change volume via. UI:")]
    [SerializeField] private string groupSong_Name;
    [SerializeField] private Slider uiSlider;

    private const string EXPOSED_VOLUME_NAME = "Volume";
    private IAudioManager am;

    private void Start()
    {
        SettingsHelper.SetupSounds(out var sounds, allSettingsFiles, this.gameObject);
        ServiceLocator.RegisterService(new DefaultAudioManager(sounds, this.gameObject));
        am = ServiceLocator.GetService();

        switch (advancedExample)
        {
            case AdvancedExample.LERP_IN_3D_SOUND_AT_POS:
                InitLerpIn3DSoundAtPosExample();
                break;
            case AdvancedExample.LOOP_SONG_SUB_SECTION:
                InitLoopSubSectionExample();
                break;
            case AdvancedExample.REVERSE_LOOP_SONG_SUB_SECTION:
                InitReverseLoopSubSectionExample();
                break;
            case AdvancedExample.PLAY_SONG_AFTER_ANOTHER:
                InitPlaySongAfterAnotherExample();
                break;
            case AdvancedExample.PLAY_SONG_IN_REVERSE:
                InitPlaySongInReverseExample();
                break;
            case AdvancedExample.FADE_IN_SONG:
                InitFadeInSongExample();
                break;
            case AdvancedExample.FADE_OUT_SONG:
                InitFadeOutSongExample();
                break;
            case AdvancedExample.CHANGE_VOLUME_VIA_UI:
                InitChangeVolumeViaUIExample();
                break;
        }
    }

    public ProgressResponse SongProgressCallback(string name, float progress, ChildType child)
    {
        // Ошибка CS0023: исправлено (сравнение с AudioError.OK)
        if (am.TryGetSource(name, out var source) != AudioError.OK) return ProgressResponse.UNSUB;

        Debug.Log($"ChildType: {Enum.GetName(typeof(ChildType), child)}, Actual: {source.Time}, Expected: {progress * source.Source.clip.length}");

        switch (advancedExample)
        {
            case AdvancedExample.LERP_IN_3D_SOUND_AT_POS:
                return HandleLerpIn3DSoundAtPosExample(name);
            case AdvancedExample.LOOP_SONG_SUB_SECTION:
                return HandleLoopSubSectionExample(name);
            case AdvancedExample.REVERSE_LOOP_SONG_SUB_SECTION:
                return HandleReverseLoopSubSectionExample(name);
            case AdvancedExample.FADE_IN_SONG:
                return HandleFadeInSongExample(name);
            case AdvancedExample.FADE_OUT_SONG:
                return HandleFadeOutSongExample(name, progress);
            default:
                return ProgressResponse.UNSUB;
        }
    }

    private void InitLerpIn3DSoundAtPosExample()
    {
        am.SubscribeProgressCoroutine(sound3D_Name, 0f, SongProgressCallback);
        am.RegisterChildAt3DPos(sound3D_Name, position3D, out ChildType child);
        am.Play(sound3D_Name, child);
    }

    private ProgressResponse HandleLerpIn3DSoundAtPosExample(string name)
    {
        if (am.TryGetSource(name, out var source) == AudioError.OK)
        {
            float endVolume = source.Volume;
            source.Volume = 0f;
            am.LerpVolume(name, endVolume, fadeInTime3D);
        }
        return ProgressResponse.RESUB_IN_LOOP;
    }

    private void InitLoopSubSectionExample()
    {
        // Ошибка CS0029: исправлено
        if (am.TryGetSource(loopSound_Name, out var source) == AudioError.OK)
        {
            source.Time = loopStart;
            float progress = (loopEnd / source.Source.clip.length);
            am.SubscribeProgressCoroutine(loopSound_Name, progress, SongProgressCallback);
            am.Play(loopSound_Name);
        }
    }

    private ProgressResponse HandleLoopSubSectionExample(string name)
    {
        am.SkipTime(name, loopStart - loopEnd);
        return ProgressResponse.RESUB_IMMEDIATE;
    }

    private void InitReverseLoopSubSectionExample()
    {
        am.SetPlaybackDirection(reverseLoopSound_Name);
        // Ошибка CS0029: исправлено
        if (am.TryGetSource(reverseLoopSound_Name, out var source) == AudioError.OK)
        {
            source.Time = source.Source.clip.length - reverseLoopStart;
            float progress = (source.Source.clip.length - reverseLoopEnd - reverseLoopStart) / source.Source.clip.length;
            am.SubscribeProgressCoroutine(reverseLoopSound_Name, progress, SongProgressCallback);
            am.Play(reverseLoopSound_Name);
        }
    }

    private ProgressResponse HandleReverseLoopSubSectionExample(string name)
    {
        am.SkipTime(name, reverseLoopEnd);
        return ProgressResponse.RESUB_IMMEDIATE;
    }

    private void InitPlaySongAfterAnotherExample()
    {
        am.PlayScheduled(firstSound_Name, 0d);
        // GetClipLength тоже возвращает AudioError, так что проверяем успех
        if (am.GetClipLength(firstSound_Name, out double time) == AudioError.OK)
        {
            am.PlayScheduled(secondSound_Name, time);
        }
    }

    private void InitPlaySongInReverseExample()
    {
        am.SetPlaybackDirection(reverseSound_Name, directionPitch);
        am.Play(reverseSound_Name);
    }

    private void InitFadeInSongExample()
    {
        am.SubscribeProgressCoroutine(fadeInSong_Name, 0f, SongProgressCallback);
        am.Play(fadeInSong_Name);
    }

    private ProgressResponse HandleFadeInSongExample(string name)
    {
        // Ошибка CS0029: исправлено
        if (am.TryGetSource(name, out var source) == AudioError.OK)
        {
            source.Volume = 0f;
            am.LerpVolume(name, fadeInEndVolume, clipFadeInTime);
        }
        return ProgressResponse.RESUB_IN_LOOP;
    }

    private void InitFadeOutSongExample()
    {
        // Ошибка CS0029: исправлено
        if (am.TryGetSource(fadeOutSong_Name, out var source) == AudioError.OK)
        {
            float progress = (source.Source.clip.length - clipFadeOutTime) / source.Source.clip.length;
            am.SubscribeProgressCoroutine(fadeOutSong_Name, progress, SongProgressCallback);
            am.Play(fadeOutSong_Name);
        }
    }

    private ProgressResponse HandleFadeOutSongExample(string name, float progress)
    {
        am.LerpVolume(name, 0f, progress);
        return ProgressResponse.UNSUB;
    }

    private void InitChangeVolumeViaUIExample()
    {
        HandleChangeVolumeViaUIExample(uiSlider.value);
        am.Play(groupSong_Name);
    }

    public void HandleChangeVolumeViaUIExample(float sliderValue)
    {
        float newVolume = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20;
        am.ChangeGroupValue(groupSong_Name, EXPOSED_VOLUME_NAME, newVolume);
    }
}