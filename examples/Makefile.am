examplesdir = $(docdir)/examples
examples_DATA = \
	DownloadManager.py \
	testapp.cs \
	testapp.py

testapp_csharpdir = $(docdir)/examples
testapp_csharp_SCRIPTS	= testapp.exe
testapp_csharp_sources = testapp.cs

testapp_csharp_build_sources = $(addprefix $(srcdir)/, $(testapp_csharp_sources))

REFERENCES = \
    -r:$(top_srcdir)/src/libcapuchin/capuchin-sharp.dll \
    -r:$(top_srcdir)/$(NDESKDBUS_LIBS) \
    $(GTKSHARP_LIBS)

testapp.exe: $(testapp_csharp_build_sources) $(resources)
	$(GMCS) $(REFERENCES) $(embedded) -out:$@ $(testapp_csharp_build_sources)

CLEANFILES = \
    testapp.exe

EXTRA_DIST = \
	$(examples_DATA) \
	$(testapp_sources)
