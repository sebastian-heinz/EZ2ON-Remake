using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;

namespace EZR
{
    public static class UserSaveData
    {
        public static string Version => "1.0";
        public static string MinVer => "1.0";
        public static JObject UserData = new JObject(new JProperty("version", Version));
        static string aesKey = "GameOldBoyEZ2ONRemake";
        static string saveName = "userdata.save";

        static byte[] getKeyBytes
        {
            get
            {
                var key = new byte[32];

                for (int i = 0; i < key.Length; i++)
                {
                    key[i] = (byte)aesKey[i % aesKey.Length];
                }

                return key;
            }
        }

        static byte[] getIvBytes
        {
            get
            {
                var iv = new byte[16];

                for (int i = 0; i < iv.Length; i++)
                {
                    iv[i] = (byte)aesKey[i % aesKey.Length];
                }

                return iv;
            }
        }

        public static void SaveData()
        {
            using (var aes = Aes.Create())
            {
                aes.Key = getKeyBytes;
                aes.IV = getIvBytes;

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            UserData["version"] = Version;
                            swEncrypt.Write(UserData.ToString(Formatting.None));
                        }
                        File.WriteAllBytes(Path.Combine(Application.persistentDataPath, saveName), msEncrypt.ToArray());
                    }
                }
            }
            Debug.Log(UserData.ToString(Formatting.Indented));
        }

        public static void LoadSave()
        {
            var fullPath = Path.Combine(Application.persistentDataPath, saveName);
            if (!File.Exists(fullPath)) return;
            byte[] cipherText = File.ReadAllBytes(fullPath);
            using (var aes = Aes.Create())
            {
                aes.Key = getKeyBytes;
                aes.IV = getIvBytes;

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            var plaintext = srDecrypt.ReadToEnd();
                            try
                            {
                                var userData = JObject.Parse(plaintext);
                                if (!string.IsNullOrEmpty(((string)userData["version"])) &&
                                EZR.Utils.Version2Decmal((string)userData["version"]) >= EZR.Utils.Version2Decmal(MinVer))
                                    UserData = userData;
                            }
                            catch { }
                        }
                    }
                }
            }
        }

        public static int GetScore(string name, GameType type, GameMode.Mode mode, GameDifficult.Difficult diff)
        {
            JObject jobj;
            if (!UserData.ContainsKey("myBestScore")) return 0;
            jobj = (JObject)UserData["myBestScore"];
            if (!jobj.ContainsKey(type.ToString())) return 0;
            jobj = (JObject)jobj[type.ToString()];
            if (!jobj.ContainsKey(name)) return 0;
            jobj = (JObject)jobj[name];
            if (!jobj.ContainsKey(mode.ToString())) return 0;
            jobj = (JObject)jobj[mode.ToString()];
            if (!jobj.ContainsKey(diff.ToString())) return 0;
            return (int)jobj[diff.ToString()];
        }
        public static bool SetScore(int score, string name, GameType type, GameMode.Mode mode, GameDifficult.Difficult diff)
        {
            if (score == 0) return false;
            JObject jobj;
            if (!UserData.ContainsKey("myBestScore"))
                UserData["myBestScore"] = new JObject();
            jobj = (JObject)UserData["myBestScore"];
            if (!jobj.ContainsKey(type.ToString()))
                jobj[type.ToString()] = new JObject();
            jobj = (JObject)jobj[type.ToString()];
            if (!jobj.ContainsKey(name))
                jobj[name] = new JObject();
            jobj = (JObject)jobj[name];
            if (!jobj.ContainsKey(mode.ToString()))
                jobj[mode.ToString()] = new JObject();
            jobj = (JObject)jobj[mode.ToString()];
            if (!jobj.ContainsKey(diff.ToString()))
            {
                jobj[diff.ToString()] = score;
                return true;
            }
            var bestScore = (int)jobj[diff.ToString()];
            if (score > bestScore)
            {
                jobj[diff.ToString()] = score;
                return true;
            }
            else return false;
        }

        public static Option GetOption()
        {
            var option = new Option();
            if (!UserData.ContainsKey("setting")) return option;
            option.FullScreenMode = Utils.ParseEnum<FullScreenMode>((string)UserData["setting"]["fullScreenMode"] ?? option.FullScreenMode.ToString());
            option.Resolution = new Resolution()
            {
                width = (int)(UserData["setting"]["resolution"]["width"] ?? option.Resolution.width),
                height = (int)(UserData["setting"]["resolution"]["height"] ?? option.Resolution.height)
            };
            option.Language = Utils.ParseEnum<SystemLanguage>((string)UserData["setting"]["language"] ?? option.Language.ToString());
            option.TimePrecision = (int)(UserData["setting"]["timePrecision"] ?? option.TimePrecision);
            option.FrostedGlassEffect = (bool)(UserData["setting"]["frostedGlassEffect"] ?? option.FrostedGlassEffect);
            option.VSync = (bool)(UserData["setting"]["vSync"] ?? option.VSync);
            option.SimVSync = (bool)(UserData["setting"]["simVSync"] ?? option.SimVSync);
            option.LimitFPS = (bool)(UserData["setting"]["limitFPS"] ?? option.LimitFPS);
            option.TargetFrameRate = (int)(UserData["setting"]["targetFrameRate"] ?? option.TargetFrameRate);
            return option;
        }

        public static void SetOption(Option option)
        {
            JObject jobj;
            if (!UserData.ContainsKey("setting"))
                UserData["setting"] = new JObject();
            jobj = (JObject)UserData["setting"];
            jobj["fullScreenMode"] = option.FullScreenMode.ToString();
            if (!jobj.ContainsKey("resolution"))
                jobj["resolution"] = new JObject();
            jobj["resolution"]["width"] = option.Resolution.width;
            jobj["resolution"]["height"] = option.Resolution.height;
            jobj["language"] = option.Language.ToString();
            jobj["timePrecision"] = option.TimePrecision;
            jobj["frostedGlassEffect"] = option.FrostedGlassEffect;
            jobj["vSync"] = option.VSync;
            jobj["simVSync"] = option.SimVSync;
            jobj["limitFPS"] = option.LimitFPS;
            jobj["targetFrameRate"] = option.TargetFrameRate;
        }
    }
}