capuchindir=$(pkglibdir)
capuchin_SCRIPTS = capuchin.exe
EXTRA_DIST = $(capuchin_sources) $(capuchin_sources_in)
CLEANFILES = capuchin.exe
DISTCLEANFILES = AssemblyInfo.cs Makefile.in

capuchin_sources_in = AssemblyInfo.cs.in
capuchin_generated_sources = $(capuchin_sources_in:.in=)
capuchin_sources = Main.cs

capuchin_build_sources = $(addprefix $(srcdir)/, $(capuchin_sources))
capuchin_build_sources += $(capuchin_generated_sources)

REFERENCES = \
    -r:../libcapuchin/capuchin-sharp.dll \
    -r:Mono.GetOptions \
    $(NDESKDBUS_LIBS) \
    $(GTKSHARP_LIBS)

capuchin.exe: $(capuchin_build_sources) $(resources)
	$(GMCS) $(REFERENCES) $(embedded) -out:$@ $(capuchin_build_sources)

# Install Unstable Mono Libraries (see configure.ac)

install-data-hook:
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		$(INSTALL) -c -m 0755 $$ASM $(DESTDIR)$(pkglibdir); \
	done;

uninstall-hook:
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		rm -f $(DESTDIR)$(pkglibdir)/`basename $$ASM`; \
	done;

