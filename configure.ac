AC_INIT(src/nsm/Main.cs)
AC_CANONICAL_SYSTEM
AC_PREREQ(2.54)
AM_INIT_AUTOMAKE(new-stuff-manager, 0.2.3)
AM_MAINTAINER_MODE

AC_PROG_INSTALL

dnl pkg-config
AC_PATH_PROG(PKG_CONFIG, pkg-config, no)
if test "x$PKG_CONFIG" = "xno"; then
	AC_MSG_ERROR([You need to install pkg-config])
fi

AC_PATH_PROG(MONO, mono, no)

dnl C# Compiler
AC_PATH_PROG(GMCS, gmcs, no)
if test "x$GMCS" = "xno"; then
        AC_MSG_ERROR([gmcs Not found])
fi
AC_SUBST(GMCS)

dnl Expansions
SHAMROCK_EXPAND_DATADIR
SHAMROCK_EXPAND_LIBDIR

dnl ****************************************************************************
dnl * mono
dnl ****************************************************************************
MONO_REQUIRED_VERSION=1.1.13
PKG_CHECK_MODULES(MONO, mono >= $MONO_REQUIRED_VERSION, has_mono=true, has_mono=false)
if test "x$has_mono" = "xfalse"; then
      AC_MSG_ERROR([Please install mono version $MONO_REQUIRED_VERSION or later])
fi

dnl ****************************************************************************
dnl * GPG
dnl ****************************************************************************
AC_PATH_PROG(GPG, gpg, no)
if test "x$GPG" = "xno"; then
  AC_MSG_ERROR(you need to have gpg installed !)
fi

dnl ****************************************************************************
dnl * gnome-vfs-sharp
dnl ****************************************************************************
GNOMEVFSSHARP_REQUIRED_VERSION=2.0
PKG_CHECK_MODULES(GNOMEVFSSHARP, gnome-vfs-sharp-2.0 >= $GNOMEVFSSHARP_REQUIRED_VERSION)

dnl ****************************************************************************
dnl * ndesk-dbus
dnl ****************************************************************************
AC_ARG_ENABLE([internal-dbus], AS_HELP_STRING([--enable-internal-dbus], [Use internal ndesk-dbus library (default: auto)]),
enable_internal_ndeskdbus=$enableval, enable_internal_ndeskdbus=auto)

NDESKDBUS_REQUIRED_VERSION=0.5.2
PKG_CHECK_MODULES(NDESKDBUS, ndesk-dbus-1.0 >= $NDESKDBUS_REQUIRED_VERSION, has_ndeskdbus=yes, has_ndeskdbus=no)

if test "x$has_ndeskdbus" = "xno"; then
	if test "x$enable_internal_ndeskdbus" = "xno"; then
		AC_MSG_ERROR([Could not find ndesk-dbus library])
	fi
	enable_internal_ndeskdbus="yes"
	NDESKDBUS_LIBS="-r:../../dbus-sharp/NDesk.DBus.dll"
else
	if test "x$enable_internal_ndeskdbus" = "xyes"; then
		enable_internal_ndeskdbus="yes"
		NDESKDBUS_LIBS= "-r:../../dbus-sharp/NDesk.DBus.dll"
	else
		enable_internal_ndeskdbus="no"
	fi
fi
AM_CONDITIONAL(ENABLE_INTERNAL_NDESKDBUS, test "x$enable_internal_ndeskdbus" = "xyes")
AC_SUBST(NDESKDBUS_LIBS)

dnl ****************************************************************************
dnl * Check for DBUS Services directory
dnl ****************************************************************************
DBUS_REQUIRED_VERSION=0.60
PKG_CHECK_MODULES(DBUS, dbus-1 >= $DBUS_REQUIRED_VERSION)
DBUS_SERVICES_DIR="$expanded_datadir/dbus-1/services"
AC_SUBST(DBUS_SERVICES_DIR)
AC_DEFINE_UNQUOTED(DBUS_SERVICES_DIR, "$DBUS_SERVICES_DIR", [DBus services directory])

AC_OUTPUT([
Makefile
data/Makefile
dbus-sharp/Makefile
examples/Makefile
nsm-sharp.pc
specs/Makefile
src/Makefile
src/libnsm/Makefile
src/libnsm/Globals.cs
src/libnsm/AssemblyInfo.cs
src/nsm/Makefile
src/nsm/AssemblyInfo.cs
])

echo "
new-stuff-manager-$VERSION

    Install Prefix:               ${prefix}
    Datadir:                      ${expanded_datadir}
    Libdir:                       ${expanded_libdir}
    D-Bus dir:                    ${DBUS_SERVICES_DIR}

    Mono C# Compiler:             ${GMCS}
    Mono Runtime:                 ${MONO}
    Use internal D-Bus library:   ${enable_internal_ndeskdbus}
"
