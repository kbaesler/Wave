Search Services for ArcMap and ArcCatalog
========================================================

A set of WCF services that are hosted within ArcMap and ArcCatalog extensions that provide text-based and map-based search services. Theses services allow callers to search the data in the current context of either the ArcMap or ArcCatalog using predefined or dynamic searches. 

Text-based Search Service
---------------------------
The text-based search services allows for querying a set of table(s), class(es) and relationship(s) using the data in the active session. 

- allows for equals, not-equals, and like (contains, stars-with and ends-with) matches.

Map-based Search Service
---------------------------
The map based search operates on the same principles as the text-based search service, however additional the matching rows are linked to a feature so that it can be located on a map. When tables are searched, matches will be linked to the feature that participates in a relationship (that is 

- allows for spatial extent restrictions/filtering, such as any, current extent or overlapping current extent.
