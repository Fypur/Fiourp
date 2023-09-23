using FMOD.Studio;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class Sound3D : Component
    {
        public EventInstance Sound;

        private FMOD.ATTRIBUTES_3D attributes;

        private string eventName;
        private bool autoRemove;

        public Sound3D(string eventName, bool autoRemove = false)
        {
            Sound = Audio.PlayEvent(eventName);
            this.eventName = eventName;

            attributes = new FMOD.ATTRIBUTES_3D()
            {
                forward = Vector2.UnitX.ToFMODVector(),
                up = (-Vector2.UnitY).ToFMODVector(),
            };

            this.autoRemove = autoRemove;
        }

        public override void Added()
        {
            base.Added();

            attributes.position = (ParentEntity.MiddlePos - Engine.Cam.MiddlePos).ToFMODVector();
            Sound.set3DAttributes(attributes);
        }

        public override void Update()
        {
            base.Update();

            attributes.position = (ParentEntity.MiddlePos - Engine.Cam.MiddlePos).ToFMODVector();
            Sound.set3DAttributes(attributes);

            Sound.getPlaybackState(out var state);

            if(autoRemove && state == PLAYBACK_STATE.STOPPED)
                ParentEntity.RemoveComponent(this);
        }

        public override void Removed()
        {
            base.Removed();

            Audio.StopEvent(Sound);
        }

        public void Play()
        {
            Sound = Audio.PlayEvent(eventName);
        }

        public void Stop(bool allowFadeOut = true)
            => Audio.StopEvent(Sound, allowFadeOut);

        public PLAYBACK_STATE GetState()
        {
            Sound.getPlaybackState(out var state);
            return state;
        }

        public bool isPlaying()
        {
            Sound.getPlaybackState(out var state);
            if (state == PLAYBACK_STATE.PLAYING || state == PLAYBACK_STATE.STARTING)
                return true;
            return false;
        }
        
        /// <summary>
        /// Volume should be in between 0 and 1
        /// </summary>
        /// <param name="volume"></param>
        public void SetVolume(float volume)
        {
            Sound.setVolume(volume);
        }

        public void SetParameter(string parameter, float value, bool ignoreSeekSpeed = false)
            => Sound.setParameterByName(parameter, value, ignoreSeekSpeed);
    }
}
