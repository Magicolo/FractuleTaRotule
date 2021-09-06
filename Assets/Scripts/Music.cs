using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Melanchall.DryWetMidi.MusicTheory;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

/*
    DOING:

    TODO:
    - Change scale when global luminosity drops low. Maybe change instrument group and/or change some visual parameters?
    - Round octave rather than flooring it to get the closest sample and apply less pitch shift.
*/
public class Music : MonoBehaviour
{
    public sealed class Sound
    {
        public AudioClip Clip;
        public AudioSource Source;
        public float Volume;
        public float Pitch;
        public float Pan;
        public float Duration;
        public float Fade;
        public Func<bool> Continue;
        public Action<AudioSource> OnPlay;
        public Action<AudioSource> OnStop;
    }

    static readonly NoteName[] PentatonicMajor = new[] {
        NoteName.C,
        NoteName.D,
        NoteName.E,
        NoteName.G,
        NoteName.A,
    };
    static readonly NoteName[] PentatonicMinor = new[] {
        NoteName.C,
        NoteName.DSharp,
        NoteName.F,
        NoteName.G,
        NoteName.ASharp,
    };

    static int Snap(int note, NoteName[] notes)
    {
        var source = note % 12;
        var target = notes.Cast<int>().OrderBy(note => Math.Abs(note - source)).FirstOrDefault();
        return note - source + target;
    }

    public int Voices = 16;
    public float Threshold = 0.1f;
    public Camera Camera;

    void Awake() => Application.targetFrameRate = 15;

    IEnumerator Start()
    {
        var instruments = GetComponentsInChildren<Instrument>().Where(instrument => instrument.isActiveAndEnabled).ToArray();
        var texture = Camera.targetTexture;
        var width = texture.width;
        var height = texture.height;
        var pixels = new Color32[width * height];
        var buffer = new NativeArray<Color32>(width * height, Allocator.Persistent);
        var sources = new AudioSource[width * height];
        var notes = PentatonicMinor;

        while (true)
        {
            var time = TimeSpan.FromSeconds(Time.time);
            var process = Task.Run(() =>
            {
                var routines = new List<IEnumerator>();
                var sum = Color.clear;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var index = x + y * width;
                        var pixel = pixels[index];
                        sum += pixel;

                        var source = sources[index];
                        if (source) continue;

                        Color.RGBToHSV(pixel, out var hue, out _, out var value);
                        if (value < Threshold) continue;

                        var instrument = instruments[(int)(hue * instruments.Length)];
                        var note = Snap((int)(value * value * 80), notes);
                        var sound = new Sound
                        {
                            Clip = instrument.Clips[note / 12],
                            Source = instrument.Source,
                            Volume = value * value,
                            Pitch = Mathf.Pow(2, note % 12 / 12f),
                            Pan = Mathf.Clamp01((float)x / width) * 2f - 1f,
                            Duration = instrument.Duration,
                            Fade = instrument.Duration,
                            Continue = () =>
                            {
                                var pixel = pixels[index];
                                Color.RGBToHSV(pixel, out _, out _, out var value);
                                return value >= Threshold;
                            },
                            OnPlay = source => sources[index] = source,
                            OnStop = _ => sources[index] = null,
                        };
                        routines.Add(Play(sound));
                    }
                }
                return routines;
            });

            var request = AsyncGPUReadback.RequestIntoNativeArray(ref buffer, texture);
            while (!request.done) yield return null;
            buffer.CopyTo(pixels);
            while (!process.IsCompleted) yield return null;

            for (int i = 0; i < Voices && i < process.Result.Count; i++)
                StartCoroutine(process.Result[UnityEngine.Random.Range(0, process.Result.Count)]);
        }
    }

    IEnumerator Play(Sound sound)
    {
        var source = Instantiate(sound.Source, transform);
        source.name = sound.Clip.name;
        source.clip = sound.Clip;
        source.volume = sound.Source.volume * sound.Volume;
        source.pitch = sound.Source.pitch * sound.Pitch;
        source.panStereo = sound.Source.panStereo + sound.Pan;
        source.Play();
        sound.OnPlay(source);

        for (var counter = 0f; counter < sound.Duration && source.isPlaying && sound.Continue(); counter += Time.deltaTime)
            yield return null;

        for (var counter = 0f; counter < sound.Fade && source.isPlaying; counter += Time.deltaTime)
        {
            source.volume = sound.Volume * (1f - Mathf.Clamp01(counter / sound.Fade));
            yield return null;
        }

        source.Stop();
        sound.OnStop(source);
        Destroy(source.gameObject);
    }
}
