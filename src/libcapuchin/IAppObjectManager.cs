using System;
using NDesk.DBus;

namespace Capuchin
{
    /// <summary>Entry point for applications to request their own <see cref="Capuchin.AppObject /> object</summary>
    [Interface("org.gnome.Capuchin")]
    public interface IAppObjectManager
    {
        ObjectPath GetAppObject(string repository_url);
    }
}
