using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using System.Diagnostics;

namespace KinectHeightMeasurement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor _sensor;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _sensor = KinectSensor.KinectSensors.FirstOrDefault();//KinectSensor.KinectSensors.Where(x => x.Status == KinectStatus.Connected).FirstOrDefault();

            if (_sensor != null)
            {
                
                _sensor.SkeletonFrameReady += Sensor_SkeletonFrameReady;//new EventHandler<SkeletonFrameReadyEventArgs>(Sensor_SkeletonFrameReady);
                //_sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                _sensor.SkeletonStream.Enable();


                _sensor.Start();
            }
            else
            {
                MessageBox.Show("No Kinect Connected!");
                Close();
            }
        }

        void Sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            
            using (var frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    canvas.Children.Clear();

                    Skeleton[] skeletons = new Skeleton[frame.SkeletonArrayLength];

                    frame.CopySkeletonDataTo(skeletons);

                    var skeleton = skeletons.Where(s => s.TrackingState == SkeletonTrackingState.Tracked).FirstOrDefault();

                    if (skeleton != null)
                    {
                        // Calculate height.
                        double height = Math.Round(skeleton.Height(), 2);
                        double armExtendsWidth = Math.Round(skeleton.ArmExtendWith(), 2);
                        // Draw skeleton joints.
                        foreach (JointType joint in Enum.GetValues(typeof(JointType)))
                        {
                            DrawJoint(skeleton.Joints[joint].ScaleTo(640, 480));
                        }

                        // Display height.
                        double inches = height * 39.37;
                        double feet = Math.Floor(inches / 12);
                        inches = inches - feet * 12;

                        tblHeight.Text = String.Format("Height: {0}feet {1}inches,{2}m", feet, inches, height);
                        tblArmExtendWidth.Text = String.Format("ArmWidth: {0} m", armExtendsWidth);
                    }
                }
            }
        }

        private void DrawJoint(Joint joint)
        {
            Ellipse ellipse = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = new SolidColorBrush(Colors.LightCoral)
            };

            Canvas.SetLeft(ellipse, joint.Position.X);
            Canvas.SetTop(ellipse, joint.Position.Y);

            canvas.Children.Add(ellipse);
        }
    }
}
