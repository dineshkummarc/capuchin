libmukkumukkudir=$(pkglibdir)
libmukkumukku_SCRIPTS = mukkumukku-sharp.dll
CLEANFILES = mukkumukku-sharp.dll
DISTCLEANFILES = \
    AssemblyInfo.cs \
    Globals.cs \
    Makefile.in
EXTRA_DIST = \
    $(libmukkumukku_sources) \
    $(libmukkumukku_sources_in)

libmukkumukku_sources_in = AssemblyInfo.cs.in Globals.cs.in
libmukkumukku_generated_sources = $(libmukkumukku_sources_in:.in=)
libmukkumukku_sources = \
	NewStuffManager.cs \
	Repository.cs \
	NewStuff.cs \
	ConfParser.cs \
	Downloader.cs \
	Compression/Decompresser.cs \
	Compression/IExtracter.cs \
	Compression/TarBz2Extracter.cs \
	Compression/TarGzExtracter.cs \
	Compression/ZipExtracter.cs \
	Compression/TarExtracter.cs \
	Verification/ChecksumVerifier.cs \
	Verification/GnuPGVerifier.cs \
	Verification/IVerifier.cs 

REFERENCES =  \
	-r:System \
	-r:System.Xml \
	-r:ICSharpCode.SharpZipLib \
	$(NDESKDBUS_LIBS) \
	$(GNOMEVFSSHARP_LIBS)

libmukkumukku_build_sources = $(addprefix $(srcdir)/, $(libmukkumukku_sources))
libmukkumukku_build_sources += $(libmukkumukku_generated_sources)

mukkumukku-sharp.dll: $(libmukkumukku_build_sources)
	$(GMCS) $(REFERENCES) -target:library -out:$@ $(libmukkumukku_build_sources)
