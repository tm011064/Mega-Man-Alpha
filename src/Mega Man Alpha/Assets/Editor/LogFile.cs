#if UNITY_EDITOR

using System;
using System.IO;
using UnityEngine;

public class LogFile : IDisposable
{
  private StreamWriter _logStream;

  private bool _echo;

  public LogFile(string filename, bool echo_to_debug)
  {
    var fileInfo = new FileInfo(filename);
    if (!fileInfo.Exists)
    {
      if (!fileInfo.Directory.Exists)
      {
        fileInfo.Directory.Create();
      }

      if (fileInfo.Exists)
      {
        fileInfo.Create();
      }
    }

    _logStream = new StreamWriter(filename);

    _echo = echo_to_debug;
  }

  public void Message(string msg)
  {
    if (_logStream != null)
    {
      _logStream.WriteLine(msg);

      if (_echo)
      {
        Debug.Log(msg);
      }
    }
  }

  public void Dispose()
  {
    try
    {
      _logStream.Dispose();
    }
    catch (Exception err)
    {
      Debug.LogException(err);
    }
  }
}

#endif
