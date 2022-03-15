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
		public static string GetRightSide(string dir) {
			StringBuilder result = new StringBuilder();
			for(int i = dir.Length - 1; i > 0; --i) {
				if(dir[i] == '\\') {
					break;
				}
				result.Append(dir[i]);
			}
			return new string(result.ToString().Reverse().ToArray());
		}

		public static string GetLeftSide(string dir) {
			string temp = GetRightSide(dir);
			StringBuilder sb = new StringBuilder(dir);
			sb.Length -= temp.Length + 1;
			return sb.ToString();
		}
	}
}