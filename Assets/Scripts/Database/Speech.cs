using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using UnityEngine;

public class Speech {

    [XmlAttribute("Name")]
    public string name;

    [XmlAttribute("Branch")]
    public string branch;

    [XmlElement("PID")]
    public int portraitID;

    [XmlElement("NOV")]
    public string nameOverride;

    [XmlElement("Type")]
    public int type;

    [XmlElement("Content")]
    public string content;

    [XmlElement("OB")]
    public string optionbranch;

    [XmlElement("O1")]
    public string option1;

    [XmlElement("O2")]
    public string option2;

    [XmlElement("O3")]
    public string option3;

    [XmlElement("O4")]
    public string option4;

    [XmlElement("O5")]
    public string option5;

}
