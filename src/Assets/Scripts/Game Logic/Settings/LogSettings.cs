using System;
using System.Collections.Generic;

[Serializable]
public class LogSettings
{
  public string LogFile = @"Log/DefaultLog.txt";

  public int TotalArchivedFilesToKeep = 3;

  public bool EchoToConsole = true;

  public bool AddTimeStamp = true;

  public bool BreakOnError = true;

  public bool BreakOnAssert = true;

  public List<string> EnabledTraceTags = new List<string>();

  public bool EnableAllTraceTags = false;

  public bool AddTraceTagToMessage = true;

  public LogSettings Clone()
  {
    return this.MemberwiseClone() as LogSettings;
  }
}
