// Decompiled with JetBrains decompiler
// Type: MedalEncoder.GameCustomizations2.REPO.REPOFormatter
// Assembly: MedalEncoder, Version=3.1062.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 707522FE-0E3F-4042-8489-C5CD5D666AE8
// Assembly location: C:\Users\Mineb\AppData\Local\Medal\recorder-3.1062.0\MedalEncoder.exe

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace MedalEncoder.GameCustomizations2.REPO;

internal class REPOFormatter
{
  private static IReadOnlyDictionary<string, string> EmojiStringMap = (IReadOnlyDictionary<string, string>) new Dictionary<string, string>()
  {
    {
      "Death",
      "\uD83D\uDC80Death"
    },
    {
      "Item Break",
      "\uD83D\uDC8EItem Break"
    }
  };
  private static IReadOnlyDictionary<string, List<(string, string)>> TagMap = (IReadOnlyDictionary<string, List<(string, string)>>) new Dictionary<string, List<(string, string)>>()
  {
    {
      "Death",
      new List<(string, string)>() { ("Event ID", "1") }
    },
    {
      "Item Break",
      new List<(string, string)>() { ("Event ID", "2") }
    }
  };
  private static IReadOnlyDictionary<string, int> PriorityMap = (IReadOnlyDictionary<string, int>) new Dictionary<string, int>()
  {
    {
      "Death",
      200
    },
    {
      "Item Break",
      100
    }
  };

  public static MetadataObject FormatClipMetadata(
    Dictionary<string, int> eventFreq,
    Dictionary<string, string> metadata,
    List<Bookmark> bookmarks)
  {
    int num1;
    List<KeyValuePair<string, int>> list = eventFreq.OrderByDescending<KeyValuePair<string, int>, int>((Func<KeyValuePair<string, int>, int>) (kvp => !REPOFormatter.PriorityMap.TryGetValue(kvp.Key, out num1) ? 0 : num1)).ToList<KeyValuePair<string, int>>();
    EventObject eventObject = new EventObject();
    string str1 = "";
    foreach (KeyValuePair<string, int> keyValuePair in list)
    {
      string key = keyValuePair.Key;
      int num2 = keyValuePair.Value;
      string str2 = list.Count < 3 ? REPOFormatter.EmojiStringMap[key] : key;
      string str3 = num2 > 1 ? $" x{num2}" : "";
      string str4 = list.Count <= 2 || key.Equals(list.Last<KeyValuePair<string, int>>().Key) ? " " : ", ";
      str1 = str1 + str2 + str3 + str4;
    }
    string str5 = str1.Trim();
    eventObject.name = str5;
    return new MetadataObject()
    {
      eventobj = eventObject,
      triggerType = "ICYMI",
      generateThumbnail = true
    };
  }
}
