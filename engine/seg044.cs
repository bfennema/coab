using Avalonia.Rendering;
using Classes;
using MiniAudioEx.Core.StandardAPI;
using System;
using System.IO;

namespace engine
{
    public class seg044
    {
        public static void SetSound(bool On)
        {
            gbl.soundType = On ? SoundType.PC : SoundType.None;
        }

        public static void SetPicture(bool On)
        {
            gbl.PicsOn = On;
        }

        public static void SetAnimation(bool On)
        {
            gbl.AnimationsOn = On;
        }

        internal static void PlaySound(Sound arg_0) /*sub_120E0*/
        {
            if (gbl.soundType == SoundType.PC)
            {
                if (arg_0 == Sound.sound_0)
                {
                    foreach (var sp in sounds)
                    {
                        if (sp != null)
                        {
                            //source.Stop();
                            //sp.Stop();
                        }
                    }
                }
                else if (arg_0 == Sound.sound_1)
                {
                }
                else if (arg_0 == Sound.sound_FF) // off maybe.
                {
                    foreach (var sp in sounds)
                    {
                        if (sp != null)
                        {
                            //source.Stop();
                            //sp.Stop();
                        }
                    }
                }
                else if (arg_0 >= Sound.sound_2 && arg_0 <= Sound.sound_e)
                {
                    int sampleId = (int)arg_0 - 1;
                    if (sounds[sampleId] != null)
                    {
                        source2.PlayOneShot(sounds[sampleId]);
                        //source2.Cursor = 0;
                        //source2.Play();
                    }
                    else
                    {
                    }
                }
                else if (arg_0 == Sound.sound_f)
                {
                }
            }
        }

        static AudioClip sound;
        static AudioClip[] sounds;
        //static AudioApp app;
        static AudioSource source1;
        static AudioSource source2;
        //static System.Media.SoundPlayer[] sounds;

        internal static void SoundInit(string resourceName)
        {
            if (System.Reflection.Assembly.GetEntryAssembly() != null)
            {
                var resources = new System.Resources.ResourceManager(resourceName, System.Reflection.Assembly.GetEntryAssembly());

                AudioContext.Initialize(44100, 2, 64);
                //app = new AudioApp(44100, 2);
                //app.Run();

                source1 = new AudioSource(1);
                byte[] data = [0x00];
                sound = StreamToClip(resources.GetStream("death"));
                source1.Loop = true;
                source1.Play(sound);

                source2 = new AudioSource(16);

                sounds = new AudioClip[13];

                sounds[1] = StreamToClip(resources.GetStream("missle"));
                sounds[2] = StreamToClip(resources.GetStream("magic_hit"));
                sounds[4] = StreamToClip(resources.GetStream("death"));
                sounds[5] = StreamToClip(resources.GetStream("sound_5"));
                sounds[6] = StreamToClip(resources.GetStream("hit"));
                sounds[8] = StreamToClip(resources.GetStream("miss"));
                //string dir = Directory.GetCurrentDirectory();
                sounds[9] = new AudioClip("step.wav", false);
                //sounds[9] = StreamToClip(resources.GetStream("step"));
                sounds[10] = StreamToClip(resources.GetStream("sound_10"));
                sounds[12] = StreamToClip(resources.GetStream("start_sound"));
                //sounds = new System.Media.SoundPlayer[13];

                //sounds[1] = new System.Media.SoundPlayer(resources.GetStream("missle"));
                //sounds[2] = new System.Media.SoundPlayer(resources.GetStream("magic_hit"));
                //sounds[4] = new System.Media.SoundPlayer(resources.GetStream("death"));
                //sounds[5] = new System.Media.SoundPlayer(resources.GetStream("sound_5"));
                //sounds[6] = new System.Media.SoundPlayer(resources.GetStream("hit"));
                //sounds[8] = new System.Media.SoundPlayer(resources.GetStream("miss"));
                //sounds[9] = new System.Media.SoundPlayer(resources.GetStream("step"));
                //sounds[10] = new System.Media.SoundPlayer(resources.GetStream("sound_10"));
                //sounds[12] = new System.Media.SoundPlayer(resources.GetStream("start_sound"));
            }
        }

        internal static AudioClip StreamToClip(Stream audioStream)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                audioStream.CopyTo(memoryStream);
                byte[] data = memoryStream.ToArray();

                AudioClip clip = new(data, false);

                return clip;
            }
        }
    }
}
