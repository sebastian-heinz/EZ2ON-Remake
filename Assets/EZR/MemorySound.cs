using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace EZR
{
    public static class MemorySound
    {
        public static Dictionary<int, FMOD.Sound> SoundList = new Dictionary<int, FMOD.Sound>();

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

        static MemorySound()
        {
            FMODUnity.RuntimeManager.LowlevelSystem.createChannelGroup("Main", out Main);
            FMODUnity.RuntimeManager.LowlevelSystem.createChannelGroup("BGM", out BGM);
            FMODUnity.RuntimeManager.LowlevelSystem.createChannelGroup("Game", out Game);
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

        public static void playSound(int id, float vol, float pan, FMOD.ChannelGroup group)
        {
            if (SoundList.ContainsKey(id))
            {
                FMOD.Channel channel;
                FMOD.RESULT result;

                result = FMODUnity.RuntimeManager.LowlevelSystem.playSound(SoundList[id], group, true, out channel);
                if (result == FMOD.RESULT.OK)
                {
                    channel.setVolume(vol);
                    channel.setPan(pan);
                    channel.setPaused(false);
                }
            }
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