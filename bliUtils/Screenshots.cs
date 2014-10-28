using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace bliUtils
{
    public class Screenshots
    {
        public static void createScreenshot(Window window, string savePath){
                BitmapEncoder encoder = new PngBitmapEncoder();
                RenderTargetBitmap render = new RenderTargetBitmap((int)window.ActualWidth, (int)window.ActualHeight, 96.0, 96.0, PixelFormats.Pbgra32);
                render.Render(window);
                encoder.Frames.Add(BitmapFrame.Create(render));
			    try
				{	string path = savePath;
                    if (File.Exists(path)) {
                        File.Delete(path);   
                    }
                    using (FileStream fs = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.Read))
				    {
                            encoder.Save(fs);
                            fs.Dispose();
                            fs.Close();
                    }
                }
                catch (Exception e)
                {
                    Debug.Print("Exception on screen save: " + e.Message);
                }

        }
    }
}
