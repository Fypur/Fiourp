using System;
using System.Collections.Generic;
using System.Text;
using FMOD;
using FMOD.Studio;

namespace Fiourp
{
    public static class Audio
    {
        public static FMOD.Studio.System system;
        public static FMOD.System coreSystem;
        private static Dictionary<string, EventDescription> cachedEventDescriptions = new Dictionary<string,EventDescription>();

        public static void Initialize()
        {
            FMOD.Studio.System.create(out system);
#if RELEASE
            RESULT result = system.initialize(1024, FMOD.Studio.INITFLAGS.NORMAL, FMOD.INITFLAGS.NORMAL, IntPtr.Zero);
#endif
#if DEBUG
            RESULT result = system.initialize(1024, FMOD.Studio.INITFLAGS.LIVEUPDATE, FMOD.INITFLAGS.NORMAL, IntPtr.Zero);
#endif

            Load();

            system.getCoreSystem(out coreSystem);
        }

        private static void Load()
        {
            foreach(string path in DataManager.GetAllFMODBanksPaths())
            {
                RESULT result = system.loadBankFile(path, LOAD_BANK_FLAGS.NORMAL, out _);
                system.getBankCount(out int count);
                if (result != RESULT.OK)
                    throw new Exception("Bank Loading failed: " + result);
            }
        }

        public static EventInstance PlayEvent(string eventName)
        {
            EventInstance instance = CreateEventInstance("event:/" + eventName);
            instance.start();
            instance.release();
            return instance;
        }

        public static void StopEvent(EventInstance eventInstance, bool allowFadeOut = true)
            => eventInstance.stop(allowFadeOut ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);

        public static void ChangeParameters(EventInstance eventInstance, string paramName, float value)
            => eventInstance.setParameterByName(paramName, value);

        private static EventInstance CreateEventInstance(string eventPath)
        {
            EventDescription eventDesc = GetEventDescription(eventPath);

            if (!eventDesc.isValid())
                throw new Exception("Event Problems on Instance");

            eventDesc.createInstance(out EventInstance instance);
            return instance;
        }

        private static EventDescription GetEventDescription(string path)
        {
            if(cachedEventDescriptions.TryGetValue(path, out EventDescription eventDescription))
                return eventDescription;

            switch (system.getEvent(path, out eventDescription))
            {
                case RESULT.OK:
                    eventDescription.loadSampleData();
                    cachedEventDescriptions.Add(path, eventDescription);
                    return eventDescription;
                default:
                    throw new Exception("Event problems: " + system.getEvent(path, out _));
            }
        }

        public static void SetGroupVolume(string groupID, float volume)
        {
            RESULT result = system.getBus("bus:/" + groupID, out Bus bus);
            if (result != RESULT.OK)
                throw new Exception("help + " + result);

            bus.setVolume(volume);
        }

        public static float GetGroupVolume(string groupID)
        {
            RESULT result = system.getBus("bus:/" + groupID, out Bus bus);
            if (result != RESULT.OK)
                throw new Exception("help + " + result);

            bus.getVolume(out float volume);
            return volume;
        }

        public static void SetMasterVolume(float volume)
        {
            RESULT result = system.getBus("bus:/", out Bus bus);
            if (result != RESULT.OK)
                throw new Exception("help + " + result);

            bus.setVolume(volume);
        }

        public static float GetMasterVolume()
        {
            RESULT result = system.getBus("bus:/", out Bus bus);
            if (result != RESULT.OK)
                throw new Exception("help + " + result);

            bus.getVolume(out float volume);
            return volume;
        }

        public static void Update()
        {
            system.update();
        }

        public static void Finish()
        {
            system.unloadAll();
            system.release();
        }
    }
}
