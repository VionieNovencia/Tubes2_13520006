using System;
using System.Collections.Generic;
using System.IO;
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
using System.Runtime.InteropServices;
using Microsoft.Msagl.Drawing;

namespace FolderCrawler {
	public class Helper {
		/* melakukan pembacaan pada path direktori
			cth : terdapat sebuah path dir = "C:\A\B\D"
					GetRightSide(dir) -> "C:\D"
					GetLeftSide(dir)  -> "C:\A\B"
		*/
		
		public static string GetRightSide(string dir) {					//baca sisi kiri direktori
			StringBuilder result = new StringBuilder();
			for(int i = dir.Length - 1; i > 0; --i) {
				if(dir[i] == '\\') {
					break;
				}
				result.Append(dir[i]);
			}
			return new string(result.ToString().Reverse().ToArray());
		}

		public static string GetLeftSide(string dir) {					//baca sisi kanan direktori
			string temp = GetRightSide(dir);
			StringBuilder sb = new StringBuilder(dir);
			sb.Length -= temp.Length + 1;
			return sb.ToString();
		}
	}
}