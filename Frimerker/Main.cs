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
		const string source_name = "no-nb_digibok_2013070108295_";

		const int maxLevel = 6;
		const int level = 4;
		const int x_tiles = 0;
		const int y_tiles = 0;
		const int resX = 2608;
		const int resY = 3576;

		const int start_page = 1;
		const int stop_page = 248;

		public static Document document;
		public static PdfWriter writer;

		static void Main (string[] args)
		{
			HentBackFront (start_page - 1);

			DirectoryInfo directory = new DirectoryInfo ("/Users/PKN/Downloads/");
			FileInfo[] files = directory.GetFiles ("WidthImage*");

			HentBackFront (stop_page + 1);

			directory = new DirectoryInfo ("/Users/PKN/Downloads/");
			files = directory.GetFiles ("WidthImage*");

			foreach (var item in files) {
				File.Delete (item.FullName);
			}

			for (int page = start_page; page <= stop_page; page++) {
				HentFiler (page); 

				CombineImagesWidth (0, x_tiles);

				CombineImagesHeight (page);
			}

			ConvertToPdf ();
		}

		public static void HentBackFront(int page)
		{
			const int BFmaxLevel = 6;
			const int BFlevel = 4;
			const int BFx_tiles = 0;
			const int BFy_tiles = 0;
			const int BFresX = 2758;
			const int BFresY = 3847;

			WebClient client = new WebClient ();

			string pageName;
			int index = 0;

			if (page == start_page - 1)
				pageName = "C1";
			else
				pageName = "C3";
			
			for (int i = 0; i <= BFy_tiles; i++)
				for (int j = 0; j <= BFx_tiles; j++) {
					index++;
					string download = "Fett" + index.ToString().PadLeft(3, '0');

					client.DownloadFile ("http://www.nb.no/services/image/resolver?url_ver=geneza&urn=URN:NBN:" + source_name + pageName +
						"&maxLevel=" + BFmaxLevel + 
						"&level=" + BFlevel +
						"&col=" + j +
						"&row=" + i +
						"&resX=" + BFresX +
						"&resY=" + BFresY +
						"&tileWidth=1024&tileHeight=1024&pg_id=" + page,
						"/Users/PKN/Downloads/" + download + ".jpg");
				}
					
			CombineImagesWidth (0, BFx_tiles);

			CombineImagesHeight (page);
		}
			
		public static void HentFiler(int page)
		{
			WebClient client = new WebClient ();
			 
			int index = 0;

			for (int i = 0; i <= y_tiles; i++)
				for (int j = 0; j <= x_tiles; j++) {
					index++;
					string download = "Fett" + index.ToString().PadLeft(3, '0');

					client.DownloadFile ("http://www.nb.no/services/image/resolver?url_ver=geneza&urn=URN:NBN:" + source_name + page.ToString().PadLeft(4, '0') + 
										 "&maxLevel=" + maxLevel + 
										 "&level=" + level +
										 "&col=" + j +
										 "&row=" + i +
										 "&resX=" + resX +
										 "&resY=" + resY +
										 "&tileWidth=1024&tileHeight=1024&pg_id=" + page,
										 "/Users/PKN/Downloads/" + download + ".jpg");
				}
		}

		public static void CombineImagesWidth(int i, int count)
		{
			DirectoryInfo directory = new DirectoryInfo ("/Users/PKN/Downloads/");
			FileInfo[] files = directory.GetFiles ("Fett*");

			string finalImage = "/Users/PKN/Downloads/WidthImage" + i + ".jpg";

			List <int> imageHeights = new List <int> ();
			int nIndex = 0;
			int width = 0;

			for (int j = i; j <= i + count; j++) {
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

			for (int j = i; j <= i + count; j++) {
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
			DirectoryInfo directory = new DirectoryInfo ("/Users/PKN/Downloads/");
			FileInfo[] files = directory.GetFiles ("WidthImage*");

			string finalImage = "/Users/PKN/Downloads/Page" + page.ToString ().PadLeft (3, '0') + ".jpg";

			List <int> imageWidths = new List <int> ();
			int nIndex = 0;
			int height = 0;

			foreach (var item in files) {
				FileInfo file = item;
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

			foreach (var item in files) {
				FileInfo file = item;
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
			iTextSharp.text.Rectangle pagesize = new iTextSharp.text.Rectangle (PageSize.A4.Width, PageSize.A4.Height);
			document = new Document (pagesize);
			writer = PdfWriter.GetInstance (document, new FileStream ("/Users/PKN/Downloads/Bok.pdf", FileMode.Create));
			document.Open ();

			for (int i = start_page - 1; i <= stop_page + 1; i++)
				AddPage ("Page" + i.ToString().PadLeft(3, '0') + ".jpg");

			document.Close ();
		}

		public static void AddPage(string pagename)
		{
			iTextSharp.text.Image img;
			float Krymp;

			img = iTextSharp.text.Image.GetInstance ("/Users/PKN/Downloads/" + pagename);
			Krymp = Math.Min ((PageSize.A4.Height / img.Height), (PageSize.A4.Width / img.Width));
			img.ScalePercent ((Krymp * 100), (Krymp * 100));
			img.SetAbsolutePosition ((PageSize.A4.Width / img.Width * 100 / 2), 0);
			img.CompressionLevel = 9;
			writer.DirectContent.AddImage (img);
			document.NewPage ();
		}
	}
}
