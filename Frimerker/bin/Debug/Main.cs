using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.Diagnostics;

using System.Drawing.Imaging;

using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;


namespace Frimerker
{
	class MainClass
	{
		public static Document document;
		public static PdfWriter writer;

		static void Main (string[] args)
		{
/*			for (int page = 147; page <= 147; page++) {
				DirectoryInfo directory = new DirectoryInfo ("/Users/PKN/Temp/");
				FileInfo[] files = directory.GetFiles ();
				foreach (FileInfo file in files)
					file.Delete ();

				HentFiler (page); 
				for (int i = 0; i <= 3; i++)
					CombineImagesWidth (i);

				CombineImagesHeight (page);
			}
*/
			ConvertToPdf ();
			//NSApplication.Init ();
			//NSApplication.Main (args);
		}
			
		public static void HentFiler(int page)
		{
			WebClient client = new WebClient ();
			 
			int index = 0;

			for (int i = 0; i <= 3; i++)
				for (int j = 0; j <= 2; j++) {
					index++;
					string download = "Fett" + index.ToString().PadLeft(3, '0');

					client.DownloadFile ("http://www.nb.no/services/image/resolver?url_ver=geneza&urn=URN:NBN:no-nb_digibok_2009042800046_0" + page + 
										 "&maxLevel=6&level=6" +
										 "&col=" + j +
										 "&row=" + i +
										 "&resX=2520&resY=3744&tileWidth=1024&tileHeight=1024&pg_id=" + page,
										 "/Users/PKN/Temp/" + download);
				}
		}

		public static void CombineImagesWidth(int i)
		{
			DirectoryInfo directory = new DirectoryInfo ("/Users/PKN/Temp/");
			FileInfo[] files = directory.GetFiles ("Fett*");

			string finalImage = "/Users/PKN/Temp/WidthImage" + i + ".jpg";

			List <int> imageHeights = new List <int> ();
			int nIndex = 0;
			int width = 0;

			for (int j = 3 * i; j <= 3 * i + 2; j++) {
				FileInfo file = files [j];
				System.Drawing.Image img = System.Drawing.Image.FromFile (file.FullName);
				
				imageHeights.Add (img.Height);
				width += img.Width;

				img.Dispose ();
			}

			imageHeights.Sort ();
			int height = imageHeights [imageHeights.Count - 1];
			Bitmap img3 = new Bitmap (width, height);
			Graphics g = Graphics.FromImage (img3);
			g.Clear (SystemColors.AppWorkspace);

			for (int j = 3 * i; j <= 3 * i + 2; j++) {
				FileInfo file = files [j];
				System.Drawing.Image img = System.Drawing.Image.FromFile (file.FullName);

				if (nIndex == 0) {
					g.DrawImage (img, new Point (0, 0));
					nIndex++;
					width = img.Width;
				} else {
					g.DrawImage (img, new Point (width, 0));
					width += img.Width;
				}

				img.Dispose ();
			}

			g.Dispose ();
			img3.Save (finalImage, System.Drawing.Imaging.ImageFormat.Jpeg);
			img3.Dispose ();
		}

		public static void CombineImagesHeight(int page)
		{
			DirectoryInfo directory = new DirectoryInfo ("/Users/PKN/Temp/");
			FileInfo[] files = directory.GetFiles ("WidthImage*");

			string finalImage = "/Users/PKN/Downloads/Page" + page.ToString().PadLeft(3, '0') + ".jpg";

			List <int> imageWidths = new List <int> ();
			int nIndex = 0;
			int height = 0;

			for (int j = 0; j <= 3; j++) {
				FileInfo file = files [j];
				System.Drawing.Image img = System.Drawing.Image.FromFile (file.FullName);

				imageWidths.Add (img.Width);
				height += img.Height;

				img.Dispose ();
			}

			imageWidths.Sort ();
			int width = imageWidths [imageWidths.Count - 1];
			Bitmap img3 = new Bitmap (width, height);
			Graphics g = Graphics.FromImage (img3);
			g.Clear (SystemColors.AppWorkspace);

			for (int j = 0; j <= 3; j++) {
				FileInfo file = files [j];
				System.Drawing.Image img = System.Drawing.Image.FromFile (file.FullName);

				if (nIndex == 0) {
					g.DrawImage (img, new Point (0, 0));
					nIndex++;
					height = img.Height;
				} else {
					g.DrawImage (img, new Point (0, height));
					height += img.Height;
				}

				img.Dispose ();
			}

			g.Dispose ();
			img3.Save (finalImage, System.Drawing.Imaging.ImageFormat.Jpeg);
			img3.Dispose ();
		}

		public static void ConvertToPdf()
		{
			iTextSharp.text.Rectangle pagesize = new iTextSharp.text.Rectangle(2520/20, 3744/20);
			document = new Document (pagesize);
			writer = PdfWriter.GetInstance (document, new FileStream ("/Users/PKN/Temp/Bok.pdf", FileMode.Create));
			writer.CompressionLevel = 0;
			document.Open();

			AddPage ("Page146.jpg");

			for (int i = 146; i <= 147; i++)
				AddPage ("Page" + i + ".jpg");

			document.Close();
		}

		public static void AddPage(string pagename)
		{
			iTextSharp.text.Image img;
			//float Krymp;

			img = iTextSharp.text.Image.GetInstance ("/Users/PKN/Temp/" + pagename);
			//Krymp = Math.Min ((PageSize.A4.Height / img.Height), (PageSize.A4.Width / img.Width));
			img.ScalePercent (5);//(Krymp * 100);
			img.SetAbsolutePosition (0, 0);
			writer.DirectContent.AddImage (img);

			document.NewPage ();
		}
	}
}
