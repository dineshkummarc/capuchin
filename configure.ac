AC_INIT(src/capuchin/Main.cs)
AC_CANONICAL_SYSTEM
AC_PREREQ(2.54)
AM_INIT_AUTOMAKE(capuchin, 0.3.2)
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

dnl Examples
AC_ARG_WITH([examples], AC_HELP_STRING([--with-examples], [Compile examples]),
with_examples=yes, with_examples=no)
AM_CONDITIONAL(WITH_EXAMPLES, test "x$with_examples" = "xyes")

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
NDESKDBUS_REQUIRED_VERSION=0.5.2
PKG_CHECK_MODULES(NDESKDBUS, ndesk-dbus-1.0 >= $NDESKDBUS_REQUIRED_VERSION, has_ndeskdbus=yes, has_ndeskdbus=no)

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
examples/Makefile
capuchin-sharp.pc
src/Makefile
src/libcapuchin/Makefile
src/libcapuchin/Globals.cs
src/libcapuchin/AssemblyInfo.cs
src/capuchin/Makefile
src/capuchin/AssemblyInfo.cs
])

echo "
capuchin-$VERSION

    Install Prefix:               ${prefix}
    Datadir:                      ${expanded_datadir}
    Libdir:                       ${expanded_libdir}
    D-Bus dir:                    ${DBUS_SERVICES_DIR}

    Mono C# Compiler:             ${GMCS}
    Mono Runtime:                 ${MONO}
"
