dbusdir = $(pkglibdir)
dbus_SCRIPTS = NDesk.DBus.dll
CLEANFILES = NDesk.DBus.dll
EXTRA_DIST = $(dbus_sources)

dbus_sources = \
    Address.cs \
    AssemblyInfo.cs \
    Bus.cs \
    BusObject.cs \
    Connection.cs \
    ExportObject.cs \
    Authentication.cs \
    Protocol.cs \
    Mapper.cs \
    MatchRule.cs \
    Message.cs \
    MessageFilter.cs \
    MessageReader.cs \
    MessageWriter.cs \
    PendingCall.cs \
    SocketTransport.cs \
    Transport.cs \
    TypeImplementer.cs \
    Wrapper.cs \
    UnixTransport.cs \
    UnixNativeTransport.cs \
    DBus.cs \
    Introspection.cs \
    DProxy.cs \
    Signature.cs

REFERENCES = \
    -r:Mono.Posix

dbus_build_sources = $(addprefix $(srcdir)/, $(dbus_sources))

NDesk.DBus.dll: $(dbus_build_sources)
	$(GMCS) $(REFERENCES) -unsafe -target:library -out:$@ $(dbus_build_sources)
