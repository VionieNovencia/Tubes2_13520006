using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
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
using System.Windows.Threading;
using System.Runtime.InteropServices;
using Drawing = Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;

namespace FolderCrawler
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	
	// Tampilan Window dan fungsinya
	public partial class MainWindow : Window
	{
		[DllImport("Kernel32.dll")]
		public static extern bool AttachConsole(int processId);

		string folderDir = "";

		DirectoryTree treeResult;
		bool findAll;
		List<string> allResult;
		
		public MainWindow() {
			AttachConsole(-1);
			InitializeComponent();
		}

		// Memutuskan apa warna garis (merah/biru)
		private bool IsPathToItem(string checkPath, string itemPath) {
			string temp = itemPath;
			while(temp != treeResult.Name) {
				if(temp == checkPath) {
					return true;
				}
				temp = Helper.GetLeftSide(temp);
			}
			return false;
		}

		// Menggambar pohon dengan menggunakan Msagl
		private void DrawTree() {
			Drawing.Graph graph = new Drawing.Graph();

			Queue<string> nodes = new Queue<string>();
			nodes.Enqueue(treeResult.Name);
			while(nodes.Count > 0) {
				string current = nodes.Dequeue();				
				DirectoryTree currentNode = DirectoryTree.FindChild(treeResult, current);
				if(currentNode.Childs != null) {
					foreach(var kvp in currentNode.Childs) {
						nodes.Enqueue(kvp.Key);
					}
				}
				if(current == folderDir) {
					continue;
				}
				DirectoryTree parentNode = DirectoryTree.FindChild(treeResult, Helper.GetLeftSide(current));
				string parentName = Helper.GetRightSide(parentNode.Name);
				string currentName = Helper.GetRightSide(current);
				if(currentNode.Explored) {
					bool isPath = false;
					for(int i = 0; i < allResult.Count; ++i) {
						// Console.WriteLine("Current: " + current);
						// Console.WriteLine("All result: " + allResult[i]);
						if(IsPathToItem(current, allResult[i])) {
							isPath = true;
							break;
						}
					}
					Drawing.Edge edge = graph.AddEdge(
							parentName, 
							currentName);
					if(isPath) {
						edge.Attr.Color = Drawing.Color.Blue;
					} else {
						edge.Attr.Color = Drawing.Color.Red;
					}
				} else {
					graph.AddEdge(parentName, currentName).Attr.Color = Drawing.Color.Black;
				}
			}
			GraphControl.Graph = graph;
		}

		// Input folder pada GUI
		private void OnChooseDir(object sender, RoutedEventArgs e) {
			var dlg = new FolderPicker();
			dlg.InputPath = "c:";
			if (dlg.ShowDialog() == true)
			{
				FolderDirLabel.Content = dlg.ResultPath;
				folderDir = dlg.ResultPath;
			}
		}

		// Tombol search pada GUI
		private void OnSearchButton(object sender, RoutedEventArgs e) {
			if(folderDir == "") {
				MessageBox.Show("Please Choose a folder first");
				return;
			}
			if(FileToSearch.Text == "") {
				MessageBox.Show("Please choose a file to search for");
				return;
			}
			if((RadioDFS.IsChecked == false) && (RadioBFS.IsChecked == false)) {
				MessageBox.Show("Please Choose either BFS or DFS before proceeding");
				return;
			}

			// Memulai penghitungan waktu eksekusi program
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			findAll = FindAll.IsChecked == true;

			allResult = new List<string>();
			if(RadioBFS.IsChecked == true) {
				SearchBFS();
			} else {
				SearchDFS();
			}
			DrawTree();

			// Program selesai, hentikan penghitungan waktu
			stopwatch.Stop();
			TimeSpan ts = stopwatch.Elapsed;

			string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
									            ts.Hours, ts.Minutes, ts.Seconds,
									            ts.Milliseconds / 10);
			TimeTaken.Content = "Time taken: " + elapsedTime;
		}

		// Menerapkan BFS
		public void SearchBFS() {
			bool first = true;
			Queue<string> queue = new Queue<string>();
			DirectoryTree root = null;
			queue.Enqueue(folderDir);
			while(queue.Count > 0) {
				string current = queue.Dequeue();
				if(root == null) {
					root = new DirectoryTree(current);
					root.Explored = true;
					string[] newDirs = Directory.GetDirectories(current);
					for(int i = 0; i < newDirs.Length; ++i) {
						queue.Enqueue(newDirs[i]);
					}
					string[] files = Directory.GetFiles(current);
					bool found = false;
					string tempRes = "";
					foreach(var file in files) {
						if(Helper.GetRightSide(file) == FileToSearch.Text) {
							found = true;
							tempRes = file;
						}
						DirectoryTree newNode = new DirectoryTree(file);
						root.AddChild(newNode);
						newNode.Explored = true;
					}
					if(found) {
						first = false;
						PathText.Text = tempRes + Environment.NewLine;
						allResult.Add(tempRes);
						if(!findAll) {
							break;
						}
					}
				} else {
					DirectoryTree newNode = new DirectoryTree(current);
					newNode.Explored = true;
					DirectoryTree parentNode = DirectoryTree.FindChild(root, Helper.GetLeftSide(current));
					parentNode.AddChild(newNode);
					string[] allDirs = Directory.GetDirectories(current);
					foreach(var dir in allDirs) {
						queue.Enqueue(dir);
					}					
					string[] files = Directory.GetFiles(current);
					bool found = false;
					string result = "";
					foreach(var file in files) {
						if(Helper.GetRightSide(file) == FileToSearch.Text) {
							found = true;
							result = file;
						}
						DirectoryTree fileNode = new DirectoryTree(file);
						fileNode.Explored = true;
						newNode.AddChild(fileNode);
					}
					if(found) {
						if(first) {
							PathText.Text = result + Environment.NewLine;
							first = false;
						} else {
							PathText.Text += result + Environment.NewLine;
						}
						allResult.Add(result);
						if(!findAll) {
							break;
						}
					}
				}
			}
			while(queue.Count > 0) {
				string current = queue.Dequeue();
				DirectoryTree newNode = new DirectoryTree(current);
				if(Helper.GetRightSide(current) == FileToSearch.Text) {
					newNode.Explored = true;
				} else {
					newNode.Explored = false;
				}
				DirectoryTree parentNode = DirectoryTree.FindChild(root, Helper.GetLeftSide(current));
				parentNode.AddChild(newNode);
			}
			if(allResult.Count == 0) {
				PathText.Text = "No file found!";
			}
			treeResult = root;
		}

		// Menerapkan DFS
		public void SearchDFS() {
			bool first = true;
			Stack<string> stack = new Stack<string>();
			DirectoryTree root = null;
			stack.Push(folderDir);
			while(stack.Count > 0) {
				string current = stack.Pop();
				if(root == null) {
					root = new DirectoryTree(current);
					root.Explored = true;
					string[] newDirs = Directory.GetDirectories(current);
					for(int i = 0; i < newDirs.Length; ++i) {
						stack.Push(newDirs[i]);
					}
					string[] files = Directory.GetFiles(current);
					bool found = false;
					string tempRes = "";
					foreach(var file in files) {
						if(Helper.GetRightSide(file) == FileToSearch.Text) {
							found = true;
							tempRes = file;
						}
						DirectoryTree newNode = new DirectoryTree(file);
						root.AddChild(newNode);
						newNode.Explored = true;
					}

					if(found) {
						first = false;
						PathText.Text = tempRes + Environment.NewLine;
						allResult.Add(tempRes);
						if(!findAll) {
							break;
						}
					}
				} else {
					DirectoryTree newNode = new DirectoryTree(current);
					newNode.Explored = true;
					DirectoryTree parentNode = DirectoryTree.FindChild(root, Helper.GetLeftSide(current));
					parentNode.AddChild(newNode);
					string[] allDirs = Directory.GetDirectories(current);
					foreach(var dir in allDirs) {
						stack.Push(dir);
					}					
					string[] files = Directory.GetFiles(current);
					bool found = false;
					string result = "";
					foreach(var file in files) {
						if(Helper.GetRightSide(file) == FileToSearch.Text) {
							found = true;
							result = file;
						}
						DirectoryTree fileNode = new DirectoryTree(file);
						fileNode.Explored = true;
						newNode.AddChild(fileNode);
					}
					if(found) {
						if(first) {
							PathText.Text = result + Environment.NewLine;
							first = false;
						} else {
							PathText.Text += result + Environment.NewLine;
						}
						allResult.Add(result);
						if(!findAll) {
							break;
						}
					}
				}
			}
			while(stack.Count > 0) {
				string current = stack.Pop();
				DirectoryTree newNode = new DirectoryTree(current);
				if(Helper.GetRightSide(current) == FileToSearch.Text) {
					newNode.Explored = true;
				} else {
					newNode.Explored = false;
				}
				DirectoryTree parentNode = DirectoryTree.FindChild(root, Helper.GetLeftSide(current));
				parentNode.AddChild(newNode);
			}
			if(allResult.Count == 0) {
				PathText.Text = "No file found!";
			}
			treeResult = root;
		}
	}
}
