////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// It sets and play sounds from the assets or Windows media files
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Media;

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Class that use the C:\Windows\Media Sounds
    /// </summary>
    public class SoundManager
    {
        /// <summary>
        /// Are the sounds initialized
        /// </summary>
        private static bool _isSoundsInitialized;
        /// <summary>
        /// Main object used to play custom Media sounds
        /// </summary>
        private static SoundPlayer _soundPlayer = new SoundPlayer();
        /// <summary>
        /// Possible initialized sounds
        /// </summary>
        private static Dictionary<SoundType, SoundPlayer> _soundsPlayer;
        /// <summary>
        /// Enum of the different types of sounds
        /// </summary>
        public enum SoundType
        {
            None = 0,
            Custom = 1,
            CaregiverAttention = 2,
            OpenEyes = 3,
            Click,
            OpenEyesCalibration,
            CloseEyesCalibration,
        }

        /// <summary>
        /// Play a sounds as notifications warnings
        /// </summary>
        /// <param name="soundType"> Type of sound to play</param>
        /// <param name="customPath"> (Optional) Path of the file to play a custom sound </param>
        public static void playSound(SoundType soundType, string customPath = null)
        {
            try
            {
                if (!_isSoundsInitialized)
                    SetSounds();

                if (CoreGlobals.AppPreferences.EnableSounds && _isSoundsInitialized)
                {
                    switch (soundType)
                    {
                        case SoundType.Custom:
                            if (_soundPlayer == null)
                            {
                                _soundPlayer = new SoundPlayer();
                            }
                            if (customPath != null)
                                SetSoundFile(_soundPlayer, customPath);
                            break;

                        case SoundType.CaregiverAttention:
                            var soundCA = _soundsPlayer[SoundType.CaregiverAttention];
                            soundCA.Play();
                            break;

                        case SoundType.OpenEyes:
                            var soundOE = _soundsPlayer[SoundType.OpenEyes];
                            soundOE.Play();
                            break;

                        case SoundType.Click:
                            var soundC = _soundsPlayer[SoundType.Click];
                            soundC.Play();
                            break;

                        case SoundType.OpenEyesCalibration:
                            var soundOEC = _soundsPlayer[SoundType.OpenEyesCalibration];
                            soundOEC.Play();
                            break;

                        case SoundType.CloseEyesCalibration:
                            var soundCEC = _soundsPlayer[SoundType.CloseEyesCalibration];
                            soundCEC.Play();
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Error playing Sound: " + ex);
            }
        }

        /// <summary>
        /// Sets the file path to the SoundPlayer object
        /// </summary>
        /// <param name="player"></param>
        /// <param name="soundFilePath"></param>
        private static void SetSoundFile(SoundPlayer player, string soundFilePath)
        {
            if (player != null)
            {
                player.Stop();
                player.SoundLocation = soundFilePath;
                player.Load();
                player.Play();
            }
            else
                Log.Debug("_soundPlayer object was null");
        }

        /// <summary>
        /// Sets the file path to the SoundPlayer object
        /// </summary>
        /// <param name="player"></param>
        /// <param name="soundFilePath"></param>
        private static void SetSoundFileSync(SoundPlayer player, string soundFilePath)
        {
            if (player != null)
            {
                player.Stop();
                player.SoundLocation = soundFilePath;
                player.Load();
                player.PlaySync();
            }
            else
                Log.Debug("_soundPlayer object was null");
        }

        private static void SetSounds()
        {
            _soundsPlayer = new Dictionary<SoundType, SoundPlayer>
            {
                { SoundType.CaregiverAttention, new SoundPlayer(@"C:\windows\media\Ring01.wav") },
                { SoundType.OpenEyes, new SoundPlayer(@"C:\windows\media\Speech On.wav") },
                { SoundType.Click, new SoundPlayer(FileUtils.GetSoundPath("click.wav")) },
                { SoundType.OpenEyesCalibration, new SoundPlayer(FileUtils.GetSoundPath("eyesopen.wav")) },
                { SoundType.CloseEyesCalibration, new SoundPlayer(FileUtils.GetSoundPath("eyesclosed.wav")) },
            };
            _isSoundsInitialized = true;
        }
    }
}