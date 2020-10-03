using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.LocalServices;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Rasters;
using Esri.ArcGISRuntime.Tasks;
using Esri.ArcGISRuntime.Tasks.Geoprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace geoprocessing_task_test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Esri.ArcGISRuntime.LocalServices.LocalServer _localServer;
        private Map myMap = new Map();
        private LocalGeoprocessingService _gpService = new LocalGeoprocessingService(@"C:\Temp\tile_package_service.gpkx", GeoprocessingServiceType.AsynchronousSubmit);

        // Hold a reference to the task
        private GeoprocessingTask _gpTask;

        // Hold a reference to the job
        private GeoprocessingJob _gpJob;

        public MainWindow()
        {
            InitializeComponent();
            StartLocalServer();
            Initialize();
        }

        private async void Initialize()
        {
            //Add the geotif to the map. This is the geotif we ultimatly want to convert to a tile package
            Raster myRaster = new Raster(@"C:\Temp\test.tif");
            //await myRaster.LoadAsync();
            RasterLayer newRasterLayer = new RasterLayer(myRaster);
            // Add the raster layer to the maps layer collection.

            myMap.OperationalLayers.Add(newRasterLayer);
            // Assign the map to the map view.
            MyMapView.Map = myMap;
            try
            {
                await newRasterLayer.LoadAsync();
                await MyMapView.SetViewpointGeometryAsync(newRasterLayer.FullExtent);
            }
            catch (Exception exe) 
            {
                MessageBox.Show(exe.Message);
            }
        }

        private async void StartLocalServer()
        {
            // Get the singleton LocalServer object using the static Instance property.
            _localServer = Esri.ArcGISRuntime.LocalServices.LocalServer.Instance;

            // Handle the StatusChanged event to react when the server is started.
            _localServer.StatusChanged += ServerStatusChanged;

            // Start the Local Server instance.
            await _localServer.StartAsync();
            
        }

        private async void ServerStatusChanged(object sender, StatusChangedEventArgs e)
        {
            // Check if the server started successfully.
            if (e.Status == LocalServerStatus.Started)
            {
                await LocalServer.Instance.StartAsync();
            }
        }

        private async void btnGeoProcess_Click(object sender, RoutedEventArgs e)
        {
            //Get the local proprocessing package
            _gpService = new LocalGeoprocessingService(@"C:\Temp\tile_package_service.gpkx",GeoprocessingServiceType.AsynchronousSubmit);
            // Start the local service.
            try
            {
                await _gpService.StartAsync();
            }
            catch (Exception exe)
            {

                MessageBox.Show(exe.Message);
            }
            
            // If the service is started, get the URL for the specific geoprocessing tool.
            string gpSvcUrl = _gpService.Url.AbsoluteUri + "/TP_Model";

            // Create the geoprocessing task with the URL.
            _gpTask = await GeoprocessingTask.CreateAsync(new Uri(gpSvcUrl));
            GeoprocessingParameters gpParams = new GeoprocessingParameters(GeoprocessingExecutionType.AsynchronousSubmit);

            //******************************************
            //***This is the bit I think is wrong how do I pass a map obkect as a gpstring parameter?****
            //*********************************************
            gpParams.Inputs["Input_Map"] = new GeoprocessingString(MyMapView.Name);
            _gpJob = _gpTask.CreateJob(gpParams);
            _gpJob.JobChanged += GeoprocessingJob_JobChanged;
            _gpJob.Start();

        }

        private void GeoprocessingJob_JobChanged(object sender, EventArgs e)
        {
            if (_gpJob.Status == JobStatus.Failed)
            {
                MessageBox.Show("Job Failed " + _gpJob.Messages);
                return;
            }
            if (_gpJob.Status == JobStatus.Succeeded)
            {
                MessageBox.Show("Job Succeeded");
                return;
            }
        }
    }
}
