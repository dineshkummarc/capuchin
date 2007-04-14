mukkumukkudir=$(pkglibdir)
mukkumukku_SCRIPTS = mukkumukku.exe
EXTRA_DIST = $(mukkumukku_sources) $(mukkumukku_sources_in)
CLEANFILES = mukkumukku.exe
DISTCLEANFILES = AssemblyInfo.cs Makefile.in

mukkumukku_sources_in = AssemblyInfo.cs.in
mukkumukku_generated_sources = $(nsm_sources_in:.in=)
mukkumukku_sources = Main.cs

mukkumukku_build_sources = $(addprefix $(srcdir)/, $(mukkumukku_sources))
mukkumukku_build_sources += $(mukkumukku_generated_sources)

REFERENCES = \
    -r:../libmukkumukku/mukkumukku-sharp.dll \
    -r:Mono.GetOptions \
    $(NDESKDBUS_LIBS) \
    $(GTKSHARP_LIBS)

mukkumukku.exe: $(mukkumukku_build_sources) $(resources)
	$(GMCS) $(REFERENCES) $(embedded) -out:$@ $(mukkumukku_build_sources)

# Install Unstable Mono Libraries (see configure.ac)

install-data-hook:
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		$(INSTALL) -c -m 0755 $$ASM $(DESTDIR)$(pkglibdir); \
	done;

uninstall-hook:
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		rm -f $(DESTDIR)$(pkglibdir)/`basename $$ASM`; \
	done;

