using System.Collections.Generic;

namespace FolderCrawler 
{
	public class DirectoryTree 
	{
		string name;
		IDictionary<string, DirectoryTree> childs;
		bool explored;

		public string Name {
			get {
				return name;
			}
		}

		public IDictionary<string, DirectoryTree> Childs {
			get {
				return childs;
			}
		}

		public bool Explored {
			get {
				return explored;
			}
			set {
				explored = value;
			}
		}

		public DirectoryTree(string name) {
			this.name = name;
			this.childs = null;
			explored = false;
		}

		public void AddChild(DirectoryTree newChild) {
			if(this.childs == null) {
				this.childs = new Dictionary<string, DirectoryTree>();
			}

			this.childs.Add(newChild.Name, newChild);
		}

		/**
		 * This function is used in order to get the childs of root that has the name x
		 * It achieves this by using the name stored
		 * If the Directory is
		 * C:\A\B\D\E
		 * it'll loop through those until it gets the name that's in the root
		 * let's say root is C:\A
		 * it'll remove E from C:\A\B\D
		 * and then add it to List
		 * and then remove  D from it
		 * and then add C:\A\B into list
		 * until it gets to the same name as root
		 * at which point it'll stop the looking for, and start
		 * traversing until it gets the child DirectoryTree node
		*/

		/* DirectoryTree FindChild(DirectoryTree root, string name)
			Mencari path yang berkorespondensi dengan name pada direktoriTree root.
			cth :
				FindChild(root, "C:\A\B\D\E") ; dimana root = C:\A
					iter 1 : rootChilds = [C:\A\B\D\E]
							 current    = 


				keluaran : sebuah directoryTree bernilai C:\A\B\D
		*/
		public static DirectoryTree FindChild(DirectoryTree root, string name) {
			List<string> rootChilds = new List<string>();
			string current = name;
			while(current != root.Name) {
				rootChilds.Add(current);
				current = Helper.GetLeftSide(current);
			}
			DirectoryTree traverse = root;
			// traverse = traverse.Childs[name];
			for(int i = rootChilds.Count - 1; i >= 0; --i) {
				traverse = traverse.Childs[rootChilds[i]];
			}

			return traverse;
		}
	}
}