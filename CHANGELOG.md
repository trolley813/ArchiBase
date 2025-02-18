## 0.3.0 (2025-02-04)
- upgraded to .NET 9.0
- switched to WebAssembly for the UI
- added Web API (experimental, stabilization scheduled to version 0.4.0)
- added comments search
- added location labels with flags to comment authors
- added list of photos per design
- added ability to specify location upon register 
- added ability to edit architects for local editors
- fixed error in upload limit calculation

## 0.2.9 (2024-12-31)
- fixed cadastre link for Russia
- fixed builder/customer translation
- added New Year/Christmas garland

## 0.2.8 (2024-12-08)
- fixed photo upload page
- added linking of renamed streets
- when street is renamed, the name effective to shooting date is displayed on the photo page
- added suggestion of building binds upon uploading photos

## 0.2.7 (2024-12-01)
- added editing of design entries inside the catalogue
- added ability to change password
- added viewing photos by location
- many tiny fixes

## 0.2.6 (2024-11-19)
- added photo editing (coordinates, binds, dates, EXIF)
- added comment citing
- added comment editing/deleting (only one's own unresponded, no later than 24 hours after writing)
- optimized some memory usage

## 0.2.5 (2024-11-12)
- fixed excessive memory usage on large map
- added sorting/filtering by dates on the general building list

## 0.2.4 (2024-11-10)
- fixed photo upload
- added city/locations buildings list and stats by design/category
- added role badges to comment authors
- added placeholders for Yandex maps (not working yet)

## 0.2.3 (2024-11-07)
- new Leaflet-based map
- photo previews
- some heavy pages wrapped into a loading screen
- fix when uploading multiple photos
- several tiny bug fixes

## 0.2.2 (2024-10-27)
- more responsive table layouts
- street list now displayed by letter in several columns
- automatic street sorting
- fixed editing of buildings
- allowed restoring of photos
- add photos to comment list

## 0.2.1 (2024-10-16)
- added architects management (add/edit)
- added ability to change personal info
- added author name filtering in photos list
- fixed typesetting issues
- added photo lost flag
- several tiny bug fixes

## 0.2.0 (2024-10-13)
- new material design theme
- added light/dark theme switcher
- added photo moderation
- added voting
- added list of building for design
- map markers ard now colored depending on building status
- more user-friendly mobile interface
- several bug fixes

## 0.1.14 (2024-10-05)
- significantly lowered memory consuption in photos and comments page
- added large map 
- added photo counter to user page

## 0.1.13 (2024-10-03)
- added EXIF view and uploading
- added design display upon photo upload (when binding a building)
- added photo list view
- added link to the commented entity in comment view
- added loading screen for some "heavy" pages (photos, comments)
- added some Estonian localization

## 0.1.12 (2024-09-24)
- fixed street editing (no save button)
- fixed design editing (catalogue entries now adding properly)
- added galleries
- added photo upload limit to 2 MiB (2097152 bytes)

## 0.1.11 (2024-08-27)
- fixed authentication behind proxy

## 0.1.10 (2024-08-26)
- improved street view (map, obsolete addresses etc.)

## 0.1.9 (2024-08-19)
- reworked admin panel (less queries)
- added building table row coloring based on status

## 0.1.8 (2024-08-16)
- enriched data structure (new migrations)
- added code for editing new fields

## 0.1.7 (2024-08-14)
- enriched data structure (new migrations)

## 0.1.6 (2024-07-25)
- removed certificate revocation check in email client
- stubs for bulk edit features

## 0.1.5 (2024-07-03)
- local editor features
- quick building creation upon adding a street
- photo uploading (experimental, direct only)

## 0.1.4 (2024-06-24)
- another set of new features
- more Russian localization

## 0.1.3 (2024-06-18)
- new porting features
- fixes in design and code
- *Warning:* Migrations are reset

## 0.1.2 (2024-06-09)
- Added most of the entities needed to port PB data backup

## 0.1.1 (2024-05-31)
- Applying migrations from inside Docker
- Added `docker-compose` example into `README.md`

## 0.1.0 (2024-05-30)
- Initial release