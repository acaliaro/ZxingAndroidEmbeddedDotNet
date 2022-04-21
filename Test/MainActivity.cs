using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using Android.Widget;
using Com.Journeyapps.Barcodescanner;
using Com.Google.Zxing;
using AndroidX.Activity.Result;
using AndroidX.Activity.Result.Contract;
using static AndroidX.Activity.Result.Contract.ActivityResultContracts;

namespace Test
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IActivityResultCallback
    {

        private ActivityResultLauncher _barcodeLauncher;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            AndroidX.AppCompat.Widget.Toolbar toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            _barcodeLauncher = RegisterForActivityResult(new ActivityResultContracts.StartActivityForResult(), this);
            ImageView imageViewBarcode = FindViewById<ImageView>(Resource.Id.image_view_barcode);

            try
            {
                BarcodeEncoder barcodeEncoder = new BarcodeEncoder();
                Android.Graphics.Bitmap bitmap = null;
                bitmap = barcodeEncoder.EncodeBitmap("content", BarcodeFormat.QrCode , 400, 400);
                
                imageViewBarcode.SetImageBitmap(bitmap);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            //Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
            //    .SetAction("Action", (View.IOnClickListener)null).Show();

            ScanOptions options = new ScanOptions();
            options.SetDesiredBarcodeFormats(ScanOptions.DataMatrix);
            options.SetPrompt(GetString(Resource.String.read_datamatrix));
            options.SetCameraId(0);
            options.SetBeepEnabled(true);
            options.SetBarcodeImageEnabled(true);
            options.SetOrientationLocked(false);
            _barcodeLauncher.Launch(options.CreateScanIntent(this));
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void OnActivityResult(Java.Lang.Object p0)
        {
            if(p0 is ActivityResult)
            {
                string scanResult = ((ActivityResult)p0).Data.GetStringExtra("SCAN_RESULT");

                TextView textViewBarcode = FindViewById<TextView>(Resource.Id.text_view_barcode);
                textViewBarcode.Text = scanResult;
            }
        }
    }
}
