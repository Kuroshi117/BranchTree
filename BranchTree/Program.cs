//Phoebe Huang
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
namespace BranchTree
{
    class Program
    {
        public static List<string> Text = new List<string>();
        public static List<Tree> Nodes = new List<Tree>();
        public static bool IsReady = false;
        static void Main(string[] args)
        {
            LoadText(@"C:\workspace\names.tab", Text);
            if (IsReady == true)
            {

                SortBranchTree(Text, Nodes);
                ReadNodes(Nodes);
                Inputs();
            }

            Console.ReadKey();
        }

        public static void LoadText(string path, List<string> text)
        {
            string line;

            using (StreamReader sr = new StreamReader(path))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    text.Add(line);
                }

            }
            if (Text != null)
            {
                IsReady = true;
            }
        }

        public static void SortBranchTree(List<string> text, List<Tree> nodes)
        {
            string tempName;
            for (int i = 0; i < text.Count; i++)
            {
                tempName = text[i].Trim();

                nodes.Add(new Tree(tempName));
                nodes[i].Depth = NumberOfOcc(text[i], "\t");
                if (NumberOfOcc(text[i], "\t") == 0)
                {
                    AddNode(nodes[i], null);
                }
                else if ((NumberOfOcc(text[i], "\t")) > (NumberOfOcc(text[i - 1], "\t")))
                {
                    AddNode(nodes[i], nodes[i - 1].id());

                }
                else if ((NumberOfOcc(text[i], "\t")) == (NumberOfOcc(text[i - 1], "\t")))
                {
                    AddNode(nodes[i], nodes[i - 1].Parent.id());


                }
                else if ((NumberOfOcc(text[i], "\t")) < (NumberOfOcc(text[i - 1], "\t")))
                {
                    int d = NumberOfOcc(text[i], "\t");
                    int j = nodes.Count - 2;
                    while (nodes[j].Depth != d)
                    {
                        j--;
                    }
                    AddNode(nodes[i], nodes[j].Parent.id());



                }
            }
        }

        public static void AddNode(Tree tree, string parentID)
        {
            
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].id() == parentID)
                {
                    tree.Parent = Nodes[i];
                    tree.Depth = Nodes[i].Depth + 1;
                    Nodes[i].Children.Add(tree);
                    break;

                }
                
            }

        }

        public static void AddNewNode(string name, string parentID)
        {
            Tree tree = new Tree(name.Trim());
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].id() == parentID)
                {
                    tree.Parent = Nodes[i];
                    tree.Depth = Nodes[i].Depth + 1;
                    Nodes[i].Children.Add(tree);
                    Nodes.Insert(i + 1, tree);
                    break;

                }
				else if ( parentID=="null")
				{
					tree.Parent = null;
					tree.Depth = 0;
					Nodes.Add(tree);
					break;

				}
                
            }
            
        }

        public static void DeleteNode(string id)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                 if (Nodes[i].id() == id)
                {
                    if(Nodes[i].Parent != null)
                    {
                        Nodes[i].Parent.Children.Remove(Nodes[i]);
                    }
                    
                    Nodes[i].Parent = null;
                    foreach (Tree c in Nodes[i].Children)
                    {
                        c.Parent = null;
                        Nodes.Remove(c);
                    }
                    Nodes[i].Children.Clear();

                    Nodes.RemoveAt(i);
                    break;
                }
                
            }
        }

        public static void MoveNode(string id, string parentID)
        {
            //reference from https://www.dotnetperls.com/list-find
            Nodes.Find(tree => tree.id() == id).Parent.Children.Remove(Nodes.Find(tree => tree.id() == id));
            Nodes.Find(tree => tree.id() == id).Parent = Nodes.Find(tree => tree.id() == parentID);
            SetDepth(id);
            Nodes.Find(tree => tree.id() == parentID).Children.Add(Nodes.Find(tree => tree.id() == id));


        }

        public static Tree FindNode(string id)
        {
            return Nodes.Find(tree => tree.id() == id);
        }

        public static List<Tree> FindNodebyContent(string contentID)
        {
            List<Tree> GotNodes = new List<Tree>();
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].Content() == contentID)
                {
                    GotNodes.Add(Nodes[i]);

                }
                
            }
            
            return GotNodes;
        }

        public static List<Tree> FindLeaves()
        {
            List<Tree> GotNodes = new List<Tree>();
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (!Nodes[i].Children.Any())
                {
                    GotNodes.Add(Nodes[i]);

                }

            }

            return GotNodes;
        }

        public static List<Tree> FindInternalNodes()
        {
            List<Tree> GotNodes = new List<Tree>();
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].Parent == null && !Nodes[i].Children.Any())
                {
                    GotNodes.Add(Nodes[i]);

                }

            }

            return GotNodes;
        }



        public static int NumberOfOcc(string text, string pattern)
        {
            int count = 0;
            int i = 0;
            while ((i = text.IndexOf(pattern, i)) != -1)
            {
                i += pattern.Length;
                count++;
            }
            return count;
        }

        public static void SetDepth(string id)
        {
            Nodes.Find(tree => tree.id() == id).Depth = Nodes.Find(tree => tree.id() == id).Parent.Depth + 1;
            if(Nodes.Find(tree => tree.id() == id).Children != null)
            {
                foreach (Tree t in Nodes.Find(tree => tree.id() == id).Children)
                {
                    SetDepth(t.id());
                }
            }
        }

        public static void ReadNodes(List<Tree> nodes)
        {

            List<Tree> Roots = new List<Tree>();
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].Depth == 0)
                {
                    Roots.Add(nodes[i]);
                }
            }
            foreach (Tree t in Roots)
            {
                ReadChildren(t);
            }

        }

        public static void ReadChildren(Tree node)
        {
            string tabs;
            tabs = new string('\t', node.Depth);
            Console.WriteLine(tabs + node.Content()+", "+node.id());
            if (node.Children != null)
            {
                foreach (Tree t in node.Children)
                {
                    ReadChildren(t);
                }
            }
        }

        public static void WriteOutlineFile(List<Tree> nodes, string filename)
        {
            using (StreamWriter sw = new StreamWriter(@"C:\workspace\"+filename+".txt"))
            {
                //basic way
                /*string tabs;
                for (int i = 0; i < nodes.Count; i++)
                {
                    tabs = new string('\t', nodes[i].Depth);
                    sw.WriteLine(tabs + nodes[i].Content());
                }*/

                //hierarichal way
                List<Tree> Roots = new List<Tree>();
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (nodes[i].Depth == 0)
                    {
                        Roots.Add(nodes[i]);
                    }
                }
                foreach (Tree t in Roots)
                {
                    WriteChildren(t, sw);
                }

            }
        }

        public static void WriteChildren(Tree node, StreamWriter sw)
        {
            string tabs;
            tabs = new string('\t', node.Depth);
            sw.WriteLine(tabs + node.Content()+", "+ node.id());
            if (node.Children != null)
            {
                foreach (Tree t in node.Children)
                {
                    WriteChildren(t, sw);
                }
            }
        }

        public static void Inputs()
        {
            Console.WriteLine("\n---INPUTS--- \nadd parentID,nameOfNewNode \nremove nodeToRemoveID \nmove nodeToMoveID,newParentID \nget ID \nget name \nget leaves \nget internal \nwrite fileName \nexit");
            string inputString = "";
            string[] inputArray = new string[4];
            char[] c = new char[]{ ' ', ',' };
            while(true)
            {
                inputString = Console.ReadLine();
                inputArray = inputString.Split(c, StringSplitOptions.RemoveEmptyEntries);
                Console.WriteLine("\n");

                if (inputArray[0].Equals("add", StringComparison.InvariantCultureIgnoreCase))
                {
                    AddNewNode(inputArray[2], inputArray[1]);
                    ReadNodes(Nodes);
                }
                else if (inputArray[0].Equals("remove", StringComparison.InvariantCultureIgnoreCase))
                {
                    DeleteNode(inputArray[1]);
                    ReadNodes(Nodes);
                }
                else if (inputArray[0].Equals("move", StringComparison.InvariantCultureIgnoreCase))
                {
                    MoveNode(inputArray[1], inputArray[2]);
                    ReadNodes(Nodes);
                }
                else if (inputArray[0].Equals("get", StringComparison.InvariantCultureIgnoreCase))
                {
                    
                    if(inputArray.Length <= 1)
                    {
                        Console.WriteLine("Nothing to find. ");
                        continue;
                    }

                    if (inputArray[1].Equals("leaves", StringComparison.InvariantCultureIgnoreCase))
                    {
                        List<Tree> tempTreeList = FindLeaves();
                        if (tempTreeList.Any())
                        {
                            Console.WriteLine("Found: ");
                            foreach (Tree t in tempTreeList)
                            {
                                Console.WriteLine(t.Content() + ", " + t.id());
                            }
                        }
                        else
                        {
                            Console.WriteLine("No leaves found.");
                        }
                    }
                    else if (inputArray[1].Equals("internal", StringComparison.InvariantCultureIgnoreCase))
                    {
                        List<Tree> tempTreeList = FindInternalNodes();
                        if (tempTreeList.Any())
                        {
                            Console.WriteLine("Found: ");
                            foreach (Tree t in tempTreeList)
                            {
                                Console.WriteLine(t.Content() + ", " + t.id());
                            }
                        }
                        else
                        {
                            Console.WriteLine("No internal nodes found.");
                        }
                    }
					else
					{
						if (FindNode(inputArray[1]) != null)
						{
							Tree tempTree = FindNode(inputArray[1]);
							Console.WriteLine("Found:\n" + tempTree.Content() + ", " + tempTree.id());

						}
						else if (FindNodebyContent(inputArray[1]) != null)
						{
							List<Tree> tempTreeList = FindNodebyContent(inputArray[1]);
							if (tempTreeList.Any())
							{
								Console.WriteLine("Found: ");
								foreach (Tree t in tempTreeList)
								{
									Console.WriteLine(t.Content() + ", " + t.id());
								}
							}
							else
							{
								Console.WriteLine("Not found.");
							}
						}
						else
						{
							Console.WriteLine(inputArray[1] + " not found.");
						}
					}
                    

                    
                }
                else if (inputArray[0].Equals("write", StringComparison.InvariantCultureIgnoreCase))
                {
                    WriteOutlineFile(Nodes, inputArray[1]);
                    Console.WriteLine(inputArray[1]+".txt was written.");
                }
                else if(inputArray[0].Equals("exit", StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.WriteLine("Exiting program.");
                    break;
                }
                else
                {
                    Console.WriteLine("Please enter valid input");
                }
                
            }
        }



        /*public static void SortTree(List<string> text, List<Tree> nodes)
        {
            string tempName;
            for (int i = 0; i < text.Count; i++)
            {
                tempName = text[i].Trim();

                nodes.Add(new Tree(tempName));
                nodes[i].Depth = NumberOfOcc(text[i], "\t");
                if (NumberOfOcc(text[i], "\t") == 0)
                {
                    continue;
                }
                else if ((NumberOfOcc(text[i], "\t")) > (NumberOfOcc(text[i - 1], "\t")))
                {
                    nodes[i].Parent = nodes[i - 1];
                    nodes[i - 1].Children.Add(nodes[i]);
                }
                else if ((NumberOfOcc(text[i], "\t")) == (NumberOfOcc(text[i - 1], "\t")))
                {
                    nodes[i].Parent = nodes[i - 1].Parent;
                    if (nodes[i - 1].Parent == null)
                    {
                        continue;
                    }
                    nodes[i - 1].Parent.Children.Add(nodes[i]);
                }
                else if ((NumberOfOcc(text[i], "\t")) < (NumberOfOcc(text[i - 1], "\t")))
                {
                    int d = NumberOfOcc(text[i], "\t");
                    int j = nodes.Count - 1;
                    while (nodes[j].Depth <= d)
                    {
                        j--;
                    }
                    nodes[i].Parent = nodes[j];
                    nodes[j].Children.Add(nodes[i]);

                }
            }
        }

        public static void SortTreeOther(List<string> text, List<Tree> nodes)
        {
            for (int i = 0; i < text.Count; i++)
            {

                if (NumberOfOcc(text[i], "\t") == 0)
                {
                    nodes.Add(AddNodeAlt(null, text[i]));
                }
                else if ((NumberOfOcc(text[i], "\t")) > (NumberOfOcc(text[i - 1], "\t")))
                {
                    nodes.Add(AddNodeAlt(nodes[nodes.Count - 1], text[i]));


                }
                else if ((NumberOfOcc(text[i], "\t")) == (NumberOfOcc(text[i - 1], "\t")))
                {
                    nodes.Add(AddNodeAlt(nodes[nodes.Count - 1].Parent, text[i]));
                }
                else if ((NumberOfOcc(text[i], "\t")) < (NumberOfOcc(text[i - 1], "\t")))
                {
                    int d = NumberOfOcc(text[i], "\t");
                    int j = nodes.Count - 1;
                    while (nodes[j].Depth != d)
                    {
                        j--;
                    }
                    nodes.Add(AddNodeAlt(nodes[j].Parent, text[i]));

                }

            }
        }

        public static Tree AddNodeAlt(Tree parent, string name)
        {
            Tree tree = new Tree(name.Trim());
            if (parent != null)
            {
                tree.Parent = parent;
                tree.Depth = parent.Depth + 1;
                parent.Children.Add(tree);
            }
            else
            {
                tree.Depth = 0;
            }
            return tree;
        } 

        public static void RemoveNode(Tree node)
        {
            node.Parent = null;
            foreach (Tree c in node.Children)
            {
                c.Parent = null;
            }
            node.Children = null;
            node = null;

        }

        public static Tree GetNode(string name)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].Content() == name)
                {
                    //Console.WriteLine(Nodes[i].Content());
                    return Nodes[i];

                }
                else
                {
                    Console.WriteLine(name + " not found.");
                }

            }
            return null;
        }

        public static List<Tree> GetNodes(string name)
        {
            List<Tree> GotNodes = new List<Tree>();
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].Content() == name)
                {
                    GotNodes.Add(Nodes[i]);

                }
                else
                {
                    Console.WriteLine(name + " not found.");
                }
            }
            for (int j = 0; j < GotNodes.Count; j++)
            {
                Console.WriteLine(GotNodes[j].Content());

            }
            return GotNodes;
        }

        public static List<Tree> GetLeaves()
        {
            List<Tree> GotNodes = new List<Tree>();
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (!Nodes[i].Children.Any())
                {
                    GotNodes.Add(Nodes[i]);

                }

            }
            for (int j = 0; j < GotNodes.Count; j++)
            {

                Console.WriteLine(GotNodes[j].Content());

            }
            return GotNodes;
        }

        public static List<Tree> GetInternalNodes()
        {
            List<Tree> GotNodes = new List<Tree>();
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].Parent == null && !Nodes[i].Children.Any())
                {
                    GotNodes.Add(Nodes[i]);

                }

            }
            for (int j = 0; j < GotNodes.Count; j++)
            {

                Console.WriteLine(GotNodes[j].Content());

            }
            return GotNodes;
        }*/
    }

    class Tree: INode
    {
        private string Name;
        public Tree Parent;
        public List<Tree> Children = new List<Tree>();
        public int Depth;
        private static Random rand = new Random();
        private string identifier;
        public Tree(string n)
        {
            Name = n;
            identifier=GenerateID();
        }

        public string id()
        {
            return identifier;
        }

        public string Content()
        {
            return Name;
        }

        public bool IsReady()
        {
            return true;
        }

        private string GenerateID()
        {
            //reference from https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings-in-c
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] CharArray = new char[8];
            for (int i=0; i<CharArray.Length; i++)
            {
                CharArray[i] = chars[rand.Next(chars.Length)];
            }
            return new string(CharArray);
        }
    }

    interface INode
    {
        string id();
        string Content();
        bool IsReady();

    }

}
