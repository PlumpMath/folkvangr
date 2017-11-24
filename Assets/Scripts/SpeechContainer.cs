using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using UnityEngine;
using System.IO;

[XmlRoot("DialogueTree")]
public class SpeechContainer {

    [XmlArray("Dialogue")]
    [XmlArrayItem("Speech")]
    public List<Speech> diaBranch = new List<Speech>();


    public static SpeechContainer Load(string path)
    {
        TextAsset _xml = Resources.Load<TextAsset>(path);

        XmlSerializer serializer = new XmlSerializer(typeof(SpeechContainer));

        StringReader reader = new StringReader(_xml.text);

        SpeechContainer _dialogue = serializer.Deserialize(reader) as SpeechContainer;

        reader.Close();

        return _dialogue;
    }
}
