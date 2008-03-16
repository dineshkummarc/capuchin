SUBDIRS = data src
if WITH_EXAMPLES
SUBDIRS += examples
endif

bin_SCRIPTS = capuchin
EXTRA_DIST = capuchin-sharp.pc.in \
    capuchin.in

pkgconfigdir = $(libdir)/pkgconfig
pkgconfig_DATA = capuchin-sharp.pc

REWRITE = \
	sed -e "s|\@prefix\@|$(prefix)|g"       \
	    -e "s|\@pkglibdir\@|$(pkglibdir)|g" \
	    -e "s|\@bindir\@|$(bindir)|g"		\
	    -e "s|\@MONO\@|$(MONO)|g" 

$(bin_SCRIPTS): $(srcdir)/capuchin.in Makefile
	$(REWRITE) $(srcdir)/capuchin.in > $@.tmp
	mv $@.tmp $@

CLEANFILES = \
	capuchin

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
