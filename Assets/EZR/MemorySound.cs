using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.RegularExpressions;

namespace EZR
{
    public static class MemorySound
    {
        public static Dictionary<int, FMOD.Sound> SoundList = new Dictionary<int, FMOD.Sound>();
        public static Dictionary<string, FMOD.Sound> SoundUI = new Dictionary<string, FMOD.Sound>();

        public static float MasterVolume
        {
            get
            {
                FMODUnity.RuntimeManager.LowlevelSystem.getMasterChannelGroup(out FMOD.ChannelGroup masterGroup);
                masterGroup.getVolume(out float vol);
                return vol;
            }
            set
            {
                FMODUnity.RuntimeManager.LowlevelSystem.getMasterChannelGroup(out FMOD.ChannelGroup masterGroup);
                masterGroup.setVolume(value);
            }
        }
        public static float MainVolume
        {
            get
            {
                Main.getVolume(out float vol);
                return vol;
            }
            set
            {
                Main.setVolume(value);
            }
        }
        public static float BGMVolume
        {
            get
            {
                BGM.getVolume(out float vol);
                return vol;
            }
            set
            {
                BGM.setVolume(value);
            }
        }
        public static float GameVolume
        {
            get
            {
                Game.getVolume(out float vol);
                return vol;
            }
            set
            {
                Game.setVolume(value);
            }
        }
        public static FMOD.ChannelGroup Main;
        public static FMOD.ChannelGroup BGM;
        public static FMOD.ChannelGroup Game;

        static object streamSound;
        public static object StreamChannel;

        static MemorySound()
        {
            FMODUnity.RuntimeManager.LowlevelSystem.createChannelGroup("Main", out Main);
            FMODUnity.RuntimeManager.LowlevelSystem.createChannelGroup("BGM", out BGM);
            FMODUnity.RuntimeManager.LowlevelSystem.createChannelGroup("Game", out Game);

            var soundUIPath = Path.Combine(EZR.Master.GameResourcesFolder, "SoundUI");
            if (Directory.Exists(soundUIPath))
            {
                var files = Directory.GetFiles(soundUIPath);
                foreach (var fileName in files)
                {
                    var ext = Path.GetExtension(fileName);
                    if (Regex.IsMatch(ext, @"\.ogg$|\.mp3$|\.wav$", RegexOptions.IgnoreCase))
                    {
                        LoadSound(Path.GetFileNameWithoutExtension(fileName), File.ReadAllBytes(fileName));
                    }
                }
            }
        }

        public static void LoadSound(int id, byte[] data)
        {
            var exinfo = new FMOD.CREATESOUNDEXINFO();
            exinfo.cbsize = Marshal.SizeOf(exinfo);
            exinfo.length = (uint)data.Length;
            var result = FMODUnity.RuntimeManager.LowlevelSystem.createSound(data, FMOD.MODE._2D | FMOD.MODE.OPENMEMORY, ref exinfo, out FMOD.Sound sound);
            if (result == FMOD.RESULT.OK)
                SoundList[id] = sound;
        }

        public static void LoadSound(string name, byte[] data)
        {
            var exinfo = new FMOD.CREATESOUNDEXINFO();
            exinfo.cbsize = Marshal.SizeOf(exinfo);
            exinfo.length = (uint)data.Length;
            var result = FMODUnity.RuntimeManager.LowlevelSystem.createSound(data, FMOD.MODE._2D | FMOD.MODE.OPENMEMORY, ref exinfo, out FMOD.Sound sound);
            if (result == FMOD.RESULT.OK)
                SoundUI[name] = sound;
        }

        public static object PlaySound(int id, float vol, float pan, FMOD.ChannelGroup group)
        {
            if (SoundList.ContainsKey(id))
            {
                var result = FMODUnity.RuntimeManager.LowlevelSystem.playSound(SoundList[id], group, true, out FMOD.Channel channel);
                if (result == FMOD.RESULT.OK)
                {
                    channel.setVolume(vol);
                    channel.setPan(pan);
                    channel.setPaused(false);
                    return channel;
                }
                else return null;
            }
            else return null;
        }

        public static void PlaySound(string name)
        {
            if (SoundUI.ContainsKey(name))
            {
                FMODUnity.RuntimeManager.LowlevelSystem.playSound(SoundUI[name], Game, false, out FMOD.Channel channel);
            }
        }

        public static void PlayStream(string path)
        {
            StopStream();
            var result = FMODUnity.RuntimeManager.LowlevelSystem.createSound(path, FMOD.MODE._2D | FMOD.MODE.CREATESTREAM | FMOD.MODE.LOOP_NORMAL, out FMOD.Sound sound);
            if (result == FMOD.RESULT.OK)
            {
                streamSound = sound;
                var result2 = FMODUnity.RuntimeManager.LowlevelSystem.playSound(sound, Game, false, out FMOD.Channel channel);
                if (result2 == FMOD.RESULT.OK)
                    StreamChannel = channel;
            }
        }

        public static void StopStream()
        {
            if (StreamChannel != null) ((FMOD.Channel)StreamChannel).stop();
            if (streamSound != null) ((FMOD.Sound)streamSound).release();
        }

        public static void UnloadAllSound()
        {
            foreach (var item in SoundList)
            {
                item.Value.release();
            }
            SoundList.Clear();
        }
    }
}