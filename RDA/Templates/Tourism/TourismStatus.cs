using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDA.Templates.Tourism {
  class TourismStatus {
  }

  // HINWEIS: Für den generierten Code ist möglicherweise mindestens .NET Framework 4.5 oder .NET Core/Standard 2.0 erforderlich.
  /// <remarks/>
  [System.SerializableAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
  public partial class Status {

    private StatusRequirement requirementField;

    private StatusText textField;

    private uint poolField;

    /// <remarks/>
    public StatusRequirement Requirement {
      get {
        return this.requirementField;
      }
      set {
        this.requirementField = value;
      }
    }

    /// <remarks/>
    public StatusText Text {
      get {
        return this.textField;
      }
      set {
        this.textField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public uint Pool {
      get {
        return this.poolField;
      }
      set {
        this.poolField = value;
      }
    }
  }

  /// <remarks/>
  [System.SerializableAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  public partial class StatusRequirement {

    private string enField;

    private string deField;

    private string idField;

    /// <remarks/>
    public string EN {
      get {
        return this.enField;
      }
      set {
        this.enField = value;
      }
    }

    /// <remarks/>
    public string DE {
      get {
        return this.deField;
      }
      set {
        this.deField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string ID {
      get {
        return this.idField;
      }
      set {
        this.idField = value;
      }
    }
  }

  /// <remarks/>
  [System.SerializableAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  public partial class StatusText {

    private string enField;

    private string deField;

    private ushort idField;

    /// <remarks/>
    public string EN {
      get {
        return this.enField;
      }
      set {
        this.enField = value;
      }
    }

    /// <remarks/>
    public string DE {
      get {
        return this.deField;
      }
      set {
        this.deField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public ushort ID {
      get {
        return this.idField;
      }
      set {
        this.idField = value;
      }
    }
  }


}
