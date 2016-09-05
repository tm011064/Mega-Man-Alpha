using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

public class Logger : IDisposable
{
  private const string EQUALS_SIGN = ": ";

  private static readonly string SEPARATOR = Environment.NewLine;

  private static Logger _logger;

  private LogSettings _loggerSettings;

  private StreamWriter _outputStream;

  private HashSet<string> _enabledTraceTags;

  private Timer _writeTimer;

  private List<string> _messagesToWrite = new List<string>();

  private readonly object _messagesToWriteLock = new object();

  private Logger(LogSettings logSettings)
  {
    _loggerSettings = CloneAndFormat(logSettings);

    _enabledTraceTags = new HashSet<string>(_loggerSettings.EnabledTraceTags);

#if !FINAL
    _writeTimer = new Timer(WriteToDisk, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

    var fileInfo = new FileInfo(logSettings.LogFile);
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

    UnityEngine.Debug.Log(
      "Logger initialized. File location: " + fileInfo.FullName
      + ", Time: " + DateTime.Now.ToString());

    _outputStream = new StreamWriter(logSettings.LogFile.Replace("\\", "/"), false);
#endif
  }

  public bool BreakOnError { get { return _loggerSettings.BreakOnError; } }

  public bool BreakOnAssert { get { return _loggerSettings.BreakOnAssert; } }

  public static void Initialize(LogSettings logSettings)
  {
    if (_logger != null)
    {
      _logger.Dispose();
    }

    _logger = new Logger(logSettings);
  }

  private LogSettings CloneAndFormat(LogSettings logSettings)
  {
    var clone = logSettings.Clone();

    clone.LogFile = clone.LogFile.Replace("\\", "/");

    return clone;
  }

  private void Write(string traceTag, LogLevel type, string message)
  {
#if !FINAL
    if (!string.IsNullOrEmpty(traceTag))
    {
      if (!_loggerSettings.EnableAllTraceTags && !_enabledTraceTags.Contains(traceTag))
      {
        return;
      }

      if (_loggerSettings.AddTraceTagToMessage)
      {
        message = "[" + traceTag + "] " + message;
      }
    }

    if (_loggerSettings.AddTimeStamp)
    {
      message = Time.time.ToString(".0000000") + " " + message;
    }

    lock (_messagesToWriteLock)
    {
      _messagesToWrite.Add(message);
    }

    if (_loggerSettings.EchoToConsole)
    {
      if (type == LogLevel.Trace || type == LogLevel.Info)
      {
        UnityEngine.Debug.Log(message);
      }
      else if (type == LogLevel.Warning)
      {
        UnityEngine.Debug.LogWarning(message);
      }
      else
      {
        // Both Error and Assert go here.
        UnityEngine.Debug.LogError(message);
      }
    }
#endif
  }

  private void WriteToDisk(object obj)
  {
    if (_outputStream != null)
    {
      try
      {
        List<string> messages;

        lock (_messagesToWrite)
        {
          // make a local copy so we don't lock the main thread while writing
          messages = new List<string>(_messagesToWrite);

          _messagesToWrite = new List<string>();
        }

        for (var i = 0; i < messages.Count; i++)
        {
          _outputStream.WriteLine(messages[i]);
          _outputStream.Flush();
        }
      }
      catch (Exception err)
      {
        UnityEngine.Debug.LogException(err);
      }
    }
  }

  [Conditional("DEBUG")]
  public static void Info(string msg)
  {
#if !FINAL
    if (_logger != null)
    {
      _logger.Write(null, LogLevel.Info, msg);
    }
    else
    {
      UnityEngine.Debug.Log(msg);
    }
#endif
  }

  [Conditional("DEBUG")]
  public static void TraceFormat(string msg, params object[] args)
  {
#if !FINAL
    if (_logger != null)
    {
      _logger.Write(null, LogLevel.Trace, string.Format(msg, args));
    }
    else
    {
      UnityEngine.Debug.Log(msg);
    }
#endif
  }

  [Conditional("DEBUG")]
  public static void Trace(string tag, string msg, params object[] args)
  {
#if !FINAL
    if (_logger != null)
    {
      _logger.Write(tag, LogLevel.Trace, string.Format(msg, args));
    }
    else
    {
      UnityEngine.Debug.Log(msg);
    }
#endif
  }

  [Conditional("DEBUG")]
  public static void Trace(string msg)
  {
#if !FINAL
    if (_logger != null)
    {
      _logger.Write(null, LogLevel.Trace, msg);
    }
    else
    {
      UnityEngine.Debug.Log(msg);
    }
#endif
  }

  [Conditional("DEBUG")]
  public static void Trace(object obj)
  {
#if !FINAL
    if (obj == null)
      Trace("NULL");
    else
      Trace(obj.ToString());
#endif
  }

  [Conditional("DEBUG")]
  public static void Trace(string tag, object obj)
  {
#if !FINAL
    if (obj == null)
      Trace(tag, "NULL");
    else
      Trace(tag, obj.ToString());
#endif
  }

  [Conditional("DEBUG"), Conditional("PROFILE")]
  public static void Warning(string msg)
  {
#if !FINAL
    if (_logger != null)
      _logger.Write(null, LogLevel.Warning, msg);
    else
    {
      UnityEngine.Debug.Log(msg);
    }
#endif
  }

  public static void Error(Exception err)
  {
#if !FINAL
    Error(err.Message + Environment.NewLine + err.StackTrace);
#endif
  }

  public static void Error(string msg, Exception err)
  {
#if !FINAL
    Error(msg + Environment.NewLine + err.StackTrace);
#endif
  }

  public static void Error(string msg)
  {
#if !FINAL
    if (_logger != null)
    {
      _logger.Write(null, LogLevel.Error, msg);

      if (_logger.BreakOnError)
        UnityEngine.Debug.Break();
    }
    else
    {
      UnityEngine.Debug.Log(msg);
    }
#endif
  }

  private static string GetMemberName<T>(Expression<Func<T>> memberExpression)
  {
    MemberExpression expressionBody = (MemberExpression)memberExpression.Body;

    return expressionBody.Member.Name + EQUALS_SIGN + memberExpression.Compile().Invoke().ToString();
  }

  [Conditional("DEBUG"), Conditional("PROFILE")]
  public static void DebugMember<T1>(Expression<Func<T1>> memberExpression1)
  {
    var message = GetMemberName(memberExpression1);

    UnityEngine.Debug.Log(message);
  }

  [Conditional("DEBUG"), Conditional("PROFILE")]
  public static void DebugMember<T1, T2>(
    Expression<Func<T1>> memberExpression1,
    Expression<Func<T2>> memberExpression2)
  {
    var message = GetMemberName(memberExpression1)
      + SEPARATOR + GetMemberName(memberExpression2);

    UnityEngine.Debug.Log(message);
  }

  [Conditional("DEBUG"), Conditional("PROFILE")]
  public static void DebugMember<T1, T2, T3>(
    Expression<Func<T1>> memberExpression1,
    Expression<Func<T2>> memberExpression2,
    Expression<Func<T3>> memberExpression3)
  {
    var message = GetMemberName(memberExpression1)
      + SEPARATOR + GetMemberName(memberExpression2)
      + SEPARATOR + GetMemberName(memberExpression3);

    UnityEngine.Debug.Log(message);
  }

  [Conditional("DEBUG"), Conditional("PROFILE")]
  public static void DebugMember<T1, T2, T3, T4>(
    Expression<Func<T1>> memberExpression1,
    Expression<Func<T2>> memberExpression2,
    Expression<Func<T3>> memberExpression3,
    Expression<Func<T4>> memberExpression4)
  {
    var message = GetMemberName(memberExpression1)
      + SEPARATOR + GetMemberName(memberExpression2)
      + SEPARATOR + GetMemberName(memberExpression3)
      + SEPARATOR + GetMemberName(memberExpression4);

    UnityEngine.Debug.Log(message);
  }

  [Conditional("DEBUG"), Conditional("PROFILE")]
  public static void Assert(bool condition, string msg)
  {
#if !FINAL
    if (condition)
      return;

    if (_logger != null)
    {
      _logger.Write(null, LogLevel.Assert, msg);

      if (_logger.BreakOnAssert)
      {
        // we also log so we can double click in unity
        UnityEngine.Debug.LogError("Assertion failed. " + msg);

        UnityEngine.Debug.Break();
      }
    }
#endif
  }

  public void Dispose()
  {
#if !FINAL
    if (_outputStream != null)
    {
      // write remaining messages
      WriteToDisk(null);

      _writeTimer.Dispose();

      try
      {
        _outputStream.Dispose();
      }
      catch (Exception err)
      {
        UnityEngine.Debug.LogException(err);
      }

      try
      {
        var fileInfo = new FileInfo(_loggerSettings.LogFile);

        if (_loggerSettings.TotalArchivedFilesToKeep > 0)
        {
          var archivedFileName = Path.Combine(fileInfo.DirectoryName, fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf('.')) + "_" + DateTime.Now.ToString("ddMMyy_HHmm") + ".txt");

          UnityEngine.Debug.Log("Archiving current log file to: " + archivedFileName);

          fileInfo.CopyTo(archivedFileName, true);
        }

        var regex = new Regex("^" + fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf('.')) + "_([0-9]{6})_([0-9]{4}).txt$");

        var archivedFiles = new List<FileInfo>();

        foreach (FileInfo file in fileInfo.Directory.GetFiles())
        {
          if (regex.IsMatch(file.Name))
          {
            archivedFiles.Add(file);
          }
        }

        int totalArchivedFiles = archivedFiles.Count;

        if (totalArchivedFiles > _loggerSettings.TotalArchivedFilesToKeep)
        {
          UnityEngine.Debug.Log("Running log file archive cleanup. Total archived files: " + totalArchivedFiles + ", threshold: " + _loggerSettings.TotalArchivedFilesToKeep);

          archivedFiles = archivedFiles.OrderBy(c => c.CreationTimeUtc).ToList();

          for (var i = 0; i < (totalArchivedFiles - _loggerSettings.TotalArchivedFilesToKeep); i++)
          {
            UnityEngine.Debug.Log("Deleting archived file: " + archivedFiles[i].FullName);

            archivedFiles[i].Delete();
          }
        }
      }
      catch (Exception err)
      {
        UnityEngine.Debug.LogException(err);
      }
    }
#endif
  }

  public static void Restart()
  {
    if (_logger != null)
    {
      Initialize(_logger._loggerSettings);
    }
  }

  public static void Destroy()
  {
    if (_logger != null)
    {
      _logger.Dispose();
    }
  }

  private enum LogLevel
  {
    Trace,

    Info,

    Warning,

    Error,

    Assert
  };
}
