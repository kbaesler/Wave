Frequently Asked Questions
================================

This will serve as a list of all of the frequently ask questions.

.. note::

    If you don't see your question answered here, please submit an issue on the `GitHub <https://github.com/Jumpercables/Wave/issues>`_ page with your question.

What is Wave?
------------------
Wave is meant to provide a simplistic approach to extending the ArcGIS and ArcFM platforms by providing simplified implementations for complex operations, frequently use call stacks, enforcing the recommended best practices and create a seamless integration with the ArcFM Solution.

What was the motivation?
---------------------------
Over the years as a developer in the Geographic Information System (GIS) for Utilities industry there's been an increasing need for a way to make development easier as the industries needs increase in complexity.

What is ArcGIS?
----------------
`ArcGIS <http://resources.arcgis.com/en/help/getting-started/articles/026n00000014000000.htm>`_ is a comprehensive system that allows people to collect, organize, manage, analyze, communicate, and distribute geographic information. As the world's leading platform for building and using geographic information systems (GIS), ArcGIS is used by people all over the world to put geographic knowledge to work in government, business, science, education, and media. ArcGIS enables geographic information to be published so it can be accessed and used by anyone. The system is available everywhere using web browsers, mobile devices such as smartphones, and desktop computers.

What is ArcFM?
----------------
`ArcFM <http://www.schneider-electric.com/solutions/ww/en/sol/26048721-geospatial-intelligence--arcfm-solution?other=-1>`_ is an Enterprise GIS with reliable network intelligence that provides information such as pipe water direction flows, the number of customers attached to a gas meter, and what devices are downstream of a particular fuse.

Do I need both software packages installed?
-------------------------------------------
Wave has been developed with it's dependencies in mind to allow the library to be used in two different ways:

1. When only the ArcGIS Desktop is installed:

    - You can use the Wave.Extensions.ESRI assembly.

2. When both the ArcGIS Desktop and ArcFM Solution is installed:

    - You can use the Wave.Extensions.ESRI and Wave.Extensions.Miner assemblies.

Is it compatible with 10?
------------------------------------------
Most of the content is non-specific to the version of ArcGIS/ArcFM but there are some dependencies and assumptions about the version(s) of the core software used.

Making the library compatible with 10.x would take a few minor changes:

#.	The assembly references will need to be updated in the project files.
#.	The sample data used for the unit tests will need to be upgraded.
#.	The licensing classes for both ArcGIS and ArcFM will need to be modified to support the new checkout for ArcGIS 10.

.. todo::

  In the future, the code will support multiple versions of the software either using solution / project files or compiler directives.

Do I need a license for Wave?
--------------------------------
Wave is open-source and doesn't directly require any licenses, however since both ArcFM and ArcGIS are commercial software packages that require licenses. You will need a license for ArcGIS and/or ArcFM to use Wave.
