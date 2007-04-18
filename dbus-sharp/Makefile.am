dbusdir = $(pkglibdir)
dbus_SCRIPTS = NDesk.DBus.dll
CLEANFILES = NDesk.DBus.dll
EXTRA_DIST = $(dbus_sources)

dbus_sources = \
    Address.cs \
    Introspection.cs \
    Signature.cs \
    AssemblyInfo.cs  \
    SocketTransport.cs \
    Authentication.cs  \
    Mapper.cs \
    Transport.cs \
    Bus.cs \
    Message.cs \
    BusObject.cs \
    MessageFilter.cs \
    UnixNativeTransport.cs \
    Connection.cs \
    MessageReader.cs \
    UnixTransport.cs \
    DBus.cs \
    MessageWriter.cs \
    Wrapper.cs \
    DProxy.cs \
    Protocol.cs \
    ExportObject.cs \
    Server.cs

REFERENCES = \
    -r:Mono.Posix

dbus_build_sources = $(addprefix $(srcdir)/, $(dbus_sources))

NDesk.DBus.dll: $(dbus_build_sources)
	$(GMCS) $(REFERENCES) -unsafe -target:library -out:$@ $(dbus_build_sources)
