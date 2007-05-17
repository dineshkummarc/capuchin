SUBDIRS = data examples specs 
if ENABLE_INTERNAL_NDESKDBUS
SUBDIRS += dbus-sharp
endif
SUBDIRS += src
bin_SCRIPTS = new-stuff-manager
EXTRA_DIST = nsm-sharp.pc.in \
    new-stuff-manager.in

pkgconfigdir = $(libdir)/pkgconfig
pkgconfig_DATA = nsm-sharp.pc

REWRITE = \
	sed -e "s|\@prefix\@|$(prefix)|g"       \
	    -e "s|\@pkglibdir\@|$(pkglibdir)|g" \
	    -e "s|\@bindir\@|$(bindir)|g"		\
	    -e "s|\@MONO\@|$(MONO)|g" 

$(bin_SCRIPTS): $(srcdir)/new-stuff-manager.in Makefile
	$(REWRITE) $(srcdir)/new-stuff-manager.in > $@.tmp
	mv $@.tmp $@

CLEANFILES = \
	new-stuff-manager

DISTCLEANFILES = \
	configure \
	config.guess \
	config.sub \
	Makefile.in \
	install-sh \
	aclocal.m4 \
	INSTALL \
	COPYING \
	missing
