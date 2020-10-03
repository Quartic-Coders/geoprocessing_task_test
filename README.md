# geoprocessing_task_test
 sample code of failed runtime local server generate tile package geoprocessing task. 
 Built using .net runtime 100.8 and runtime local server 100.8


app requires 2 files to be added to c:\temp. The second one is in the repo.
1. test.tif - Any sample geotif file. The app adds this to the map on load, You will have to provide one as mine are 2 large to push to github
2. tile_package_service.gpkx - This is in the repo root directory. This geopressing package that contains a task that converts a map to a map tile package file called oregon.tpkx
