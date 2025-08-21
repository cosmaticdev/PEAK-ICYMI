using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace MedalEncoder.GameCustomizations2.PEAK;

internal class PEAKFormatter
{
  private static IReadOnlyDictionary<string, string> EmojiStringMap = (IReadOnlyDictionary<string, string>) new Dictionary<string, string>()
  {
    {
      "Passed Out",
      "\uD83D\uDCABPassed Out"
    },
    {
      "Died Instantly",
      "\uD83D\uDC80Died Instantly"
    },
    {
      "Large Fall",
      "\ud83e\udd38Large Fall"
    },
    {
      "Thrown By Scoutmaster",
      "\ud83e\udd38Thrown By Scoutmaster"
    },
    {
      "Attacked By Antlion",
      "\uD83D\uDCA5Attacked By Antlion"
    },
    {
      "Revived",
      "\uD83D\uDC9ARevived"
    },
    {
      "Woke Up",
      "\uD83E\uDD71Woke Up"
    },
    {
      "Run Ended",
      "\uD83D\uDD90Run Ended"
    },
    {
      "Called Helicopter",
      "\uD83D\uDE81Called Helicopter"
    }
  };
  private static IReadOnlyDictionary<string, List<(string, string)>> TagMap = (IReadOnlyDictionary<string, List<(string, string)>>) new Dictionary<string, List<(string, string)>>()
  {
    {
      "Passed Out",
      new List<(string, string)>() { ("Event ID", "1") }
    },
    {
      "Died Instantly",
      new List<(string, string)>() { ("Event ID", "2") }
    },
    {
      "Large Fall",
      new List<(string, string)>() { ("Event ID", "3") }
    },
    {
      "Thrown By Scoutmaster",
      new List<(string, string)>() { ("Event ID", "4") }
    },
    {
      "Attacked By Antlion",
      new List<(string, string)>() { ("Event ID", "5") }
    },
    {
      "Revived",
      new List<(string, string)>() { ("Event ID", "6") }
    },
    {
      "Woke Up",
      new List<(string, string)>() { ("Event ID", "7") }
    },
    {
      "Run Ended",
      new List<(string, string)>() { ("Event ID", "8") }
    },
    {
      "Called Helicopter",
      new List<(string, string)>() { ("Event ID", "9") }
    }
  };
  private static IReadOnlyDictionary<string, int> PriorityMap = (IReadOnlyDictionary<string, int>) new Dictionary<string, int>()
  {
    {
      "Called Helicopter",
      900
    },
    {
      "Run Ended",
      800
    },
    {
      "Died Instantly",
      700
    },
    {
      "Revived",
      600
    },
    {
      "Woke Up",
      500
    },
    {
      "Passed Out",
      400
    },
    {
      "Thrown By Scoutmaster",
      300
    },
    {
      "Attacked By Antlion",
      200
    },
    {
      "Large Fall",
      100
    }
  };

  public static MetadataObject FormatClipMetadata(
    Dictionary<string, int> eventFreq,
    Dictionary<string, string> metadata,
    List<Bookmark> bookmarks)
  {
    int num1;
    List<KeyValuePair<string, int>> list = eventFreq.OrderByDescending<KeyValuePair<string, int>, int>((Func<KeyValuePair<string, int>, int>) (kvp => !PEAKFormatter.PriorityMap.TryGetValue(kvp.Key, out num1) ? 0 : num1)).ToList<KeyValuePair<string, int>>();
    EventObject eventObject = new EventObject();
    string str1 = "";
    foreach (KeyValuePair<string, int> keyValuePair in list)
    {
      string key = keyValuePair.Key;
      int num2 = keyValuePair.Value;
      string str2 = list.Count < 3 ? PEAKFormatter.EmojiStringMap[key] : key;
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
