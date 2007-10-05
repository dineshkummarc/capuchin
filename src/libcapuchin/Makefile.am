libcapuchindir=$(pkglibdir)
libcapuchin_SCRIPTS = capuchin-sharp.dll
CLEANFILES = capuchin-sharp.dll
DISTCLEANFILES = \
    AssemblyInfo.cs \
    Globals.cs \
    Makefile.in
EXTRA_DIST = \
    $(libcapuchin_sources) \
    $(libcapuchin_sources_in)

libcapuchin_sources_in = AssemblyInfo.cs.in Globals.cs.in
libcapuchin_generated_sources = $(libcapuchin_sources_in:.in=)
libcapuchin_sources = \
	Capuchin.cs \
	Repository.cs \
	AppObject.cs \
	Download.cs \
	DownloadManager.cs \
	Downloaders/AbstractDownloader.cs \
	Downloaders/HttpDownloader.cs \
	Downloaders/FtpDownloader.cs \
	Compression/Decompresser.cs \
	Compression/IExtracter.cs \
	Compression/TarBz2Extracter.cs \
	Compression/TarGzExtracter.cs \
	Compression/ZipExtracter.cs \
	Compression/TarExtracter.cs \
	Verification/ChecksumVerifier.cs \
	Verification/GnuPGVerifier.cs \
	Verification/IVerifier.cs \
	Logging/DelayedTextWriter.cs \
	Logging/FileSystem.cs \
	Logging/Log.cs \
	Logging/TeeTextWriter.cs

REFERENCES =  \
	-r:System \
	-r:System.Xml \
	-r:ICSharpCode.SharpZipLib \
	-r:Mono.Posix \
	$(NDESKDBUS_LIBS) \
	$(GNOMEVFSSHARP_LIBS)

libcapuchin_build_sources = $(addprefix $(srcdir)/, $(libcapuchin_sources))
libcapuchin_build_sources += $(libcapuchin_generated_sources)

capuchin-sharp.dll: $(libcapuchin_build_sources)
	$(GMCS) $(REFERENCES) -target:library -out:$@ $(libcapuchin_build_sources)
