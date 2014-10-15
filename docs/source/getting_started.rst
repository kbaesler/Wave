Getting Started
================================
Wave is a C# library that extends the ESRI ArcObjects and Schneider Electric ArcFM 
APIs that are used for developing extensions to the ArcMap and ArcFM platforms. The library has been developed using extension methods to extend the functionality of existing objects and provides workable wrappers around commonly used COM objects.

The concept is to eliminate the need for the developer to learn new namespaces and api, but allow them to take advantage of the Visual Studio IDE to identify the new methods for objects. That being said, it's always best to consult the :doc:`api` documentation to understand the purpose of the methods and classes.

There are two ways for getting started using Wave.

GitHub
---------------------
You can visit `GitHub <https://github.com/Jumpercables/Wave>`_ to download and compiled the source. Once compiled you can reference the assemblies in your projects.



Microsoft Package Manager
--------------------------------------
In the future, Wave will be publicly distributed using the `Nuget <http://www.nuget.org>`_ service that is available to the .NET community within Visual Studios. 

- This will allow users the option to simply install the assemblies without having to compile the source code.

.. warning::

    This is on the roadmap but not implemented yet.

Requirements
--------------------
The prerequisites for developing and running Wave require a combination of open source and commercial software.

- `ArcGIS Desktop 9.3.1 <http://www.esri.com/software/arcgis>`_
- `ArcFM Solution 9.3.1 <http://www.schneider-electric.com/products/ww/en/6100-network-management-software/6120-geographic-information-system-arcfm-solution/62051-arcfm/>`_
- `Apache log4net 2.0.3 <https://github.com/apache/log4net>`_
- `Microsoft .NET Framework 3.5 SP1 <http://www.microsoft.com/en-us/download/details.aspx?id=22>`_