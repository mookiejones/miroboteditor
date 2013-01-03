﻿namespace MsgBox
{
  using MsgBox.Internal;

  public static class ServiceInjector
  {
    /// <summary>
    /// Loads service objects into the ServiceContainer on startup.
    /// </summary>
    public static void InjectServices()
    {
      ServiceContainer.Instance.AddService<IMsgBoxService>(new MessageBoxService());
    }
  }
}
