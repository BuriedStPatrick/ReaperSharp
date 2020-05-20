using System.Collections;
using ReaperCore;
using UnityEngine;

namespace ReaperUnity
{
    public partial class UnityReaperParser
    {
        /// <summary>
        /// Container to pass certain data types to IEnumerators as reference.
        /// </summary>
        public class Container<T>
        {
            public T t;
        }

        /// <summary>
        /// Loads the audio from disk using the WWW class. Adds the loaded file
        /// as an AudioClip to the Container<AudioClip>.
        /// </summary>
        /// <param name="item">The ReaperNode instance with type "ITEM".</param>
        /// <param name="clipContainer">An instance of ClipContainer<AudioClip>.</param>
        public static IEnumerator LoadAudioFromDisk(ReaperNode item, Container<AudioClip> clipContainer)
        {
            if (item.Type == "ITEM")
            {
                var source_wave = item.GetNode("SOURCE");
                var source_path = source_wave.GetNode("FILE").Value;

                if (source_path[0] != '/') // is relative rppPath
                    source_path = item.Parser.directory + "/" + source_path;

                string url = "file:///" + source_path;
                WWW www = new WWW(url);

                clipContainer.t = www.GetAudioClip();

                while (!clipContainer.t.isReadyToPlay)
                    yield return null;
            }
        }
    }
}
