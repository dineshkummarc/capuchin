libnsmdir=$(pkglibdir)
libnsm_SCRIPTS = nsm-sharp.dll
CLEANFILES = nsm-sharp.dll
DISTCLEANFILES = \
    AssemblyInfo.cs \
    Globals.cs \
    Makefile.in
EXTRA_DIST = \
    $(libnsm_sources) \
    $(libnsm_sources_in)

libnsm_sources_in = AssemblyInfo.cs.in Globals.cs.in
libnsm_generated_sources = $(libnsm_sources_in:.in=)
libnsm_sources = \
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

libnsm_build_sources = $(addprefix $(srcdir)/, $(libnsm_sources))
libnsm_build_sources += $(libnsm_generated_sources)

nsm-sharp.dll: $(libnsm_build_sources)
	$(GMCS) $(REFERENCES) -target:library -out:$@ $(libnsm_build_sources)
