﻿namespace AssetViewer.Data {

  using System.ComponentModel;
  using AssetViewer.Converter;

  [TypeConverter(typeof(EnumDescriptionTypeConverter))]
  public enum Languages {

    [Description("English")]
    English,

    [Description("Deutsch")]
    German,

    [Description("Brazilian")]
    Brazilian,

    [Description("简体中文")]
    Chinese,

    [Description("Français")]
    French,

    [Description("Italiano")]
    Italian,

    [Description("日本の")]
    Japanese,

    [Description("한국의")]
    Korean,

    [Description("Polski")]
    Polish,

    [Description("Português")]
    Portuguese,

    [Description("Pусский")]
    Russian,

    [Description("Español")]
    Spanish,

    [Description("ไทย")]
    Taiwanese
  }
}