nsmdir=$(pkglibdir)
nsm_SCRIPTS = new-stuff-manager.exe
EXTRA_DIST = $(nsm_sources) $(nsm_sources_in)
CLEANFILES = new-stuff-manager.exe
DISTCLEANFILES = AssemblyInfo.cs Makefile.in

nsm_sources_in = AssemblyInfo.cs.in
nsm_generated_sources = $(nsm_sources_in:.in=)
nsm_sources = Main.cs

nsm_build_sources = $(addprefix $(srcdir)/, $(nsm_sources))
nsm_build_sources += $(nsm_generated_sources)

REFERENCES = \
    -r:../libnsm/nsm-sharp.dll \
    -r:Mono.GetOptions \
    $(NDESKDBUS_LIBS) \
    $(GTKSHARP_LIBS)

new-stuff-manager.exe: $(nsm_build_sources) $(resources)
	$(GMCS) $(REFERENCES) $(embedded) -out:$@ $(nsm_build_sources)

# Install Unstable Mono Libraries (see configure.ac)

install-data-hook:
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		$(INSTALL) -c -m 0755 $$ASM $(DESTDIR)$(pkglibdir); \
	done;

uninstall-hook:
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		rm -f $(DESTDIR)$(pkglibdir)/`basename $$ASM`; \
	done;

