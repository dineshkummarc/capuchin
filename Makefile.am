SUBDIRS = data specs 
if ENABLE_INTERNAL_NDESKDBUS
SUBDIRS += dbus-sharp
endif
SUBDIRS += src
bin_SCRIPTS = mukkumukku
EXTRA_DIST = mukkumukku-sharp.pc.in \
    mukkumukku.in

pkgconfigdir = $(libdir)/pkgconfig
pkgconfig_DATA = mukkumukku-sharp.pc

REWRITE = \
	sed -e "s|\@prefix\@|$(prefix)|g"       \
	    -e "s|\@pkglibdir\@|$(pkglibdir)|g" \
	    -e "s|\@bindir\@|$(bindir)|g"		\
	    -e "s|\@MONO\@|$(MONO)|g" 

$(bin_SCRIPTS): $(srcdir)/mukkumukku.in Makefile
	$(REWRITE) $(srcdir)/mukkumukku.in > $@.tmp
	mv $@.tmp $@

CLEANFILES = \
	mukkumukku

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
