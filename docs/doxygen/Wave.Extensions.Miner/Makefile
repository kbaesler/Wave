$
# Makefile for Doxygen documentation
#

DOXYGEN   ?= `which doxygen`
TMAKEPATH =
ENV       = env TMAKEPATH=$(TMAKEPATH)
TMAKE     =
MAKE      = /usr/bin/make
PERL      = /usr/bin/perl
RM        = rm -f
CP        = cp
VERSION   = 1.5.7.1
INSTALL   = /tmp
INSTTOOL  = /usr/bin/install
DOCDIR    = $(INSTALL)/share/doc/packages/doxygen
QTDIR     =
HAVE_DOT  = /usr/bin/dot

all: Wave.Extensions.Miner/xml/index.xml

clean:
	rm -rf  Wave.Extensions.Miner

Wave.Extensions.Miner/xml/index.xml:
	$(DOXYGEN)
