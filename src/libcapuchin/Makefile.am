libcapuchindir=$(pkglibdir)
libcapuchin_SCRIPTS = capuchin-sharp.dll

libcapuchin_sources_in = AssemblyInfo.cs.in Globals.cs.in
libcapuchin_generated_sources = $(libcapuchin_sources_in:.in=)
libcapuchin_sources = \
	Installation/Compression/Decompresser.cs \
	Installation/Compression/IExtracter.cs \
	Installation/Compression/TarBz2Extracter.cs \
	Installation/Compression/TarGzExtracter.cs \
	Installation/Compression/ZipExtracter.cs \
	Installation/Compression/TarExtracter.cs \
	Installation/Simple/SimpleInstaller.cs \
	Installation/IInstaller.cs \
	Downloaders/AbstractDownloader.cs \
	Downloaders/HttpDownloader.cs \
	Logging/DelayedTextWriter.cs \
	Logging/FileSystem.cs \
	Logging/Log.cs \
	Logging/TeeTextWriter.cs \
	Verification/ChecksumVerifier.cs \
	Verification/GnuPGVerifier.cs \
	Verification/IVerifier.cs \
	Xml/author.cs \
	Xml/changelog.cs \
	Xml/checksum.cs \
	Xml/item.cs \
	Xml/ItemsDict.cs \
	Xml/Repository.cs \
	AppObject.cs \
	AppObjectManager.cs \
	Download.cs \
	DownloadManager.cs \
	IAppObject.cs \
	IAppObjectManager.cs \
	IDownloadManager.cs	 \
	RepositoryConnectionException.cs \
	RepositoryNotInitializedException.cs \
	NoSuchPluginException.cs

REFERENCES =  \
	-r:System \
	-r:System.Xml \
	-r:ICSharpCode.SharpZipLib \
	-r:Mono.Posix \
	$(GNOMEVFSSHARP_LIBS)

if ENABLE_INTERNAL_NDESKDBUS
REFERENCES += -r:$(top_srcdir)/$(NDESKDBUS_LIBS)
else
REFERENCES += $(NDESKDBUS_LIBS)
endif

libcapuchin_build_sources = $(addprefix $(srcdir)/, $(libcapuchin_sources))
libcapuchin_build_sources += $(libcapuchin_generated_sources)

capuchin-sharp.dll: $(libcapuchin_build_sources)
	$(GMCS) $(REFERENCES) -target:library -out:$@ $(libcapuchin_build_sources)
	
CLEANFILES = capuchin-sharp.dll

DISTCLEANFILES = \
    AssemblyInfo.cs \
    Globals.cs \
    Makefile.in
    
EXTRA_DIST = \
    $(libcapuchin_sources) \
    $(libcapuchin_sources_in)
